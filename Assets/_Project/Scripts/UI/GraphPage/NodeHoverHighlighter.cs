using System.Collections.Generic;
using FDLayout;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Polymer.UI.GraphPage
{
    public class NodeHoverHighlighter : MonoBehaviour
    {
        [SerializeField] private GraphScaler graphScaler;
        [SerializeField] private Color highlightColor = Color.white;
        [SerializeField] private float fadeAlpha = 0.15f;
        [SerializeField] private float transitionDuration = 0.2f;
        
        [Inject] private List<Node> _nodes;
        [Inject] private NodesRenderer _nodesRenderer;
        [Inject] private LinksRenderer _linksRenderer;
        [Inject] private Camera _mainCamera;

        private Node _hoveredNode;
        private readonly Dictionary<Node, ColorTransition> _transitions = new();
        private readonly List<Node> _transitionNodes = new();

        private struct ColorTransition
        {
            public Color StartColor;
            public Color TargetColor;
            public float Elapsed;
        }

        private void Update()
        {
            var hoverChanged = UpdateHoveredNode();
            var transitionChanged = UpdateTransitions();

            if (!hoverChanged && !transitionChanged) return;

            _nodesRenderer.RecalculateMesh();
            _linksRenderer.RecalculateMesh();
        }
        
        private bool UpdateHoveredNode()
        {
            var screenPos = Pointer.current.position.ReadValue();
            var worldPos = (Vector2)_mainCamera.ScreenToWorldPoint(screenPos);
            var graphPos = (worldPos - graphScaler.Offset) / graphScaler.Scale;

            Node closest = null;
            var closestDistSqr = float.MaxValue;

            foreach (var node in _nodes)
            {
                var distSqr = (node.Position - graphPos).sqrMagnitude;
                var radiusSqr = node.Radius * node.Radius;
                if (distSqr < radiusSqr && distSqr < closestDistSqr)
                {
                    closest = node;
                    closestDistSqr = distSqr;
                }
            }

            if (closest == _hoveredNode) return false;

            ResetHighlight();
            _hoveredNode = closest;
            if (_hoveredNode != null) ApplyHighlight();
            _linksRenderer.SetHoverContext(_hoveredNode, fadeAlpha);
            return true;
        }

        private void ApplyHighlight()
        {
            foreach (var node in _nodes)
            {
                var faded = node.Color;
                faded.a = fadeAlpha;
                SetNodeTargetColor(node, faded);
            }

            SetNodeTargetColor(_hoveredNode, highlightColor);
            foreach (var neighbor in _hoveredNode.Links)
                SetNodeTargetColor(neighbor, neighbor.Color);
        }

        private void ResetHighlight()
        {
            foreach (var node in _nodes)
                SetNodeTargetColor(node, node.Color);
        }

        private void SetNodeTargetColor(Node node, Color targetColor)
        {
            node.TargetDisplayColor = targetColor;

            if (Approximately(node.DisplayColor, targetColor))
            {
                node.DisplayColor = targetColor;
                if (_transitions.Remove(node))
                    _transitionNodes.Remove(node);
                return;
            }

            var transition = new ColorTransition
            {
                StartColor = node.DisplayColor,
                TargetColor = targetColor,
                Elapsed = 0f
            };

            if (!_transitions.TryAdd(node, transition))
            {
                _transitions[node] = transition;
                return;
            }

            _transitionNodes.Add(node);
        }

        private bool UpdateTransitions()
        {
            if (_transitionNodes.Count == 0) return false;

            var changed = false;
            var duration = Mathf.Max(transitionDuration, 0.0001f);

            for (var i = _transitionNodes.Count - 1; i >= 0; i--)
            {
                var node = _transitionNodes[i];
                var transition = _transitions[node];

                transition.Elapsed += Time.deltaTime;
                var t = Mathf.Clamp01(transition.Elapsed / duration);
                var color = Color.Lerp(transition.StartColor, transition.TargetColor, t);

                if (!Approximately(node.DisplayColor, color))
                {
                    node.DisplayColor = color;
                    changed = true;
                }

                if (t < 1f)
                {
                    _transitions[node] = transition;
                    continue;
                }

                node.DisplayColor = transition.TargetColor;
                _transitions.Remove(node);
                _transitionNodes.RemoveAt(i);
            }

            return changed;
        }

        private static bool Approximately(Color a, Color b)
        {
            return Mathf.Abs(a.r - b.r) < 0.001f &&
                   Mathf.Abs(a.g - b.g) < 0.001f &&
                   Mathf.Abs(a.b - b.b) < 0.001f &&
                   Mathf.Abs(a.a - b.a) < 0.001f;
        }
    }
}