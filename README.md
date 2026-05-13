# Unity MCP + RocketRide capabilities

Unity tactical-battle sample that shows **Unity MCP** talking to a **RocketRide** pipeline via the bundled `TBS-mcp` definition.

**Author:** Ariel Vernaza ([@dsapandora](https://github.com/dsapandora)) - ariel.vernaza@rocketride.ai

## Demo video

Walkthrough: Unity MCP server, RocketRide server, and Anthropic (Sonnet) using the RocketRide workflow node.

<video src="docs/demo-unity-mcp-rockeride.mp4" controls width="720"></video>

If the preview does not play in your viewer, open the file directly: [`docs/demo-unity-mcp-rockeride.mp4`](docs/demo-unity-mcp-rockeride.mp4).

## What is included

- Minimal Unity project folders required to run the sample (`Assets`, `Packages`, `ProjectSettings`).
- One gameplay scene for this sample: `Assets/TBSFramework/Examples/ClashOfHeroes/Scenes/Level1.unity`.
- RocketRide pipeline: `Assets/StreamingAssets/pipelines/TBS-mcp.pipe`.

## Security

- No hardcoded API keys are included.
- The pipeline uses `${ROCKETRIDE_ANTHROPIC_APIKEY}` instead of a raw token.

## Run notes

1. Open this folder as a Unity project.
2. Ensure your RocketRide server is reachable.
3. Configure env vars (see `.env.example`) before starting the pipeline.
