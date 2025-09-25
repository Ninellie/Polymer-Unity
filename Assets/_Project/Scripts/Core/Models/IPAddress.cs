using System;

namespace Core.Models
{
    /// <summary>
    /// Модель IP адреса
    /// </summary>
    [Serializable]
    public class IPAddress
    {
        public int Id;
        public string Address;
        public string SubnetMask;
        public int DeviceId;
        public int PortId;
        public string Status = "Active";
        public string Description;
    }
}
