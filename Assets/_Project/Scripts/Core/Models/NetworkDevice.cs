using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Models
{
    /// <summary>
    /// Модель сетевого устройства
    /// </summary>
    [Serializable]
    public class NetworkDevice
    {
        public int Id;
        public string Name;
        public int DeviceModelId;
        public string SerialNumber;
        public string Status = "Active";
        public Vector3 Position;
        public int LocationId;
        public List<NetworkPort> Ports = new List<NetworkPort>();
        public DateTime CreatedAt = DateTime.Now;
        public DateTime UpdatedAt = DateTime.Now;
    }
}
