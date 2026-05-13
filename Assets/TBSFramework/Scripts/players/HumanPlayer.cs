using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Controllers.GameResolvers;
using TurnBasedStrategyFramework.Common.Players;
using TurnBasedStrategyFramework.Common.Units;
using TurnBasedStrategyFramework.Unity.Mcp;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Players
{
    /// <summary>
    /// Human player driven by Rocket Ride agent via MCP. Unity exposes tools (get_world_state, move_unit, attack_unit, end_turn).
    /// </summary>
    public class HumanPlayer : Player
    {
        [Header("MCP / Rocket Ride")]
        [SerializeField] private bool _debugMode;
        [SerializeField] private bool _logWebSocketResponse = true;
        [SerializeField] private int _turnStartDelay = 0;
        [Tooltip("Extra delay (ms) before EndTurn to ensure movements/animations have finished.")]
        [SerializeField] private int _endTurnDelay = 300;
        [Tooltip("Max seconds to wait for pipeline to be ready (open returns pipe_id > 0).")]
        [SerializeField] private int _pipelineReadyTimeoutSeconds = 90;
        [Tooltip("Seconds between open retries while pipeline is starting.")]
        [SerializeField] private int _pipelineRetryIntervalSeconds = 3;

        [SerializeField] private string _rocketRideBaseUri = "http://localhost:5565";
        [SerializeField] private string _rocketRideApiKey = "";
        [Tooltip("Unused: Anthropic API key is set in the pipeline file (TBS-mcp.pipe).")]
        [SerializeField] private string _anthropicApiKey = "";
        [SerializeField] private string _rocketRideProjectId = "";
        [SerializeField] private string _role = "player";
        [Tooltip("If false, always start a fresh pipeline (use when pipe_id stays 0 with useExisting).")]
        [SerializeField] private bool _useExistingPipeline = true;
        [Tooltip("If true, start MCP server and Rocket Ride pipeline as soon as the game initializes (so they are ready before the first turn).")]
        [SerializeField] private bool _startMcpAndPipelineAtGameStart = true;
        [Tooltip("Seconds to wait between retries when Rocket Ride is not reachable (connection refused). Unity will keep retrying until connected.")]
        [SerializeField] private int _rocketRideConnectRetryIntervalSeconds = 3;
        [Tooltip("Max seconds to wait for Rocket Ride connection (0 = wait indefinitely).")]
        [SerializeField] private int _rocketRideConnectMaxWaitSeconds = 0;

        private static readonly object _pipelineStartLock = new object();
        private static bool _pipelineStartInProgress;
        private static readonly SemaphoreSlim _webSocketOpLock = new SemaphoreSlim(1, 1);

        private CancellationTokenSource _cancellationTokenSource;
        private ClientWebSocket _socket;
        private int _seq;
        private string _taskToken;

        public override PlayerType PlayerType { get; set; } = PlayerType.HumanPlayer;

        public override void Initialize(GridController gridController)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            gridController.GameEnded += OnGameEnded;
            gridController.TurnEnded += OnTurnEnded;
            if (_startMcpAndPipelineAtGameStart)
                StartMcpServerAndPipelineAtGameStartAsync();
        }

        /// <summary>
        /// Starts the MCP server and Rocket Ride pipeline as soon as the game loads, so they are ready before the first human turn.
        /// Only one pipeline start runs at a time to avoid concurrent ReceiveAsync on the same WebSocket.
        /// </summary>
        private async void StartMcpServerAndPipelineAtGameStartAsync()
        {
            lock (_pipelineStartLock)
            {
                if (!string.IsNullOrWhiteSpace(_taskToken)) return;
                if (_pipelineStartInProgress) return;
                _pipelineStartInProgress = true;
            }
            try
            {
                var mcpServer = FindFirstObjectByType<TBSMcpServer>();
                if (mcpServer == null)
                {
                    var go = new GameObject("TBSMcpServer");
                    mcpServer = go.AddComponent<TBSMcpServer>();
                    DontDestroyOnLoad(go);
                }
                if (!mcpServer.IsRunning)
                    mcpServer.StartServer();
                if (_logWebSocketResponse)
                    Debug.Log("[RocketRide] MCP server started at game init: " + mcpServer.Endpoint);

                await EnsureConnectedAndAuthenticated(CancellationToken.None);
                await EnsurePipelineToken(CancellationToken.None, mcpServer);
                if (_logWebSocketResponse)
                    Debug.Log("[RocketRide] Pipeline started at game init. Ready for first turn.");
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[RocketRide] Start pipeline at game init failed (will retry on first turn): " + ex.Message);
            }
            finally
            {
                lock (_pipelineStartLock) { _pipelineStartInProgress = false; }
            }
        }

        public override void Play(GridController gridController)
        {
            RunMcpTurn(gridController);
        }

        private void OnTurnEnded(TurnTransitionParams turnTransitionParams)
        {
            _cancellationTokenSource.Cancel();
        }

        private void OnGameEnded(GameResult gameResult)
        {
            _cancellationTokenSource.Cancel();
            _ = ForceDisconnect();
        }

        private async void RunMcpTurn(GridController gridController)
        {
            await Awaitable.WaitForSecondsAsync(_turnStartDelay / 1000f);
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();

            var playableUnits = gridController.TurnContext.PlayableUnits().ToList();
            if (_logWebSocketResponse)
                Debug.Log($"[RocketRide] MCP turn START — player {gridController.TurnContext.CurrentPlayer?.PlayerType}, {playableUnits.Count} playable units: [{string.Join(", ", playableUnits.Select(u => u.UnitID))}]");
            if (string.IsNullOrWhiteSpace(_rocketRideBaseUri) || string.IsNullOrWhiteSpace(_rocketRideApiKey))
                Debug.LogWarning("[RocketRide] Rocket Ride Base URI or Api Key is empty. Configure them on the HumanPlayer component in the scene so the agent can run.");
            if (playableUnits.Count == 0)
            {
                gridController.EndTurn();
                return;
            }

            var mcpServer = FindFirstObjectByType<TBSMcpServer>();
            if (mcpServer == null)
            {
                var go = new GameObject("TBSMcpServer");
                mcpServer = go.AddComponent<TBSMcpServer>();
                DontDestroyOnLoad(go);
            }

            if (!mcpServer.IsRunning)
                mcpServer.StartServer();

            await Awaitable.WaitForSecondsAsync(0.5f);

            if (_logWebSocketResponse)
                Debug.Log("[RocketRide] MCP server at " + mcpServer.Endpoint + " — Rocket Ride mcp_client must connect here. Is Rocket Ride running with TBS-mcp pipeline?");

            mcpServer.SetTurnContext(gridController, playableUnits, _rocketRideProjectId, _role);

            if (!await WaitForMcpReachable(mcpServer))
            {
                Debug.LogWarning("[RocketRide] Aborting turn: MCP server not reachable. Pipeline mcp_client cannot connect. Ensure port 8765 is free.");
                gridController.EndTurn();
                return;
            }

            try
            {
                var questionText = "Execute the turn. Call get_world_state once at the start. Each unit: move once, attack once. When all units have acted, call end_turn.";
                var payload = BuildQuestionJson("{}", "", questionText);
                await EnsureConnectedAndAuthenticated(_cancellationTokenSource.Token);
                await EnsurePipelineToken(_cancellationTokenSource.Token, mcpServer);
                var webhookOk = await PostToWebhook(payload);
                if (!webhookOk)
                {
                    Debug.LogWarning("[RocketRide] Webhook POST failed. Check Rocket Ride is running and token is valid.");
                    return;
                }
                if (_logWebSocketResponse)
                    Debug.Log("[RocketRide] Webhook POST complete — question sent.");
            }
            catch (OperationCanceledException) { return; }
            catch (Exception ex)
            {
                Debug.LogWarning("[RocketRide] MCP turn error: " + ex.Message);
                if (ex.Message.Contains("not found") || ex.Message.Contains("TBS-mcp.pipe"))
                    Debug.LogWarning("[RocketRide] In a build, the pipeline file must be in StreamingAssets. See Assets/StreamingAssets/pipelines/.");
                if (ex.Message.Contains("auth") || ex.Message.Contains("Api Key"))
                    Debug.LogWarning("[RocketRide] Set Rocket Ride Api Key on the HumanPlayer component. Put Anthropic API key in the pipeline file (TBS-mcp.pipe) if using LLM.");
            }
            // Do NOT call ClearTurnContext or gridController.EndTurn: the MCP server executes
            // the queued actions (move/attack) and calls EndTurn when the plan is complete.
        }

        private async System.Threading.Tasks.Task<bool> WaitForMcpReachable(TBSMcpServer mcpServer)
        {
            var host = "127.0.0.1";
            var port = mcpServer.Port;
            for (var i = 0; i < 30; i++)
            {
                if (await IsTcpReachable(host, port))
                {
                    if (_logWebSocketResponse)
                        Debug.Log("[RocketRide] MCP server " + mcpServer.Endpoint + " is reachable.");
                    return true;
                }
                if (_logWebSocketResponse && i < 5)
                    Debug.Log("[RocketRide] Waiting for MCP server at " + host + ":" + port + " (attempt " + (i + 1) + "/30)...");
                await Awaitable.WaitForSecondsAsync(1f);
            }
            Debug.LogWarning("[RocketRide] MCP server not reachable after 30s.");
            return false;
        }

        private static async System.Threading.Tasks.Task<bool> IsTcpReachable(string host, int port)
        {
            try
            {
                using (var client = new System.Net.Sockets.TcpClient())
                {
                    var connectTask = client.ConnectAsync(host, port);
                    var timeoutTask = System.Threading.Tasks.Task.Delay(2000);
                    if (await System.Threading.Tasks.Task.WhenAny(connectTask, timeoutTask) == connectTask && !connectTask.IsFaulted)
                        return true;
                }
            }
            catch { }
            return false;
        }

        private async Task EnsurePipelineToken(CancellationToken token, TBSMcpServer mcpServer)
        {
            if (!string.IsNullOrWhiteSpace(_taskToken)) return;
            // If game-init is already starting the pipeline, wait for it instead of starting a second one (avoids concurrent ReceiveAsync).
            var waitUntil = DateTime.UtcNow.AddSeconds(60);
            while (_pipelineStartInProgress && string.IsNullOrWhiteSpace(_taskToken) && DateTime.UtcNow < waitUntil)
            {
                token.ThrowIfCancellationRequested();
                await Awaitable.WaitForSecondsAsync(0.2f);
            }
            if (!string.IsNullOrWhiteSpace(_taskToken)) return;
            if (_socket == null || _socket.State != WebSocketState.Open)
                await EnsureConnectedAndAuthenticated(token);
            var pipelineJson = LoadPipelineConfig();
            if (string.IsNullOrWhiteSpace(pipelineJson))
                throw new InvalidOperationException("[RocketRide] TBS-mcp.pipe not found. In build use Assets/StreamingAssets/pipelines/TBS-mcp.pipe.");
            pipelineJson = pipelineJson.Replace("http://localhost:8765/mcp", mcpServer.Endpoint);
            var intervalSec = Math.Max(1, _rocketRideConnectRetryIntervalSeconds);
            for (var attempt = 1; attempt <= 5; attempt++)
            {
                token.ThrowIfCancellationRequested();
                if (_socket == null || _socket.State != WebSocketState.Open)
                    await EnsureConnectedAndAuthenticated(token);
                try
                {
                    await StartPipeline(pipelineJson, token);
                    return;
                }
                catch (Exception ex)
                {
                    if (attempt >= 5) throw;
                    if (_logWebSocketResponse)
                        Debug.Log("[RocketRide] Pipeline start failed (attempt " + attempt + "/5): " + ex.Message + ". Reconnecting and retrying in " + intervalSec + "s...");
                    try { _socket?.Dispose(); } catch { }
                    _socket = null;
                    await Awaitable.WaitForSecondsAsync(intervalSec);
                }
            }
        }

        private string LoadPipelineConfig()
        {
            var raw = LoadPipelineFromPath("pipelines/TBS-mcp.pipe");
            if (string.IsNullOrWhiteSpace(raw)) return null;
            // API key and LLM config live in the pipeline file; Unity does not inject ANTHROPIC_API_KEY.
            if (!string.IsNullOrWhiteSpace(_rocketRideProjectId))
            {
                raw = raw.TrimStart();
                if (raw.StartsWith("{"))
                    raw = "{\"project_id\":\"" + EscapeJson(_rocketRideProjectId) + "\"," + raw.Substring(1);
            }
            return raw;
        }

        private static string LoadPipelineFromPath(string relativePath)
        {
            try
            {
                // Build: pipeline must be in StreamingAssets (copied as-is). Editor: try StreamingAssets first, then Assets.
                var streamingPath = System.IO.Path.Combine(Application.streamingAssetsPath, relativePath);
                if (System.IO.File.Exists(streamingPath))
                    return System.IO.File.ReadAllText(streamingPath).Trim();
                var dataPath = System.IO.Path.Combine(Application.dataPath, relativePath);
                if (System.IO.File.Exists(dataPath))
                    return System.IO.File.ReadAllText(dataPath).Trim();
                Debug.LogWarning("[RocketRide] Pipeline file not found. Tried: " + streamingPath + " and " + dataPath + " — In build, put TBS-mcp.pipe in Assets/StreamingAssets/pipelines/.");
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[RocketRide] Failed to load pipeline: " + ex.Message);
                return null;
            }
        }

        private async Task StartPipeline(string pipelineJson, CancellationToken token)
        {
            await _webSocketOpLock.WaitAsync(token);
            try
            {
                Debug.Log("[RocketRide] Sending pipeline via execute (" + pipelineJson.Length + " chars)");
                var args = "{\"apikey\":\"" + EscapeJson(_rocketRideApiKey ?? "") + "\",\"pipeline\":" + pipelineJson + ",\"args\":[],\"useExisting\":" + (_useExistingPipeline ? "true" : "false");
                if (!string.IsNullOrWhiteSpace(_rocketRideProjectId))
                    args += ",\"projectId\":\"" + EscapeJson(_rocketRideProjectId) + "\"";
                args += ",\"source\":\"webhook_1\"}";
                var executeSeq = NextSeq();
                var executeJson = "{\"type\":\"request\",\"seq\":" + executeSeq + ",\"command\":\"execute\",\"arguments\":" + args + "}";
                await SendText(executeJson, token);
                var (response, executeRaw) = await WaitForResponseWithRaw(executeSeq, token);
            if (!response.success || response.body == null || string.IsNullOrWhiteSpace(response.body.token))
            {
                var err = response.message ?? "Rocket Ride execute failed";
                Debug.LogWarning("[RocketRide] Execute failed: " + err + (string.IsNullOrEmpty(executeRaw) ? "" : " | Raw: " + (executeRaw.Length > 600 ? executeRaw.Substring(0, 600) + "..." : executeRaw)));
                throw new InvalidOperationException(err);
            }
            _taskToken = response.body.token;
                Debug.Log("[RocketRide] Pipeline started, token obtained. mcp_client in pipeline must reach Unity MCP server (see TBSMcpServer endpoint).");
            }
            finally
            {
                _webSocketOpLock.Release();
            }
        }

        private async Task EnsureConnectedAndAuthenticated(CancellationToken token)
        {
            if (_socket != null && _socket.State == WebSocketState.Open) return;
            await _webSocketOpLock.WaitAsync(token);
            try
            {
                if (_socket != null && _socket.State == WebSocketState.Open) return;
            var wsUri = BuildTaskServiceWebSocketUri(_rocketRideBaseUri);
            var intervalSec = Math.Max(1, _rocketRideConnectRetryIntervalSeconds);
            var maxWaitSec = _rocketRideConnectMaxWaitSeconds;
            var started = DateTime.UtcNow;
            var attempt = 0;
            while (true)
            {
                token.ThrowIfCancellationRequested();
                if (maxWaitSec > 0 && (DateTime.UtcNow - started).TotalSeconds >= maxWaitSec)
                    throw new InvalidOperationException("[RocketRide] Rocket Ride not reachable after " + maxWaitSec + "s. Is the server running at " + _rocketRideBaseUri + "?");
                attempt++;
                try
                {
                    if (_socket != null)
                    {
                        try { _socket.Dispose(); } catch { }
                        _socket = null;
                    }
                    if (_logWebSocketResponse && attempt > 1)
                        Debug.Log("[RocketRide] Retry " + attempt + " connecting to " + wsUri + "...");
                    else if (_logWebSocketResponse)
                        Debug.Log("[RocketRide] Connecting to " + wsUri);
                    _socket = new ClientWebSocket();
                    await _socket.ConnectAsync(new Uri(wsUri), token);
                    var authSeq = NextSeq();
                    await SendText("{\"type\":\"request\",\"seq\":" + authSeq + ",\"command\":\"auth\",\"arguments\":{\"auth\":\"" + EscapeJson(_rocketRideApiKey) + "\"}}", token);
                    var authResponse = await WaitForResponse(authSeq, token);
                    if (!authResponse.success)
                        throw new InvalidOperationException(authResponse.message ?? "RocketRide auth failed.");
                    if (_logWebSocketResponse)
                        Debug.Log("[RocketRide] Auth OK.");
                    return;
                }
                catch (Exception ex) when (attempt > 0)
                {
                    var msg = ex.Message ?? ex.ToString();
                    if (msg.Contains("61") || msg.Contains("Connection refused") || msg.Contains("refused") || msg.Contains("Unable to connect"))
                    {
                        if (_logWebSocketResponse)
                            Debug.Log("[RocketRide] Rocket Ride not ready yet (" + msg + "). Waiting " + intervalSec + "s before retry...");
                    }
                    else
                        throw;
                }
                await Awaitable.WaitForSecondsAsync(intervalSec);
            }
            }
            finally
            {
                _webSocketOpLock.Release();
            }
        }

        private async System.Threading.Tasks.Task<bool> PostToWebhook(string payload)
        {
            var baseUri = (_rocketRideBaseUri ?? "").Trim().TrimEnd('/');
            if (string.IsNullOrEmpty(baseUri) || !baseUri.Contains("://"))
                baseUri = "http://" + baseUri;
            var url = baseUri + "/webhook?token=" + Uri.EscapeDataString(_taskToken ?? "");
            using (var req = new UnityWebRequest(url, "POST"))
            {
                var bodyBytes = Encoding.UTF8.GetBytes(payload);
                req.uploadHandler = new UploadHandlerRaw(bodyBytes);
                req.downloadHandler = new DownloadHandlerBuffer();
                req.SetRequestHeader("Content-Type", "application/rocketride-question");
                if (!string.IsNullOrEmpty(_rocketRideApiKey))
                    req.SetRequestHeader("Authorization", "Bearer " + _rocketRideApiKey);
                var op = req.SendWebRequest();
                while (!op.isDone && !_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    await Awaitable.WaitForSecondsAsync(0.05f);
                }
                if (_cancellationTokenSource.Token.IsCancellationRequested)
                    return false;
                return req.result == UnityWebRequest.Result.Success;
            }
        }

        private static string BuildQuestionJson(string worldStateJson, string strategicSummary, string questionText)
        {
            var ctx0 = EscapeJson(strategicSummary);
            var ctx1 = EscapeJson(worldStateJson);
            return "{\"expectJson\":true,\"context\":[\"" + ctx0 + "\",\"" + ctx1 + "\"],\"questions\":[{\"text\":\"" + EscapeJson(questionText) + "\"}]}";
        }

        private async Task SendText(string json, CancellationToken token)
        {
            if (_socket == null || _socket.State != WebSocketState.Open)
                throw new InvalidOperationException("RocketRide socket is not open.");
            await _socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(json)), WebSocketMessageType.Text, true, token);
        }

        private async Task SendBinary(string jsonHeader, byte[] payload, CancellationToken token)
        {
            if (_socket == null || _socket.State != WebSocketState.Open)
                throw new InvalidOperationException("RocketRide socket is not open.");
            var headerBytes = Encoding.UTF8.GetBytes(jsonHeader);
            var message = new byte[headerBytes.Length + 1 + payload.Length];
            Buffer.BlockCopy(headerBytes, 0, message, 0, headerBytes.Length);
            message[headerBytes.Length] = (byte)'\n';
            Buffer.BlockCopy(payload, 0, message, headerBytes.Length + 1, payload.Length);
            await _socket.SendAsync(new ArraySegment<byte>(message), WebSocketMessageType.Binary, true, token);
        }

        private async Task<DapResponse> WaitForResponse(int requestSeq, CancellationToken token)
        {
            var (r, _) = await WaitForResponseWithRaw(requestSeq, token);
            return r;
        }

        private async Task<(DapResponse response, string raw)> WaitForResponseWithRaw(int requestSeq, CancellationToken token)
        {
            while (true)
            {
                var raw = await ReceiveMessage(token);
                if (string.IsNullOrWhiteSpace(raw)) continue;
                var response = JsonUtility.FromJson<DapResponse>(raw);
                if (response != null && response.type == "response" && response.request_seq == requestSeq)
                    return (response, raw);
            }
        }

        private async Task<string> ReceiveMessage(CancellationToken token)
        {
            if (_socket == null || _socket.State != WebSocketState.Open)
                throw new InvalidOperationException("RocketRide socket is not open.");
            var buffer = new byte[32768];
            var offset = 0;
            WebSocketReceiveResult result;
            do
            {
                result = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer, offset, buffer.Length - offset), token);
                if (result.MessageType == WebSocketMessageType.Close)
            {
                if (_logWebSocketResponse)
                    Debug.LogWarning("[RocketRide] WebSocket connection closed by server. Check Rocket Ride logs.");
                await ForceDisconnect();
                return "";
            }
                offset += result.Count;
            } while (!result.EndOfMessage);
            if (result.MessageType == WebSocketMessageType.Text)
                return Encoding.UTF8.GetString(buffer, 0, offset);
            var idx = Array.IndexOf(buffer, (byte)'\n', 0, offset);
            return idx > 0 ? Encoding.UTF8.GetString(buffer, 0, idx) : Encoding.UTF8.GetString(buffer, 0, offset);
        }

        private async Task ForceDisconnect()
        {
            try
            {
                if (_socket != null)
                {
                    if (_socket.State == WebSocketState.Open)
                        await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                    _socket.Dispose();
                }
            }
            catch { }
            finally { _socket = null; _taskToken = null; }
        }

        private int NextSeq() { _seq++; return _seq; }

        private static string BuildTaskServiceWebSocketUri(string baseUri)
        {
            var normalized = (baseUri ?? "").Trim();
            if (!normalized.Contains("://")) normalized = "http://" + normalized;
            if (!Uri.TryCreate(normalized, UriKind.Absolute, out var uri))
                throw new InvalidOperationException("Invalid RocketRide base URI.");
            var builder = new UriBuilder(uri) { Scheme = uri.Scheme == "https" ? "wss" : "ws", Port = uri.IsDefaultPort ? 5565 : uri.Port };
            builder.Path = builder.Path.TrimEnd('/') + "/task/service";
            return builder.Uri.ToString();
        }

        private static string EscapeJson(string value)
        {
            return (value ?? "").Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r");
        }

        [Serializable] private class DapResponse { public string type; public int request_seq; public bool success; public string message; public DapBody body; }
        [Serializable] private class DapBody { public string token; public int pipe_id; public string[] answers; public string[] text; }
    }
}
