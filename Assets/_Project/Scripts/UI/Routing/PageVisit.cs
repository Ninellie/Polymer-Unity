using System.Collections.Generic;

namespace Polymer.UI.Routing
{
    /// <summary>
    /// Хранит информацию о посещении страницы
    /// </summary>
    public class PageVisit
    {
        public string Path { get; }
        public string PrefabPath { get; }
        public Dictionary<string, object> Parameters { get; }

        public PageVisit(string path, string prefabPath, Dictionary<string, object> parameters)
        {
            Path = path;
            PrefabPath = prefabPath;
            Parameters = parameters;
        }
    }
}