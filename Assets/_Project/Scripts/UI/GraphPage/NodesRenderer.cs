using System.Collections.Generic;
using FDLayout;
using UnityEngine;

namespace Polymer.UI.GraphPage
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class NodesRenderer : MonoBehaviour
    {
        [SerializeField] private Material nodeMaterial;
        [SerializeField] private float scale;

        public bool IsRendering { get; set; } = true;
        
        private List<Node> _nodes;
        private Mesh _mesh;
        private Vector3[] _vertices;
        private int[] _indices;
        private Color[] _colors;
        private Vector2[] _uvs;
        private int _lastNodeCount = 0;
        
        private Mesh _quadMesh;
        
        private void Start()
        {
            _mesh = new Mesh();
            _mesh.MarkDynamic();
            GetComponent<MeshFilter>().mesh = _mesh;

            if (nodeMaterial == null) return;
            
            GetComponent<MeshRenderer>().material = nodeMaterial;
        }
        
        private void LateUpdate()
        {
            if (!IsRendering) return;
            
            var nodeCount = _nodes.Count;

            if (nodeCount == 0) return;
            
            if (nodeCount != _lastNodeCount)
            {
                _vertices = new Vector3[nodeCount * 4];
                _indices = new int[nodeCount * 6];
                _colors = new Color[nodeCount * 4];
                _uvs = new Vector2[nodeCount * 4];

                var i = 0;
                foreach (var node in _nodes)
                {
                    var v = i * 4;
                    var t = i * 6;

                    _indices[t] = v;
                    _indices[t + 1] = v + 1;
                    _indices[t + 2] = v + 2;
                    _indices[t + 3] = v;
                    _indices[t + 4] = v + 2;
                    _indices[t + 5] = v + 3;
                    
                    _uvs[v] = new Vector2(0, 0);
                    _uvs[v + 1] = new Vector2(0, 1);
                    _uvs[v + 2] = new Vector2(1, 1);
                    _uvs[v + 3] = new Vector2(1, 0);
                    
                    _colors[v] = node.Color;
                    _colors[v + 1] = node.Color;
                    _colors[v + 2] = node.Color;
                    _colors[v + 3] = node.Color;

                    i++;
                }

                _mesh.Clear();
                _mesh.vertices = _vertices;
                _mesh.triangles = _indices;
                _mesh.colors = _colors;
                _mesh.uv = _uvs;

                _lastNodeCount = nodeCount;
            }

            // Обновляем позиции
            var index = 0;
            foreach (var node in _nodes)
            {
                var r = node.Radius * scale;
                var pos = node.Position * scale;

                var v = index * 4;
                _vertices[v] = new Vector3(pos.x - r, pos.y - r, 0);
                _vertices[v + 1] = new Vector3(pos.x - r, pos.y + r, 0);
                _vertices[v + 2] = new Vector3(pos.x + r, pos.y + r, 0);
                _vertices[v + 3] = new Vector3(pos.x + r, pos.y - r, 0);

                index++;
            }

            _mesh.vertices = _vertices;
            _mesh.RecalculateBounds();
        }

        public void SetNodes(List<Node> nodes)
        {
            _nodes = nodes;
        }
    }
}