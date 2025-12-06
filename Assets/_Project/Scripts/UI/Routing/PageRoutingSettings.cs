using UnityEngine;

namespace Polymer.UI.Routing
{
    [CreateAssetMenu]
    public class PageRoutingSettings : ScriptableObject
    {
        [SerializeField] public string initialPagePath;
    }
}