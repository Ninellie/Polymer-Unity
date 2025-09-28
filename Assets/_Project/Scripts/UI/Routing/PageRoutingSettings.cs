using UnityEngine;

namespace Polymer.UI.Routing
{
    public class PageRoutingSettings : ScriptableObject
    {
        [SerializeField] private string initialPagePath;
        public string InitialPagePath => initialPagePath;
    }
}