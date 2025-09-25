using System;

namespace Core.Models
{
    /// <summary>
    /// Модель устройства с предустановленными параметрами
    /// </summary>
    [Serializable]
    public class DeviceModel
    {
        public int Id;
        public string Name;
        public int DeviceRoleId;
        public int ManufacturerId;
        public string Description;
    }
}
