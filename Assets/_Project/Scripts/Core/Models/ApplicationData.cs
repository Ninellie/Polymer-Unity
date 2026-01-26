using System;
using System.Collections.Generic;

namespace Core.Models
{
    /// <summary>
    /// Основная модель данных приложения
    /// </summary>
    [Serializable]
    public class ApplicationData
    {
        public List<Device> Devices { get; set; } = new();
        public List<Cable> Cables { get; set; } = new();
    }
}
