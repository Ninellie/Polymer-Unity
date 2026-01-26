using System;

namespace Core.Models
{
    /// <summary>
    /// Роль устройства в сети (Router, Switch, Server, etc.)
    /// </summary>
    [Serializable]
    public class DeviceRole
    {
        public int Id;
        public string Name;
        public string Description;
        public string Color;
    }
}
