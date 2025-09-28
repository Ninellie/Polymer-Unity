using System.Collections.Generic;

namespace Polymer.UI.Routing
{
    public class RouteEntry
    {
        public string Pattern { get; }
        public string PagePrefabPath { get; }
        public List<NavArgument> Arguments { get; }

        public RouteEntry(string pattern, string prefabPath, List<NavArgument> arguments)
        {
            Pattern = pattern;
            PagePrefabPath = prefabPath;
            Arguments = arguments;
        }
    }
}