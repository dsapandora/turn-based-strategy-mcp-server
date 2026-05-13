# TBS + MCP: Unity como servidor MCP para decisiones del LLM

## Problema actual

- El LLM (Anthropic) recibe un snapshot del estado del mundo y devuelve JSON con decisiones.
- A veces inventa coordenadas en lugar de usar las de `Can move to` / `movable_cells`.
- CellManager, IsCellMovableTo, GetEnemyUnits, etc. ya validan; ese conocimiento solo está en Unity.

## Enfoque MCP (Model Context Protocol)

Rocket Ride incluye **mcp_client**, un nodo que:

- Se conecta a un **servidor MCP externo**
- Descubre herramientas vía `tools/list`
- Invoca herramientas vía `tools/call`
- Las expone a agentes (agent_rocketride, etc.)

Si Unity actúa como **servidor MCP**, el LLM puede llamar herramientas definidas por Unity (con esquemas y validaciones) en vez de devolver JSON libre.

## Flujo propuesto

```
Unity (MCP Server)                    Rocket Ride
─────────────────                    ───────────
  get_world_state()  ←──tools/list──  mcp_client
  move_unit(id,x,y)                    agent_rocketride
  attack_unit(id,target_id)            llm_anthropic
       ↑                                     │
       └──────── tools/call ─────────────────┘
```

1. Unity ejecuta un servidor MCP (stdio o streamable-http).
2. Rocket Ride tiene un pipeline: **chat → mcp_client (apunta a Unity) → agent_rocketride → llm_anthropic**.
3. El agente obtiene el estado con `get_world_state()`.
4. El LLM decide y llama `move_unit(unit_id, target_x, target_y)` o `attack_unit(unit_id, target_unit_id)`.
5. Unity valida con CellManager/IsCellMovableTo/GetEnemyUnits y ejecuta.
6. La respuesta de la herramienta puede incluir éxito/error y estado actualizado.

## Herramientas que Unity podría exponer

| Tool | Descripción | Params |
|------|-------------|--------|
| `get_world_state` | Devuelve estado actual (igual que BuildWorldState + BuildStrategicSummary) | - |
| `move_unit` | Mueve una unidad (solo si la casilla es válida) | unit_id, target_x, target_y |
| `attack_unit` | Ataca a un enemigo (solo si está al alcance) | unit_id, target_unit_id |
| `end_turn` | Finaliza el turno de la unidad actual | unit_id |

## Validación en Unity

Cada tool usa la lógica ya existente:

- `move_unit`: `CellManager.GetCellAt`, `unit.IsCellMovableTo`, `FindPath`, etc.
- `attack_unit`: `GetEnemyUnits`, `unit.IsUnitAttackable`, etc.

Si los parámetros son inválidos, la tool devuelve error y el LLM puede intentar otra acción.

## Cómo implementar el servidor MCP en Unity

Opciones:

1. **Process + stdio**: Unity lanza un proceso Python que expone el MCP server y se comunica con Unity vía pipe/socket.
2. **Streamable HTTP**: Un servidor HTTP en un hilo/process que responde a `/mcp`; Rocket Ride mcp_client con `transport: streamable-http` y `endpoint: http://localhost:PORT/mcp`.
3. **C# MCP server**: Implementar el protocolo MCP en C# (JSON-RPC sobre stdio o HTTP). Hay librerías como `StreamJsonRpc` o similares.

## Implementación

- **`TBSMcpServer.cs`** – Servidor MCP en Unity (HTTP, puerto 8765). Expone: `get_world_state`, `move_unit`, `attack_unit`, `end_turn`.
- **`TBS-mcp.pipe`** – Pipeline: webhook → parse → preprocessor/question → embedding → qdrant (RAG) → agent_rocketride + mcp_client (→ Unity) + llm_anthropic → response_answers.
- **HumanPlayer** – `_useMcpMode`: activa flujo MCP (crea/inicia TBSMcpServer, usa TBS-mcp.pipe, pregunta única por turno).

## RAG / Qdrant (manual táctico y memoria)

El pipeline **TBS-mcp.pipe** usa **Qdrant** para inyectar contexto útil en el agente:

1. **Consulta (cada turno)**: webhook (pregunta) → parse → question → embedding → **qdrant** (búsqueda semántica) → agent. El agente recibe la pregunta del turno y los fragmentos recuperados del manual táctico.
2. **Colección**: `tbs_tactical_manual`. Contenido: manual táctico (Knight, Rogue, Wizard, enemigos a distancia, prioridades, etc.) en `Assets/TBSFramework/Docs/TBS-Tactical-Manual.md`.

### Ingestar documentos de reglas en Qdrant (desde el pipeline)

