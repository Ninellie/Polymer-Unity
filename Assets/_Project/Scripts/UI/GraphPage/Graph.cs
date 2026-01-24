using System;
using System.Collections.Generic;
using FDLayout;

namespace UI.DevicePage
{
    [Serializable]
    public class Graph
    {
        public static Graph Instance
        {
            get
            {
                _instance ??= new Graph();
                return _instance;
            }
        }

        private static Graph _instance;
        
        public readonly List<Node> Nodes = new();
        public readonly List<(Node a, Node b)> Connections = new();
    }
}