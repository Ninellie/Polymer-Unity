using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI.DevicePage
{
    [Serializable]
    public class Node
    {
        public int Id;
        public float Weight;
        public bool IsDragged;
        public Vector2 Position;
        public Vector2 Velocity;
        public Vector2 Force;
        public float Radius;
        public Color Color;
        public readonly HashSet<Node> ConnectedNodes = new();
        
        public Device Device;
    }
}