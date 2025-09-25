using System;
using UnityEngine;

namespace Core.Models
{
    /// <summary>
    /// Модель локации/здания
    /// </summary>
    [Serializable]
    public class Location
    {
        public int Id;
        public string Name;
        public string Description;
        public Vector3 Position;
        public Vector3 Size = Vector3.one;
    }
}
