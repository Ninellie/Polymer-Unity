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

        /// <summary>NetBox <c>display</c> or other UI label when it differs from <see cref="Name"/>.</summary>
        public string Display;

        public string Serial;
        public string AssetTag;
        public string Model;
        public string DeviceTypeDisplay;
        public string Status;
        public string SiteName;
        public string LocationName;
        public string RackName;
        public string RackPosition;
        public string PrimaryIp4;
        public string PrimaryIp6;
    }
}
