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
        
        [Inject] private List<Node> _nodes;
        [Inject] private List<(Node a, Node b)> _connections;
        [Inject] private NodesRenderer _nodesRenderer;
        [Inject] private LinksRenderer _linksRenderer;
        [Inject] private Camera _mainCamera;

        private Node _hoveredNode;
        private bool _isNodeHovered;

        private void Update()
        {
            UpdateHoveredNode();
        }
        
        private void UpdateHoveredNode()
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

            if (closest == _hoveredNode) return;

            ResetHighlight();
            _hoveredNode = closest;
            _isNodeHovered = closest != null;
            if (_isNodeHovered) ApplyHighlight();
            _linksRenderer.SetHoverContext(_hoveredNode, fadeAlpha);
            _nodesRenderer.RecalculateMesh();
            _linksRenderer.RecalculateMesh();
        }

        private void ApplyHighlight()
        {
            foreach (var node in _nodes)
            {
                var faded = node.Color;
                faded.a = fadeAlpha;
                node.DisplayColor = faded;
            }

            _hoveredNode.DisplayColor = highlightColor;
            foreach (var neighbor in _hoveredNode.Links)
                neighbor.DisplayColor = neighbor.Color;
        }

        private void ResetHighlight()
        {
            foreach (var node in _nodes)
                node.DisplayColor = node.Color;
        }
    }
}