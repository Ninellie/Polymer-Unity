using System;
using UnityEngine;

namespace Core.Models
{
    /// <summary>
    /// Модель кабельного соединения между устройствами
    /// </summary>
    [Serializable]
    public class NetworkCable
    {
        public int Id;
        public int FromDeviceId;
        public int FromPortId;
        public int ToDeviceId;
        public int ToPortId;
        public string CableType;
        public string Status = "Active";
        public Color CableColor = Color.blue;
    }
}
