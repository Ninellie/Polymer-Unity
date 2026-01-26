using System;

namespace Core.Models
{
    /// <summary>
    /// Производитель сетевого оборудования
    /// </summary>
    [Serializable]
    public class Manufacturer
    {
        public int Id;
        public string Name;
        public string Description;
    }
}