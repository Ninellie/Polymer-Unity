using System.Linq;
using Core.Models;
using FDLayout;
using Polymer.Core.Input;
using Polymer.UI.Routing;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace Polymer.UI.GraphPage
{
    /// <summary>
    /// Applies physics forces to nodes
    /// </summary>
    public class ForceDirectedLayoutPage : PageBase
    {
        [SerializeField] private bool isGeometric = true;
        [SerializeField] private GraphScaler graphScaler;
        [SerializeField] private NodeHoverHighlighter hoverHighlighter;
        [SerializeField] private TextMeshProUGUI deviceDetails;
        [SerializeField] private TextMeshProUGUI connectionDetails;

        [Inject] private ForceDirectedLayout _layout;
        [Inject] private NodesRenderer _nodesRenderer;
        [Inject] private LinksRenderer _linksRenderer;
        [Inject] private InputManager _inputManager;
        [Inject] private Camera _mainCamera;
        [Inject] private ApplicationData _appData;

        private GraphDeviceDetailsFormatter _deviceDetailsFormatter;
        private GraphConnectionDetailsFormatter _connectionDetailsFormatter;
        private Node _draggedNode;
        private Vector2 _dragOffset;

        private void Start()
        {
            _deviceDetailsFormatter = new GraphDeviceDetailsFormatter();
            _connectionDetailsFormatter = new GraphConnectionDetailsFormatter(_appData);

            if (hoverHighlighter != null)
            {
                hoverHighlighter.HoveredNodeChanged += OnHoveredNodeChanged;
            }

            _inputManager.OnPrimaryDown += StartNodeDrag;
            _inputManager.OnPrimaryUp += StopNodeDrag;
        }

        private void OnDestroy()
        {
            if (hoverHighlighter != null)
            {
                hoverHighlighter.HoveredNodeChanged -= OnHoveredNodeChanged;
            }

            _inputManager.OnPrimaryDown -= StartNodeDrag;
            _inputManager.OnPrimaryUp -= StopNodeDrag;
        }

        public void Reload()
        {
            //todo prefab reload, not scene reload
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void Update()
        {
            if (_draggedNode != null)
            {
                UpdateDraggedNodePosition();
                _layout.Start();
            }

            _layout.Tick(Time.deltaTime);
            _layout.IsGeometric = isGeometric;
            _nodesRenderer.IsRendering = _layout.IsColliding;
            _linksRenderer.IsRendering = _layout.IsColliding;
        }

        private void OnHoveredNodeChanged(Node node)
        {
            var noUi = deviceDetails == null && connectionDetails == null;
            if (noUi) return;

            if (node == null)
            {
                if (deviceDetails != null)
                {
                    deviceDetails.text = string.Empty;
                }

                if (connectionDetails != null)
                {
                    connectionDetails.text = string.Empty;
                }

                return;
            }

            var device = _appData.Devices.FirstOrDefault(d => d.Id == node.Id);
            if (deviceDetails != null)
            {
                deviceDetails.text = _deviceDetailsFormatter.Build(device);
            }

            if (connectionDetails != null)
            {
                connectionDetails.text = _connectionDetailsFormatter.Build(node.Id);
            }
        }

        private void StartNodeDrag()
        {
            if (hoverHighlighter == null) return;

            var hoveredNode = hoverHighlighter.HoveredNode;
            if (hoveredNode == null) return;

            _draggedNode = hoveredNode;
            hoverHighlighter.LockHover(_draggedNode);
            _draggedNode.IsFixed = true;
            _draggedNode.Velocity = Vector2.zero;

            var cursorGraphPos = GetCursorGraphPosition();
            _dragOffset = _draggedNode.Position - cursorGraphPos;
        }

        private void StopNodeDrag()
        {
            if (_draggedNode == null) return;

            _draggedNode.IsFixed = false;
            _draggedNode.Velocity = Vector2.zero;
            _draggedNode = null;
            if (hoverHighlighter != null)
            {
                hoverHighlighter.UnlockHover();
            }

            _layout.Start();
        }

        private void UpdateDraggedNodePosition()
        {
            var cursorGraphPos = GetCursorGraphPosition();
            _draggedNode.Position = cursorGraphPos + _dragOffset;
            _draggedNode.Velocity = Vector2.zero;
        }

        private Vector2 GetCursorGraphPosition()
        {
            var cursorWorld = (Vector2)_mainCamera.ScreenToWorldPoint(UnityEngine.InputSystem.Pointer.current.position.ReadValue());
            return (cursorWorld - graphScaler.Offset) / graphScaler.Scale;
        }

        public override void OnPageInit(PageArgs args)
        {
        }
    }
}
