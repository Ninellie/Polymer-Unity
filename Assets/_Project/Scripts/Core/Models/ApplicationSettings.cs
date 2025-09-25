using System;

namespace Core.Models
{
    /// <summary>
    /// Настройки приложения
    /// </summary>
    [Serializable]
    public class ApplicationSettings
    {
        public float GridSize = 1.0f;
        public bool ShowGrid = true;
        public bool AutoSave = true;
        public int AutoSaveInterval = 300; // секунды
        public string Theme = "Light";
        public float CameraSpeed = 5.0f;
        public bool ShowLabels = true;
    }
}
