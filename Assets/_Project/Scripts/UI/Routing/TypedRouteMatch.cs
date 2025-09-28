using System.Collections.Generic;

namespace Polymer.UI.Routing
{
    public class TypedRouteMatch
    {
        public string PrefabPath { get; }
        public Dictionary<string, object> TypedParameters { get; }

        public TypedRouteMatch(string prefabPath, Dictionary<string, object> typedParameters)
        {
            PrefabPath = prefabPath;
            TypedParameters = typedParameters;
        }

        public T Get<T>(string key)
        {
            return (T)TypedParameters[key];
        }
    }
}