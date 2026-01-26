using System;

namespace Core.Models
{
    /// <summary>
    /// Модель устройства с предустановленными параметрами
    /// </summary>
    [Serializable]
    public class Device
    {
        public int Id;
        public string Name;
        public string Description;
        public DeviceRole Role;
        public Manufacturer Manufacturer;
    }
}
