# Unity MCP + RocketRide capabilities

Unity tactical-battle sample that shows **Unity MCP** talking to a **RocketRide** pipeline via the bundled [`TBS-mcp.pipe`](Assets/StreamingAssets/pipelines/TBS-mcp.pipe) definition.

<p align="center">
  <a href="https://www.instagram.com/p/DV4ZEQujEKr/" title="Demo reel on Instagram"><img src="https://img.shields.io/badge/Instagram-Watch%20demo%20reel-E4405F?style=for-the-badge&logo=instagram&logoColor=white" alt="Watch demo on Instagram"></a>
  &nbsp;
  <a href="https://www.instagram.com/dsapandora/" title="Instagram @dsapandora"><img src="https://img.shields.io/badge/Instagram-%40dsapandora-E4405F?style=for-the-badge&logo=instagram&logoColor=white" alt="Instagram @dsapandora"></a>
  &nbsp;
  <a href="https://x.com/dsapandora" title="X @dsapandora"><img src="https://img.shields.io/badge/X-%40dsapandora-000000?style=for-the-badge&logo=x&logoColor=white" alt="X @dsapandora"></a>
  &nbsp;
  <a href="https://github.com/dsapandora" title="GitHub @dsapandora"><img src="https://img.shields.io/badge/GitHub-dsapandora-181717?style=for-the-badge&logo=github&logoColor=white" alt="GitHub @dsapandora"></a>
</p>

<p align="center">
  <a href="https://www.instagram.com/p/DV4ZEQujEKr/" title="Watch the demo reel on Instagram">
    <img src="docs/social-demo-banner.png" alt="Demo banner — opens Instagram to watch the video" width="48%">
  </a>
  &nbsp;
  <a href="https://www.instagram.com/p/DV4ZEQujEKr/" title="Watch the demo reel on Instagram">
    <img src="docs/demo-unity-mcp-rockeride-preview.jpg" alt="Demo frame — opens Instagram to watch the video" width="48%">
  </a>
</p>

<p align="center">
  <strong>Demo video:</strong> <strong>both images</strong> link to the <a href="https://www.instagram.com/p/DV4ZEQujEKr/">same Instagram reel</a> (Unity MCP, RocketRide, and Claude), where the video <strong>plays in the app or on instagram.com</strong>. GitHub cannot embed Instagram or reliably play video in the README.
</p>

