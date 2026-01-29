using System.Collections.Generic;
using FDLayout;
using UnityEngine;

namespace Polymer.UI.GraphPage
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class LinksRenderer : MonoBehaviour
    {
        [SerializeField] private Material linkMaterial;
        [SerializeField] private float thickness = 0.1f;
        [SerializeField] private float scale = 1f;

        private List<(Node a, Node b)> _links;
        private Mesh _mesh;
        private Vector3[] _vertices;
        private int[] _indices;
        private Color[] _colors;
        private int _lastLinkCount = 0;

        private void Start()
        {
            _mesh = new Mesh { name = "LinksMesh" };
            _mesh.MarkDynamic();
            GetComponent<MeshFilter>().mesh = _mesh;
            GetComponent<MeshRenderer>().material = linkMaterial;
        }

        private void LateUpdate()
        {
            var linkCount = _links.Count;

            if (linkCount == 0) { _mesh.Clear(); return; }

            if (linkCount != _lastLinkCount)
            {
                _vertices = new Vector3[linkCount * 4];
                _indices = new int[linkCount * 6];
                _colors = new Color[linkCount * 4];
                
                for (var i = 0; i < linkCount; i++)
                {
                    var v = i * 4;
                    var t = i * 6;
                    _indices[t] = v; _indices[t + 1] = v + 1; _indices[t + 2] = v + 2;
                    _indices[t + 3] = v; _indices[t + 4] = v + 2; _indices[t + 5] = v + 3;
                }
                _mesh.Clear();
                _mesh.vertices = _vertices;
                _mesh.triangles = _indices;
                _lastLinkCount = linkCount;
            }

            for (var i = 0; i < linkCount; i++)
            {
                var link = _links[i];
                UpdateLinkGeometry(i, link.a.Position * scale, link.b.Position * scale, link.a.Color, link.b.Color);
            }

            _mesh.vertices = _vertices;
            _mesh.colors = _colors;
            _mesh.RecalculateBounds();
        }

        private void UpdateLinkGeometry(int index, Vector3 start, Vector3 end, Color colorA, Color colorB)
        {
            var dir = (end - start).normalized;
            var normal = new Vector3(-dir.y, dir.x, 0) * thickness; // Перпендикуляр для толщины

            var v = index * 4;
            _vertices[v] = start + normal;
            _vertices[v + 1] = end + normal;
            _vertices[v + 2] = end - normal;
            _vertices[v + 3] = start - normal;

            // Запись цвета (можно делать градиент между узлами)
            _colors[v] = _colors[v + 3] = colorA;
            _colors[v + 1] = _colors[v + 2] = colorB;
        }

        public void SetLinks(List<(Node a, Node b)> links)
        {
            _links = links;
        }
    }
}