El pipeline **TBS-mcp.pipe** incluye una rama de ingesta:

- **File dropper** (`file_drop_1`): origen de tipo file que acepta `.md` y `.txt`.
- **Parse** (`parse_docs_1`) → **Preprocessor** (`preprocessor_docs_1`, RecursiveCharacterTextSplitter, chunk ~2048) → **Embedding** (`embedding_q_1`, lane `documents`) → **Qdrant** (`qdrant_1`, lane `documents`).

Para cargar el manual táctico (u otros documentos de reglas): tener **Qdrant** en `localhost:6333` y, en Rocket Ride, usar el input de tipo file del pipeline (file dropper) para subir `TBS-Tactical-Manual.md` o cualquier `.md`/`.txt`. Al ejecutar con ese fichero, los chunks se embeben y se guardan en la colección `tbs_tactical_manual`. El mismo `embedding_transformer` (miniLM) se usa en consulta e ingesta, así que los vectores coinciden.

Si en tu versión de Rocket Ride el provider del file dropper tiene otro nombre (p. ej. `upload` o `data`), cambia `file_drop_1` en el `.pipe` al provider correcto.

### Memoria entre turnos (opcional)

Para que el agente “recuerde” jugadas anteriores, se puede ingestar en la misma colección (o en otra) resúmenes de turnos o partidas; el flujo de ingesta sería independiente (p. ej. otro pipeline o script que escriba en Qdrant cuando termine un turno).

## Configuración y orden

- **Todo lo que usa el pipeline está en el pipeline**: la API key de Anthropic (y el resto de config del LLM) va dentro de `TBS-mcp.pipe`. Unity **no** inyecta `ANTHROPIC_API_KEY`; debes poner tu clave en el pipeline que subes (sustituye `{{ANTHROPIC_API_KEY}}` por tu key o edita el campo `apikey` del nodo `llm_anthropic_1`).
- **Orden MCP / pipeline**: el pipeline puede dar error si el servidor MCP no está arriba cuando el agente llama a las tools (p. ej. `get_world_state`). El MCP arranca con Unity. En el flujo normal, Unity hace `WaitForMcpReachable` antes de enviar la pregunta al webhook, así que cuando el pipeline ejecuta, el MCP ya está disponible. Si ejecutas el pipeline a mano (p. ej. solo para ingestar un fichero por el file dropper) **sin** tener Unity en marcha, los pasos que usan `mcp_client` fallarán al conectar. Para partidas: arranca Unity (y opcionalmente Rocket Ride antes), luego juega; para solo ingestar documentos, puedes arrancar solo Rocket Ride y Qdrant y no usar las tools MCP.

## Cómo usar

1. Añadir `TBSMcpServer` a la escena (opcional; HumanPlayer lo crea si no existe).
2. Iniciar **Rocket Ride** (puerto 5565 por defecto).
3. (Opcional) Iniciar **Qdrant** en localhost:6333 si usas RAG.

## Probar el MCP server manualmente

Con Unity en ejecución y partida activa (turno del jugador humano), ejecuta:

```bash
# Listar herramientas
curl -X POST http://localhost:8765/mcp -H "Content-Type: application/json" \
  -d '{"jsonrpc":"2.0","id":1,"method":"tools/list"}'

# Llamar get_world_state
curl -X POST http://localhost:8765/mcp -H "Content-Type: application/json" \
  -d '{"jsonrpc":"2.0","id":2,"method":"tools/call","params":{"name":"get_world_state","arguments":{}}}'
```

O usa `Assets/TBSFramework/Docs/test-mcp-server.sh`.
3. Iniciar Unity. El `TBSMcpServer` escucha en `http://localhost:8765/mcp`.
4. El pipeline TBS-mcp se sube vía `execute`; el `mcp_client` de Rocket Ride debe conectarse a ese endpoint.
5. Si "connection closed" o "pipeline open failed": comprobar que Rocket Ride esté en marcha, que el puerto 8765 esté libre, y que el mcp_client pueda alcanzar `localhost:8765/mcp` (mismo host que Rocket Ride).

6. Si "MCP server not reachable after 30s": el servidor tarda en enlazar. Ahora hay 0.5s de espera tras StartServer. Si falla, verifica que 8765 no esté usado (`lsof -i :8765`).

7. Si el pipeline se cierra tras un turno: pon **Use Existing Pipeline** = false en HumanPlayer para iniciar un pipeline nuevo cada turno.

## Referencias en Rocket Ride

- `nodes/src/nodes/mcp_client/` – cliente MCP
- `nodes/src/nodes/agent_rocketride/` – agente que usa tools
- `nodes/src/nodes/mcp_client/services.json` – config transport (stdio, streamable-http, sse)
