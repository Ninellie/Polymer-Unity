using System;

namespace Core.Models
{
    /// <summary>
    /// Модель сетевого порта устройства
    /// </summary>
    [Serializable]
    public class NetworkPort
    {
        public int Id;
        public int DeviceId;
        public string Name;
        public string Type; // Ethernet, Fiber, etc.
        public bool IsConnected;
        public int ConnectedToDeviceId;
        public int ConnectedToPortId;
        public string Status = "Available";
    }
}
