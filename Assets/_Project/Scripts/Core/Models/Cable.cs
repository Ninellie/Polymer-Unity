using System;
using UnityEngine;

namespace Core.Models
{
    /// <summary>
    /// Модель кабельного соединения между устройствами
    /// </summary>
    [Serializable]
    public class Cable
    {
        public int Id;
        public int FromDeviceId;
        public int FromPortId;
        public int ToDeviceId;
        public int ToPortId;
        public string CableType;
        public Color CableColor;
    }
}
