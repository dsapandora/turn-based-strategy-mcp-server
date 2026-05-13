#!/bin/bash
# Test TBSMcpServer manually. Start Unity with a game, ensure TBSMcpServer is running, then run:
#   ./test-mcp-server.sh

ENDPOINT="${1:-http://localhost:8765/mcp}"

echo "=== Testing MCP server at $ENDPOINT ==="

# 1. tools/list
echo ""
echo "1. tools/list"
curl -s -X POST "$ENDPOINT" \
  -H "Content-Type: application/json" \
  -d '{"jsonrpc":"2.0","id":1,"method":"tools/list"}' | python3 -m json.tool

# 2. tools/call get_world_state (requires Unity game with turn context)
echo ""
echo "2. tools/call get_world_state"
curl -s -X POST "$ENDPOINT" \
  -H "Content-Type: application/json" \
  -d '{"jsonrpc":"2.0","id":2,"method":"tools/call","params":{"name":"get_world_state","arguments":{}}}' | python3 -m json.tool
