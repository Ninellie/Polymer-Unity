using UnityEngine;

namespace UI.DevicePage
{
    public class NetworkLoader : MonoBehaviour
    {
        public TextAsset jsonFile;
        public NetworkModel Model { get; private set; }

    }
}