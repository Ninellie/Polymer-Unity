#nullable enable
using System;
using System.Collections.Generic;

namespace Polymer.UI.Routing
{
    public class RouteBuilder
    {
        private readonly List<(string pattern, string prefabPath, List<NavArgument> args)> _routes = new();

        public RouteBuilder Add(string pattern, string prefabPath, Action<ArgumentBuilder>? argsBuilder = null)
        {
            var builder = new ArgumentBuilder();
            argsBuilder?.Invoke(builder);
            _routes.Add((pattern, prefabPath, builder.Build()));
            return this;
        }

        public void Build(RouteTable table)
        {
            foreach (var (pattern, prefabPath, args) in _routes)
            {
                table.Register(pattern, prefabPath, args);
            }
        }
    }
}