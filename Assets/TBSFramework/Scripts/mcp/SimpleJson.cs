using System;
using System.Text;
using System.Text.RegularExpressions;

namespace TurnBasedStrategyFramework.Unity.Mcp
{
    /// <summary>
    /// Minimal JSON extraction for MCP JSON-RPC (no external dependencies).
    /// </summary>
    internal static class SimpleJson
    {
        public static string GetString(string json, string key)
        {
            if (string.IsNullOrEmpty(json)) return null;
            var pattern = "\"" + Regex.Escape(key) + "\"\\s*:\\s*\"([^\"]*)\"";
            var m = Regex.Match(json, pattern);
            return m.Success ? Unescape(m.Groups[1].Value) : null;
        }

        public static int? GetInt(string json, string key)
        {
            if (string.IsNullOrEmpty(json)) return null;
            var pattern = "\"" + Regex.Escape(key) + "\"\\s*:\\s*(-?\\d+)";
            var m = Regex.Match(json, pattern);
            if (m.Success && int.TryParse(m.Groups[1].Value, out var v))
                return v;
            return null;
        }

        public static string GetObject(string json, string key)
        {
            if (string.IsNullOrEmpty(json)) return null;
            var keyPattern = "\"" + Regex.Escape(key) + "\"\\s*:\\s*";
            var m = Regex.Match(json, keyPattern);
            if (!m.Success) return null;
            var idx = json.IndexOf('{', m.Index);
            if (idx < 0) return null;
            var depth = 1;
            for (var i = idx + 1; i < json.Length; i++)
            {
                var c = json[i];
                if (c == '{') depth++;
                else if (c == '}') { depth--; if (depth == 0) return json.Substring(idx, i - idx + 1); }
            }
            return null;
        }

        private static string Unescape(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return s.Replace("\\\"", "\"").Replace("\\\\", "\\").Replace("\\n", "\n").Replace("\\r", "\r");
        }
    }
}
