using System.Collections.Generic;
using FDLayout;
using UnityEngine;
using VContainer;

namespace Polymer.UI.GraphPage
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class LinksRenderer : MonoBehaviour
    {
        [SerializeField] private Material linkMaterial;
        [SerializeField] private float thickness;
        [SerializeField] private float scale;
        [SerializeField] private float linkFadeDuration = 0.2f;

        public bool IsRendering { get; set; } = true;

        public float Scale
        {
            get => scale;
            set => scale = value;
        }

        public Vector2 Offset { get; set; }
        
        [Inject] private List<(Node a, Node b)> _links;
        private Node _hoveredNode;
        private float _fadedAlpha = 0.15f;
        private float[] _linkAlphaCurrent;
        private float[] _linkAlphaStart;
        private float[] _linkAlphaTarget;
        private float[] _linkAlphaElapsed;
        private bool[] _linkAlphaAnimating;

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
            if (!IsRendering) return;

            RecalculateMesh();
        }

        public void RecalculateMesh()
        {
            var linkCount = _links.Count;

            if (linkCount == 0)
            {
                _mesh.Clear();
                return;
            }

            if (linkCount != _lastLinkCount)
            {
                _vertices = new Vector3[linkCount * 4];
                _indices = new int[linkCount * 6];
                _colors = new Color[linkCount * 4];
                EnsureLinkAlphaState(linkCount);
                RebuildLinkAlphaTargets();
                
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
                var posA = (Vector3)(link.a.Position * Scale + Offset);
                var posB = (Vector3)(link.b.Position * Scale + Offset);
                var colorA = link.a.DisplayColor;
                var colorB = link.b.DisplayColor;
                var alpha = UpdateLinkAlpha(i);
                colorA.a *= alpha;
                colorB.a *= alpha;

                UpdateLinkGeometry(i, posA, posB, colorA, colorB);
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

            // Запись цвета (градиент между узлами)
            _colors[v] = _colors[v + 3] = colorA;
            _colors[v + 1] = _colors[v + 2] = colorB;
        }

        public void SetHoverContext(Node hoveredNode, float fadedAlpha)
        {
            _hoveredNode = hoveredNode;
            _fadedAlpha = fadedAlpha;
            RebuildLinkAlphaTargets();
        }

        private void EnsureLinkAlphaState(int linkCount)
        {
            if (_linkAlphaCurrent != null && _linkAlphaCurrent.Length == linkCount) return;

            _linkAlphaCurrent = new float[linkCount];
            _linkAlphaStart = new float[linkCount];
            _linkAlphaTarget = new float[linkCount];
            _linkAlphaElapsed = new float[linkCount];
            _linkAlphaAnimating = new bool[linkCount];

            for (var i = 0; i < linkCount; i++)
            {
                _linkAlphaCurrent[i] = 1f;
                _linkAlphaStart[i] = 1f;
                _linkAlphaTarget[i] = 1f;
                _linkAlphaElapsed[i] = 0f;
                _linkAlphaAnimating[i] = false;
            }
        }

        private void RebuildLinkAlphaTargets()
        {
            var linkCount = _links.Count;
            EnsureLinkAlphaState(linkCount);

            for (var i = 0; i < linkCount; i++)
            {
                var link = _links[i];
                var targetAlpha = _hoveredNode != null && link.a != _hoveredNode && link.b != _hoveredNode
                    ? _fadedAlpha
                    : 1f;

                if (Mathf.Abs(_linkAlphaCurrent[i] - targetAlpha) < 0.001f)
                {
                    _linkAlphaCurrent[i] = targetAlpha;
                    _linkAlphaTarget[i] = targetAlpha;
                    _linkAlphaAnimating[i] = false;
                    continue;
                }

                _linkAlphaStart[i] = _linkAlphaCurrent[i];
                _linkAlphaTarget[i] = targetAlpha;
                _linkAlphaElapsed[i] = 0f;
                _linkAlphaAnimating[i] = true;
            }
        }

        private float UpdateLinkAlpha(int index)
        {
            if (!_linkAlphaAnimating[index]) return _linkAlphaCurrent[index];

            var duration = Mathf.Max(linkFadeDuration, 0.0001f);
            _linkAlphaElapsed[index] += Time.deltaTime;
            var t = Mathf.Clamp01(_linkAlphaElapsed[index] / duration);
            _linkAlphaCurrent[index] = Mathf.Lerp(_linkAlphaStart[index], _linkAlphaTarget[index], t);

            if (t >= 1f)
            {
                _linkAlphaCurrent[index] = _linkAlphaTarget[index];
                _linkAlphaAnimating[index] = false;
            }

            return _linkAlphaCurrent[index];
        }
    }
}