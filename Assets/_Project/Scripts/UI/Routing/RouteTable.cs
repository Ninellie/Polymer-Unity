#nullable enable
using System;
using System.Collections.Generic;

namespace Polymer.UI.Routing
{
    public class RouteTable
    {
        private readonly List<RouteEntry> _routes = new();

        public void Register(string pattern, string prefabPath, List<NavArgument> arguments)
        {
            _routes.Add(new RouteEntry(pattern, prefabPath, arguments));
        }

        public TypedRouteMatch? Match(string rawPath)
        {
            var (pathOnly, queryParams) = SplitPathAndQuery(rawPath);

            foreach (var route in _routes)
            {
                var routeParams = MatchRoute(route.Pattern, pathOnly);
                if (routeParams == null) continue;

                // route + query
                var allParams = new Dictionary<string, string>(routeParams);
                foreach (var kvp in queryParams)
                    allParams[kvp.Key] = kvp.Value;

                // Преобразование в типизированные параметры
                var typedParams = new Dictionary<string, object>();

                foreach (var arg in route.Arguments)
                {
                    if (!allParams.TryGetValue(arg.Name, out var raw))
                        return null;

                    try
                    {
                        var value = Convert.ChangeType(raw, arg.Type);
                        typedParams[arg.Name] = value;
                    }
                    catch
                    {
                        UnityEngine.Debug.LogError($"[RouteTable] Failed to parse parameter '{arg.Name}'" +
                                                   $" from value '{raw}' to {arg.Type.Name}");
                        return null;
                    }
                }

                return new TypedRouteMatch(route.PagePrefabPath, typedParams);
            }

            return null;
        }

        private (string path, Dictionary<string, string> queryParams) SplitPathAndQuery(string rawPath)
        {
            var queryParams = new Dictionary<string, string>();
            var pathOnly = rawPath;

            var queryIndex = rawPath.IndexOf('?');
            if (queryIndex < 0) return (pathOnly, queryParams);
            
            pathOnly = rawPath[..queryIndex];
            var queryString = rawPath[(queryIndex + 1)..];
            queryParams = ParseQueryString(queryString);

            return (pathOnly, queryParams);
        }

        private Dictionary<string, string> ParseQueryString(string query)
        {
            var dict = new Dictionary<string, string>();
            var parts = query.Split('&');
            foreach (var part in parts)
            {
                var kv = part.Split('=');
                if (kv.Length == 2)
                {
                    var key = UnityEngine.Networking.UnityWebRequest.UnEscapeURL(kv[0]);
                    var val = UnityEngine.Networking.UnityWebRequest.UnEscapeURL(kv[1]);
                    dict[key] = val;
                }
            }
            return dict;
        }

        private Dictionary<string, string>? MatchRoute(string pattern, string path)
        {
            var patternSegments = pattern.Trim('/').Split('/');
            var pathSegments = path.Trim('/').Split('/');

            if (patternSegments.Length != pathSegments.Length)
                return null;

            var result = new Dictionary<string, string>();

            for (var i = 0; i < patternSegments.Length; i++)
            {
                var p = patternSegments[i];
                var s = pathSegments[i];

                if (p.StartsWith("{") && p.EndsWith("}"))
                {
                    var key = p.Substring(1, p.Length - 2);
                    result[key] = s;
                }
                else if (p != s)
                {
                    return null;
                }
            }

            return result;
        }
    }
}