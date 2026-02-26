using System.Collections.Generic;
using UnityEngine;

namespace FDLayout
{
    public class Node
    {
        public int Id;
        public float Weight => Links.Count + 1;
        public bool IsFixed;
        public Vector2 Position;
        public Vector2 Velocity;
        public Vector2 Force;
        public float Radius;
        public Color Color;
        public Color DisplayColor;
        public readonly HashSet<Node> Links = new();
    }
}