**Author:** Ariel Vernaza ([@dsapandora](https://github.com/dsapandora)) — [ariel@lazyracoon.tech](mailto:ariel@lazyracoon.tech)

In this **prototype** the LLM (e.g. Claude) is called over a **remote API**: perceived latency is not only “inference time” but also **network, TLS, provider queuing, and token streaming**. With a **local** model (e.g. Ollama on loopback), the network segment shrinks almost to zero and the **machine GPU** usually becomes the bottleneck.

A **round-trip** decomposition for one tool invocation (MCP + pipeline + LLM) is:

$$
T_{\mathrm{RTT}} = T_{\mathrm{req}} + T_{\mathrm{srv}} + T_{\mathrm{LLM}} + T_{\mathrm{MCP}} + T_{\mathrm{resp}}.
$$

Here $T_{\mathrm{req}}$ and $T_{\mathrm{resp}}$ are HTTP request/response (remote paths include **WAN**); $T_{\mathrm{srv}}$ is pipeline orchestration; $T_{\mathrm{LLM}}$ bundles provider queue + cloud generation + streamed tokens back; $T_{\mathrm{MCP}}$ is MCP framing plus Unity.

Splitting what is attributable to **the wide-area path** versus a **local** deployment:

$$
T_{\mathrm{net}}^{\mathrm{WAN}} := T_{\mathrm{req}} + T_{\mathrm{resp}} + T_{\mathrm{TLS}} + T_{\mathrm{edge}}, \qquad
T_{\mathrm{net}}^{\mathrm{local}} \approx T_{\mathrm{loop}} \ll T_{\mathrm{net}}^{\mathrm{WAN}}.
$$

$$
T_{\mathrm{RTT}}^{\mathrm{remote}} = T_{\mathrm{net}}^{\mathrm{WAN}} + T_{\mathrm{srv}} + T_{\mathrm{LLM}}^{\mathrm{cloud}} + T_{\mathrm{MCP}}, \qquad
T_{\mathrm{RTT}}^{\mathrm{local}} = T_{\mathrm{net}}^{\mathrm{local}} + T_{\mathrm{srv}} + T_{\mathrm{LLM}}^{\mathrm{local}} + T_{\mathrm{MCP}}.
$$

The approximate **latency gain** when colocating the model with the game (same token load, comparable hardware) is

$$
\Delta = T_{\mathrm{RTT}}^{\mathrm{remote}} - T_{\mathrm{RTT}}^{\mathrm{local}}
\approx \bigl(T_{\mathrm{net}}^{\mathrm{WAN}} - T_{\mathrm{net}}^{\mathrm{local}}\bigr)
+ \bigl(T_{\mathrm{LLM}}^{\mathrm{cloud}} - T_{\mathrm{LLM}}^{\mathrm{local}}\bigr).
$$

## End-to-end workflow

High-level path from a chat/webhook request to a move in Unity and back to the client.

```mermaid
flowchart LR
  WH["HTTP POST\n→ Webhook :5565"]

  subgraph RR["RocketRide server"]
    direction TB
    PR["Parse + Question"]
    EM["Embedding\n(miniLM)"]
    AG["agent_rocketride"]
    LLM["llm_anthropic\n(Claude)"]
    MEM["memory_internal"]
    RES["response_answers"]
    WH --> PR --> EM
    WH --> AG
    EM --> AG
    AG --> LLM
    AG --> MEM
    AG --> RES
  end

  subgraph UnitySide["Unity + MCP"]
    MCP["MCP server\n(streamable-http)"]
    GAME["TBS scene\n(Level1)"]
    MCP <--> GAME
  end

  OUT["HTTP response\n(answers lane)"]

  AG -->|"mcp_client"| MCP
  MCP -->|"tool results"| AG
  RES --> OUT
```

1. A client sends a payload to the **Webhook** source (port `5565` in the pipe).
2. **Parse** and **Question** shape the user text; **Embedding** enriches the **questions** lane for the agent.
3. **agent_rocketride** plans tool calls using **Claude** (**llm_anthropic**) and optional **memory_internal**.
4. **mcp_client** talks to the **Unity MCP** server (`http://localhost:8765/mcp` in the sample), which reads/writes the tactical battle state.
5. **response_answers** returns the pipeline output on the **answers** lane to the caller.

## RocketRide pipeline (`TBS-mcp.pipe`)

Topology of the nodes wired in [`Assets/StreamingAssets/pipelines/TBS-mcp.pipe`](Assets/StreamingAssets/pipelines/TBS-mcp.pipe) (data lanes `input` / control `control`).

```mermaid
flowchart TB
  webhook_1["webhook_1\n(Source :5565)"]
  parse_1["parse_1"]
  question_1["question_1"]
  embedding_q_1["embedding_q_1\n(miniLM)"]
  agent_1["agent_rocketride_1"]
  mcp_1["mcp_client_1\n→ Unity MCP :8765"]
  mem_1["memory_internal_1"]
  llm_1["llm_anthropic_1\n(Claude)"]
  resp_1["response_answers_1"]

  webhook_1 -->|"tags"| parse_1
  parse_1 -->|"text"| question_1
  question_1 -->|"questions"| embedding_q_1
  embedding_q_1 -->|"questions"| agent_1
  webhook_1 -->|"questions"| agent_1
  agent_1 -->|"tool"| mcp_1
  agent_1 -->|"tool"| mem_1
  agent_1 -->|"llm"| llm_1
  agent_1 -->|"answers"| resp_1
```

### Pipeline schematic (visual overview)

![TBS-mcp pipeline schematic](docs/tbs-mcp-pipeline-schematic.png)

## What is included

- Minimal Unity project folders required to run the sample (`Assets`, `Packages`, `ProjectSettings`).
- One gameplay scene for this sample: `Assets/TBSFramework/Examples/ClashOfHeroes/Scenes/Level1.unity`.
- RocketRide pipeline: `Assets/StreamingAssets/pipelines/TBS-mcp.pipe`.

## Run notes

1. Open this folder as a Unity project.
2. Ensure your RocketRide server is reachable.
3. Configure env vars (see `.env.example`) before starting the pipeline.
