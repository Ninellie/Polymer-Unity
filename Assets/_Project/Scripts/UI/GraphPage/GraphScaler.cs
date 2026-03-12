using System;
using Polymer.Core.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Polymer.UI.GraphPage
{
    /// <summary>
    /// Reads Input and updates scale and offset of graph
    /// </summary>
    public class GraphScaler : MonoBehaviour
    {
        [SerializeField] private float initialScale = 0.006f;
        [SerializeField] private float maxScale = 0.3f;
        [SerializeField] private float minScale = 0.0001f;
        [SerializeField] private float smoothTime = 0.2f;
        [SerializeField] private float intensity = 0.1f;
        [SerializeField] private float inertiaDamping = 7f;
        
        [Inject] private NodesRenderer _nodesRenderer;
        [Inject] private LinksRenderer _linksRenderer;
        [Inject] private Camera _camera;
        [Inject] private InputManager _inputManager;

        public Vector2 Offset { get; private set; }
        public float Scale { get; private set; }
        
        private float _targetScale;
        private float _scaleVelocity;
        private Vector2 _pivotWorld;
        private Vector2 _inertiaVelocity;
        private Vector2 _trackedVelocity;
        private int _lastDragFrame;

        private void Awake()
        {
            _targetScale = initialScale;
            Scale = initialScale;
        }

        private void Start()
        {
            _inputManager.OnScrollWheel += UpdateScale;
            _inputManager.OnDrag += UpdateOffset;
            _inputManager.OnDragEnd += OnDragEnd;
        }

        private void OnDestroy()
        {
            _inputManager.OnScrollWheel -= UpdateScale;
            _inputManager.OnDrag -= UpdateOffset;
            _inputManager.OnDragEnd -= OnDragEnd;
        }

        private void Update()
        {
            var changed = false;

            changed |= UpdateScale();
            changed |= UpdateInertia();

            if (!changed) return;
            ApplyScale(Scale);
            ApplyOffset(Offset);
        }
        
        private void UpdateScale(Vector2 delta)
        {
            var cursorWorld = (Vector2)_camera.ScreenToWorldPoint(Pointer.current.position.ReadValue());
            AdjustTargetScale(delta.y, cursorWorld);
        }

        private void UpdateOffset(Vector2 screenDelta)
        {
            var worldDelta = screenDelta * (2f * _camera.orthographicSize / Screen.height);
            AdjustOffset(worldDelta);
        }

        private void OnDragEnd()
        {
            ReleaseInertia();
        }
        
        private void ApplyScale(float value)
        {
            _linksRenderer.Scale = value;
            _nodesRenderer.Scale = value;
            _linksRenderer.RecalculateMesh();
            _nodesRenderer.RecalculateMesh();
        }

        private void ApplyOffset(Vector2 offset)
        {
            _nodesRenderer.Offset = offset;
            _linksRenderer.Offset = offset;
            _nodesRenderer.RecalculateMesh();
            _linksRenderer.RecalculateMesh();
        }

        private bool UpdateScale()
        {
            var newScale = Mathf.SmoothDamp(Scale, _targetScale, ref _scaleVelocity, smoothTime);
            if (Mathf.Abs(newScale - _targetScale) < 0.00001f)
            {
                newScale = _targetScale;
                _scaleVelocity = 0f;
            }
            if (Mathf.Approximately(Scale, newScale)) return false;

            var scaleRatio = newScale / Scale;
            Offset = _pivotWorld - (_pivotWorld - Offset) * scaleRatio;
            Scale = newScale;
            return true;
        }

        private bool UpdateInertia()
        {
            if (_inertiaVelocity.sqrMagnitude < 0.01f)
            {
                _inertiaVelocity = Vector2.zero;
                return false;
            }

            Offset += _inertiaVelocity * Time.deltaTime;
            _inertiaVelocity = Vector2.Lerp(_inertiaVelocity, Vector2.zero, inertiaDamping * Time.deltaTime);
            return true;
        }

        private void AdjustTargetScale(float delta, Vector2 pivotWorld)
        {
            _pivotWorld = pivotWorld;
            _targetScale += intensity * delta * _targetScale;
            _targetScale = Mathf.Clamp(_targetScale, minScale, maxScale);
        }

        private void AdjustOffset(Vector2 delta)
        {
            _inertiaVelocity = Vector2.zero;
            Offset += delta;
            _lastDragFrame = Time.frameCount;
            if (Time.deltaTime > 0f)
                _trackedVelocity = Vector2.Lerp(_trackedVelocity, delta / Time.deltaTime, 0.3f);
            ApplyOffset(Offset);
        }

        private void ReleaseInertia()
        {
            _inertiaVelocity = Time.frameCount - _lastDragFrame <= 2 ? _trackedVelocity : Vector2.zero;
            _trackedVelocity = Vector2.zero;
        }
    }
}