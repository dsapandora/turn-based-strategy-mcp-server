using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Units;
using TurnBasedStrategyFramework.Common.Units.Abilities;
using TurnBasedStrategyFramework.Common.Utilities;
using TurnBasedStrategyFramework.Unity.Units;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Mcp
{
    /// <summary>
    /// MCP server that exposes TBS game tools (get_world_state, move_unit, attack_unit, end_turn)
    /// for the Rocket Ride agent. Uses Streamable HTTP transport (POST to /mcp with JSON-RPC).
    /// </summary>
    public class TBSMcpServer : MonoBehaviour
    {
        [SerializeField] private int _port = 8765;
        [SerializeField] private string _path = "/mcp";
        [SerializeField] private bool _debugLog;
        [Tooltip("Log when MCP client calls a tool (action summary only).")]
        [SerializeField] private bool _logToolCalls = true;
        [Tooltip("Log every MCP request received and response sent.")]
        [SerializeField] private bool _logRequests = false;

        private HttpListener _listener;
        private CancellationTokenSource _cts;
        private Task _listenTask;
        private bool _running;

        private readonly object _contextLock = new object();
        private GridController _gridController;
        private List<IUnit> _playableUnits = new List<IUnit>();
        private string _projectId = "";
        private string _role = "player";

        private readonly object _toolQueueLock = new object();
        private readonly Queue<PendingToolCall> _toolQueue = new Queue<PendingToolCall>();

        private readonly object _completionQueueLock = new object();
        private readonly Queue<AsyncCompletion> _completionQueue = new Queue<AsyncCompletion>();

        private readonly StringBuilder _recentOutcomes = new StringBuilder();

        private struct AsyncCompletion { public PendingToolCall Pending; public bool Success; public string Message; }
        private string _lastTurnSummary = "";

        public int Port => _port;
        public string Endpoint => $"http://localhost:{_port}{_path}";
        public bool IsRunning => _running;

        public void SetTurnContext(GridController gridController, List<IUnit> playableUnits, string projectId = "", string role = "player")
        {
            lock (_contextLock)
            {
                _gridController = gridController;
                _playableUnits = playableUnits ?? new List<IUnit>();
                _projectId = projectId ?? "";
                _role = role ?? "player";
            }
        }

        public void ClearTurnContext()
        {
            lock (_contextLock)
            {
                _gridController = null;
                _playableUnits?.Clear();
            }
            lock (_recentOutcomes)
            {
                _recentOutcomes.Clear();
            }
        }

        public void StartServer()
        {
            if (_running)
            {
                if (_debugLog) Debug.Log("[TBSMcpServer] Already running.");
                return;
            }

            try
            {
                _listener = new HttpListener();
                _listener.Prefixes.Add($"http://127.0.0.1:{_port}/");
                _listener.Prefixes.Add($"http://localhost:{_port}/");
                _listener.Start();
                _cts = new CancellationTokenSource();
                _running = true;
                _listenTask = Task.Run(() => ListenLoop(_cts.Token));
                Debug.Log($"[TBSMcpServer] Started at {Endpoint}");
            }
            catch (HttpListenerException ex)
            {
                Debug.LogError($"[TBSMcpServer] Failed to start: {ex.Message}. Is port {_port} already in use? (lsof -i :{_port})");
                throw;
            }
        }

        public void StopServer()
        {
            _running = false;
            _cts?.Cancel();
            try { _listener?.Stop(); } catch (Exception) { }
            _listener?.Close();
            _listener = null;
            Debug.Log("[TBSMcpServer] Stopped.");
        }

        private void ListenLoop(CancellationToken token)
        {
            while (_running && !token.IsCancellationRequested)
            {
                try
                {
                    var context = _listener.GetContext();
                    if (context.Request.Url.AbsolutePath.TrimEnd('/') != _path.TrimEnd('/'))
                    {
                        context.Response.StatusCode = 404;
                        context.Response.Close();
                        continue;
                    }
                    _ = Task.Run(() => HandleRequest(context), token);
                }
                catch (HttpListenerException) { break; }
                catch (ObjectDisposedException) { break; }
                catch (Exception ex)
                {
                    if (_running && _debugLog) Debug.LogWarning($"[TBSMcpServer] Listen error: {ex.Message}");
                }
            }
        }

        private void HandleRequest(HttpListenerContext context)
        {
            try
            {
                if (context.Request.HttpMethod == "DELETE")
                {
                    context.Response.StatusCode = 202;
                    context.Response.Close();
                    return;
                }

                if (context.Request.HttpMethod != "POST")
                {
                    context.Response.StatusCode = 405;
                    context.Response.Close();
                    return;
                }

                string body;
                using (var reader = new StreamReader(context.Request.InputStream, Encoding.UTF8))
                    body = reader.ReadToEnd();

                if (string.IsNullOrWhiteSpace(body))
                {
                    SendJsonResponse(context.Response, 400, "{\"error\":\"Empty body\"}");
                    return;
                }

                var method = SimpleJson.GetString(body, "method");
                var id = SimpleJson.GetInt(body, "id") ?? (int?)0;
                var hasId = Regex.IsMatch(body, "\"id\"\\s*:");
                object result = null;

                if (method == "initialize")
                {
                    result = new McpInitResult { protocolVersion = "2025-11-25", serverInfo = new McpServerInfo { name = "TBS-MCP-Unity", version = "1.0" }, capabilities = new McpCapabilities() };
                }
                else if (method == "notifications/initialized")
                {
                    context.Response.StatusCode = 202;
                    context.Response.Close();
                    return;
                }
                else if (method == "tools/list")
                {
                    result = HandleToolsList();
                }
                else if (method == "tools/call")
                {
                    var paramsObj = SimpleJson.GetObject(body, "params");
                    result = HandleToolsCall(paramsObj ?? body);
                }
                else
                {
                    var err = new McpError { code = -32601, message = "Method not found: " + method };
                    var jsonErr = "{\"jsonrpc\":\"2.0\",\"id\":" + id + ",\"error\":" + JsonUtility.ToJson(err) + "}";
                    SendJsonResponse(context.Response, 200, jsonErr);
                    return;
                }

                if (!hasId)
                {
                    context.Response.StatusCode = 202;
                    context.Response.Close();
                    return;
                }

                var resultJson = result is McpInitResult init ? JsonUtility.ToJson(init) :
                    result is McpToolsListResult tl ? JsonUtility.ToJson(tl) :
                    result is McpToolCallResult tc ? JsonUtility.ToJson(tc) : "{}";
                var json = "{\"jsonrpc\":\"2.0\",\"id\":" + id + ",\"result\":" + resultJson + "}";
                SendJsonResponse(context.Response, 200, json);
            }
            catch (Exception ex)
            {
                if (_debugLog) Debug.LogWarning($"[TBSMcpServer] Request error: {ex.Message}");
                try { SendJsonResponse(context.Response, 500, "{\"error\":\"" + EscapeJson(ex.Message) + "\"}"); } catch { }
            }
            finally
            {
                context.Response?.Close();
            }
        }

        private static void SendJsonResponse(HttpListenerResponse response, int statusCode, string json)
        {
            response.StatusCode = statusCode;
            response.ContentType = "application/json; charset=utf-8";
            var bytes = Encoding.UTF8.GetBytes(json);
            response.ContentLength64 = bytes.Length;
            response.OutputStream.Write(bytes, 0, bytes.Length);
        }

        private object HandleToolsList()
        {
            var tools = new List<McpTool>
            {
                new McpTool { name = "get_world_state", description = "Get the current world state: strategic summary and full JSON. Use first to plan actions.", inputSchema = new McpSchema { type = "object" } },
                new McpTool { name = "move_unit", description = "Move a playable unit to a valid cell. Use ONLY coordinates from 'Can move to' in get_world_state.", inputSchema = new McpSchema { type = "object", required = new[] { "unit_id", "target_x", "target_y" }, properties = new McpProperties { unit_id = new McpProp { type = "integer" }, target_x = new McpProp { type = "integer" }, target_y = new McpProp { type = "integer" } } } },
                new McpTool { name = "attack_unit", description = "Attack an enemy. Use ONLY target_unit_id from 'Can attack' in get_world_state.", inputSchema = new McpSchema { type = "object", required = new[] { "unit_id", "target_unit_id" }, properties = new McpProperties { unit_id = new McpProp { type = "integer" }, target_unit_id = new McpProp { type = "integer" } } } },
                new McpTool { name = "end_turn", description = "Finish your turn so the enemy can move. MUST call after all your units have moved and attacked.", inputSchema = new McpSchema { type = "object" } }
            };
            return new McpToolsListResult { tools = tools.ToArray() };
        }

        private object HandleToolsCall(string paramsJson)
        {
            if (string.IsNullOrWhiteSpace(paramsJson))
                return WrapToolResult(new ToolResult { success = false, error = "Missing params" });

            var name = SimpleJson.GetString(paramsJson, "name");
            var argsJson = SimpleJson.GetObject(paramsJson, "arguments");
            if (string.IsNullOrEmpty(name))
                return WrapToolResult(new ToolResult { success = false, error = "Missing tool name" });

            var unitId = SimpleJson.GetInt(argsJson, "unit_id") ?? 0;
            var targetX = SimpleJson.GetInt(argsJson, "target_x") ?? 0;
            var targetY = SimpleJson.GetInt(argsJson, "target_y") ?? 0;
            var targetUnitId = SimpleJson.GetInt(argsJson, "target_unit_id") ?? 0;

            if (_logToolCalls)
            {
                var msg = name == "move_unit" ? $"[TBSMcpServer] MCP → {name} unit {unitId} → ({targetX},{targetY})" :
                    name == "attack_unit" ? $"[TBSMcpServer] MCP → {name} unit {unitId} → target {targetUnitId}" :
                    $"[TBSMcpServer] MCP → {name}";
                Debug.Log(msg);
            }

            // All tools run on main thread and block until complete (agent waits for each action before next).
            var pending = new PendingToolCall { Name = name, UnitId = unitId, TargetX = targetX, TargetY = targetY, TargetUnitId = targetUnitId };
            lock (_toolQueueLock) _toolQueue.Enqueue(pending);
            pending.Done.Wait(TimeSpan.FromSeconds(60));
            var result = pending.Result ?? new ToolResult { success = false, error = "Timeout" };
            return WrapToolResult(result);
        }

        private static object WrapToolResult(ToolResult r)
        {
            var text = JsonUtility.ToJson(r);
            return new McpToolCallResult { content = new[] { new McpContent { type = "text", text = text } } };
        }

        private void Update()
        {
            ProcessCompletionQueue();

            PendingToolCall pending = null;
            lock (_toolQueueLock)
            {
                if (_toolQueue.Count > 0) pending = _toolQueue.Dequeue();
            }
            if (pending == null) return;

            try { ExecuteToolOnMainThread(pending); }
            catch (Exception ex)
            {
                pending.Result = new ToolResult { success = false, error = ex.Message };
                pending.Done.Set();
            }
        }

        private void ProcessCompletionQueue()
        {
            while (true)
            {
                AsyncCompletion item;
                lock (_completionQueueLock)
                {
                    if (_completionQueue.Count == 0) break;
                    item = _completionQueue.Dequeue();
                }

                GridController gc;
                lock (_contextLock) { gc = _gridController; }

                string postActionSummary = null;
                if (gc != null && item.Success)
                {
                    var playable = gc.TurnContext.PlayableUnits?.Invoke()?.ToList() ?? new List<IUnit>();
                    var worldState = BuildWorldState(playable, gc);
                    string prevTurn = null, actionsSoFar = null;
                    lock (_recentOutcomes)
                    {
                        if (!string.IsNullOrEmpty(_lastTurnSummary)) prevTurn = _lastTurnSummary;
                        if (_recentOutcomes.Length > 0) actionsSoFar = _recentOutcomes.ToString();
                    }
                    postActionSummary = BuildStrategicSummary(worldState, prevTurn, actionsSoFar);
                }

                item.Pending.Result = new ToolResult
                {
                    success = item.Success,
                    message = item.Success ? item.Message : (item.Message ?? "Action failed"),
                    error = item.Success ? null : item.Message,
                    strategic_summary = postActionSummary
                };
                item.Pending.Done.Set();
            }
        }

        private void ExecuteToolOnMainThread(PendingToolCall pending)
        {
            GridController gc;
            List<IUnit> playable;
            lock (_contextLock)
            {
                gc = _gridController;
                playable = _playableUnits != null ? new List<IUnit>(_playableUnits) : new List<IUnit>();
            }

            if (gc == null)
            {
                pending.Result = new ToolResult { success = false, error = "No turn context" };
                pending.Done.Set();
                return;
            }

            if (pending.Name == "get_world_state")
            {
                pending.Result = ExecuteGetWorldState(gc, playable);
                pending.Done.Set();
                return;
            }

            if (pending.Name == "move_unit")
            {
                ExecuteMoveUnit(pending, gc, playable);
                return;
            }

            if (pending.Name == "attack_unit")
            {
                ExecuteAttackUnit(pending, gc, playable);
                return;
            }

            if (pending.Name == "end_turn")
            {
                lock (_recentOutcomes)
                {
                    if (_recentOutcomes.Length > 0)
                    {
                        _lastTurnSummary = _recentOutcomes.ToString();
                        _recentOutcomes.Clear();
                    }
                }
                gc.EndTurn();
                ClearTurnContext();
                pending.Result = new ToolResult { success = true, message = "End turn." };
                pending.Done.Set();
                return;
            }

            pending.Result = new ToolResult { success = false, error = "Unknown tool: " + pending.Name };
            pending.Done.Set();
        }

        private ToolResult ExecuteGetWorldState(GridController gc, List<IUnit> playable)
        {
            var worldState = BuildWorldState(playable, gc);
            var worldJson = JsonUtility.ToJson(worldState);
            string prevTurn = null, actionsSoFar = null;
            lock (_recentOutcomes)
            {
                if (!string.IsNullOrEmpty(_lastTurnSummary)) prevTurn = _lastTurnSummary;
                if (_recentOutcomes.Length > 0) actionsSoFar = _recentOutcomes.ToString();
            }
            var summary = BuildStrategicSummary(worldState, prevTurn, actionsSoFar);
            return new ToolResult { success = true, strategic_summary = summary, world_state_json = worldJson };
        }

        private void ExecuteMoveUnit(PendingToolCall pending, GridController gc, List<IUnit> playable)
        {
            var unitId = pending.UnitId;
            var targetX = pending.TargetX;
            var targetY = pending.TargetY;
            var unit = playable.FirstOrDefault(u => u.UnitID == unitId);
            if (unit == null)
            {
                pending.Result = new ToolResult { success = false, error = "Unit " + unitId + " not playable" };
                pending.Done.Set();
                return;
            }

            var dest = gc.CellManager.GetCellAt(new Vector2IntImpl(targetX, targetY));
            if (dest == null)
            {
                pending.Result = new ToolResult { success = false, error = "Cell (" + targetX + "," + targetY + ") not found" };
                pending.Done.Set();
                return;
            }
            if (!unit.IsCellMovableTo(dest))
            {
                pending.Result = new ToolResult { success = false, error = "Cell not movable for unit " + unitId };
                pending.Done.Set();
                return;
            }

            if (dest.Equals(unit.CurrentCell))
            {
                pending.Result = new ToolResult { success = true, message = "Already at destination" };
                pending.Done.Set();
                return;
            }

            unit.CachePaths(gc.CellManager);
            var path = unit.FindPath(dest, gc.CellManager);
            if (path == null || path.Count == 0)
            {
                pending.Result = new ToolResult { success = false, error = "No path" };
                pending.Done.Set();
                return;
            }

            var available = unit.GetAvailableDestinations(path);
            ICell reachable = null;
            for (var i = path.Count - 1; i >= 0; i--)
            {
                if (available != null && available.Contains(path[i])) { reachable = path[i]; break; }
            }
            if (reachable == null)
            {
                pending.Result = new ToolResult { success = false, error = "No reachable cell" };
                pending.Done.Set();
                return;
            }

            var fullPath = path.TakeWhile(c => !c.Equals(reachable)).Concat(new[] { reachable }).ToList();
            var tcs = new TaskCompletionSource<bool>();
            var reachableCell = reachable;
            unit.AIExecuteAbility(new MoveCommand(unit.CurrentCell, reachable, fullPath), gc, tcs);
            tcs.Task.ContinueWith(t =>
            {
                var ok = t.Status == TaskStatus.RanToCompletion && t.Result;
                var msg = ok ? "Unit " + unitId + " moved to (" + reachableCell.GridCoordinates.x + "," + reachableCell.GridCoordinates.y + ")" : "Move execution failed or timed out";
                if (ok) lock (_recentOutcomes) { if (_recentOutcomes.Length > 0) _recentOutcomes.Append(" "); _recentOutcomes.Append("Unit " + unitId + "→(" + reachableCell.GridCoordinates.x + "," + reachableCell.GridCoordinates.y + ")"); }
                lock (_completionQueueLock) _completionQueue.Enqueue(new AsyncCompletion { Pending = pending, Success = ok, Message = msg });
            }, TaskScheduler.FromCurrentSynchronizationContext() ?? TaskScheduler.Current);
        }

        private void ExecuteAttackUnit(PendingToolCall pending, GridController gc, List<IUnit> playable)
        {
            var unitId = pending.UnitId;
            var targetUnitId = pending.TargetUnitId;
            var unit = playable.FirstOrDefault(u => u.UnitID == unitId);
            if (unit == null)
            {
                pending.Result = new ToolResult { success = false, error = "Unit " + unitId + " not playable" };
                pending.Done.Set();
                return;
            }
            if (unit.ActionPoints <= 0)
            {
                pending.Result = new ToolResult { success = false, error = "No action points" };
                pending.Done.Set();
                return;
            }

            var enemies = gc.UnitManager.GetEnemyUnits(unit.PlayerNumber).ToList();
            var target = enemies.FirstOrDefault(u => u.UnitID == targetUnitId);
            if (target == null)
            {
                pending.Result = new ToolResult { success = false, error = "Target " + targetUnitId + " not found" };
                pending.Done.Set();
                return;
            }
            if (!unit.IsUnitAttackable(target, target.CurrentCell, unit.CurrentCell))
            {
                pending.Result = new ToolResult { success = false, error = "Target not in range" };
                pending.Done.Set();
                return;
            }

            var damage = unit.CalculateTotalDamage(target);
            var tcs = new TaskCompletionSource<bool>();
            unit.AIExecuteAbility(new AttackCommand(target, damage), gc, tcs);
            tcs.Task.ContinueWith(t =>
            {
                var ok = t.Status == TaskStatus.RanToCompletion && t.Result;
                var msg = ok ? "Attacked unit " + targetUnitId : "Attack failed";
                if (ok) lock (_recentOutcomes) { if (_recentOutcomes.Length > 0) _recentOutcomes.Append(" "); _recentOutcomes.Append("Unit " + unitId + " attacked " + targetUnitId); }
                lock (_completionQueueLock) _completionQueue.Enqueue(new AsyncCompletion { Pending = pending, Success = ok, Message = msg });
            }, TaskScheduler.FromCurrentSynchronizationContext() ?? TaskScheduler.Current);
        }

        private static WorldStateData BuildWorldState(List<IUnit> playable, GridController gc)
        {
            var all = gc.UnitManager.GetUnits().ToList();
            var primary = playable?.Count > 0 ? playable[0] : null;
            if (primary == null) return new WorldStateData();

            var friendly = all.Where(u => u.PlayerNumber == primary.PlayerNumber).Select(ToUnitData).ToArray();
            var enemy = all.Where(u => u.PlayerNumber != primary.PlayerNumber).Select(ToUnitData).ToArray();
            var options = (playable ?? new List<IUnit>()).Select(u =>
            {
                var cells = gc.CellManager.GetCells().Where(c => u.IsCellMovableTo(c) || c.Equals(u.CurrentCell)).Select(ToCellData).ToArray();
                var attackIds = gc.UnitManager.GetEnemyUnits(u.PlayerNumber).Where(e => u.IsUnitAttackable(e, e.CurrentCell, u.CurrentCell)).Select(e => e.UnitID).ToArray();
                return new UnitOptionData
                {
                    unit_id = u.UnitID,
                    unit_name = GetUnitDisplayName(u),
                    abilities = GetUnitAbilityNames(u),
                    movable_cells = cells,
                    attackable_unit_ids = attackIds
                };
            }).ToArray();

            return new WorldStateData { playable_unit_ids = playable.Select(u => u.UnitID).ToArray(), unit_options = options, friendly_units = friendly, enemy_units = enemy };
        }

        private static string BuildStrategicSummary(WorldStateData w, string previousTurnSummary = null, string actionsSoFarThisTurn = null)
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== STRATEGIC SUMMARY ===");
            if (!string.IsNullOrEmpty(previousTurnSummary))
                sb.AppendLine("PREVIOUS TURN (learn from this): " + previousTurnSummary);
            if (!string.IsNullOrEmpty(actionsSoFarThisTurn))
                sb.AppendLine("ACTIONS SO FAR THIS TURN: " + actionsSoFarThisTurn);
            sb.AppendLine();
            sb.AppendLine("ENEMIES:");
            if (w.enemy_units != null)
                foreach (var e in w.enemy_units)
                {
                    var namePart = !string.IsNullOrEmpty(e.unit_name) ? " (" + e.unit_name + ")" : "";
                    var abPart = e.abilities != null && e.abilities.Length > 0 ? " | abilities: " + string.Join(", ", e.abilities) : "";
                    sb.AppendLine("  - Unit " + e.unit_id + namePart + ": at (" + e.cell.x + "," + e.cell.y + "), HP " + e.health + "/" + e.max_health + abPart);
                }
            sb.AppendLine();
            var n = w.unit_options?.Length ?? 0;
            sb.AppendLine("YOUR UNITS (" + n + " total — execute up to 2–3 actions per get_world_state; use POST_ACTION_STATE from move/attack to decide next; do not skip any unit):");
            if (w.unit_options != null)
                foreach (var opt in w.unit_options)
                {
                    var f = w.friendly_units?.FirstOrDefault(x => x.unit_id == opt.unit_id);
                    var pos = f?.cell != null ? "at (" + f.cell.x + "," + f.cell.y + ")" : "?";
                    var hp = f != null ? ", HP " + f.health + "/" + f.max_health : "";
                    var namePart = !string.IsNullOrEmpty(opt.unit_name) ? " (" + opt.unit_name + ")" : "";
                    var abPart = opt.abilities != null && opt.abilities.Length > 0 ? " | abilities: " + string.Join(", ", opt.abilities) : "";
                    sb.Append("  - Unit " + opt.unit_id + namePart + ": " + pos + hp + abPart);
                    sb.Append(" | Can move to: ");
                    sb.Append(opt.movable_cells != null && opt.movable_cells.Length > 0 ? string.Join(", ", opt.movable_cells.Select(c => "(" + c.x + "," + c.y + ")")) : "(none)");
                    sb.Append(" | Can attack: ");
                    sb.Append(opt.attackable_unit_ids != null && opt.attackable_unit_ids.Length > 0 ? string.Join(", ", opt.attackable_unit_ids.Select(id => "unit " + id)) : "(none)");
                    sb.AppendLine();
                }
            if (n == 0)
                sb.AppendLine(">>> NO UNITS LEFT TO ACT. Call end_turn NOW. <<<");
            else
                sb.AppendLine(">>> When ACTIONS SO FAR has move+attack for every unit above, call end_turn. Do not forget end_turn or the enemy will never move. <<<");
            sb.AppendLine("=== END SUMMARY ===");
            return sb.ToString();
        }

        private static string GetUnitDisplayName(IUnit u)
        {
            if (u is INamedUnit named && !string.IsNullOrEmpty(named.UnitName)) return named.UnitName;
            return u?.GetType().Name ?? "Unit";
        }

        private static string CleanAbilityName(string typeName)
        {
            if (string.IsNullOrEmpty(typeName)) return typeName;
            if (typeName.EndsWith("Ability", StringComparison.OrdinalIgnoreCase) && typeName.Length > 7)
                return typeName.Substring(0, typeName.Length - 7);
            return typeName;
        }

        private static string[] GetUnitAbilityNames(IUnit u)
        {
            var list = u?.GetBaseAbilities();
            if (list == null) return Array.Empty<string>();
            return list.Select(a => CleanAbilityName(a?.GetType().Name ?? "Unknown")).Where(s => !string.IsNullOrEmpty(s)).ToArray();
        }

        private static UnitData ToUnitData(IUnit u)
        {
            return new UnitData
            {
                unit_id = u.UnitID,
                player_number = u.PlayerNumber,
                unit_name = GetUnitDisplayName(u),
                abilities = GetUnitAbilityNames(u),
                health = u.Health,
                max_health = u.MaxHealth,
                cell = new CoordData { x = u.CurrentCell.GridCoordinates.x, y = u.CurrentCell.GridCoordinates.y }
            };
        }

        private static CellData ToCellData(ICell c)
        {
            return new CellData { x = c.GridCoordinates.x, y = c.GridCoordinates.y };
        }

        private static string EscapeJson(string s)
        {
            return (s ?? "").Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r");
        }

        private void OnDestroy() { StopServer(); }

        [Serializable] private class McpInitResult { public string protocolVersion; public McpServerInfo serverInfo; public McpCapabilities capabilities; }
        [Serializable] private class McpServerInfo { public string name; public string version; }
        [Serializable] private class McpCapabilities { }
        [Serializable] private class McpError { public int code; public string message; }
        [Serializable] private class McpToolsListResult { public McpTool[] tools; }
        [Serializable] private class McpTool { public string name; public string description; public McpSchema inputSchema; }
        [Serializable] private class McpSchema { public string type; public string[] required; public McpProperties properties; }
        [Serializable] private class McpProperties { public McpProp unit_id; public McpProp target_x; public McpProp target_y; public McpProp target_unit_id; }
        [Serializable] private class McpProp { public string type; }
        [Serializable] private class McpToolCallResult { public McpContent[] content; }
        [Serializable] private class McpContent { public string type; public string text; }
        [Serializable] private class ToolResult { public bool success; public string error; public string message; public string strategic_summary; public string world_state_json; }
        [Serializable] private class WorldStateData { public int[] playable_unit_ids; public UnitOptionData[] unit_options; public UnitData[] friendly_units; public UnitData[] enemy_units; }
        [Serializable] private class UnitOptionData { public int unit_id; public string unit_name; public string[] abilities; public CellData[] movable_cells; public int[] attackable_unit_ids; }
        [Serializable] private class UnitData { public int unit_id; public int player_number; public string unit_name; public string[] abilities; public float health; public float max_health; public CoordData cell; }
        [Serializable] private class CoordData { public int x; public int y; }
        [Serializable] private class CellData { public int x; public int y; }

        private class PendingToolCall
        {
            public string Name;
            public int UnitId, TargetX, TargetY, TargetUnitId;
            public ToolResult Result;
            public readonly ManualResetEventSlim Done = new ManualResetEventSlim(false);
        }
    }
}
