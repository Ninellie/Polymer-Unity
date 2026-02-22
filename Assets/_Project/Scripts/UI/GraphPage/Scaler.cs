using System;
using UnityEngine;

namespace Polymer.UI.GraphPage
{
    public class Scaler : MonoBehaviour
    {
        [SerializeField] private float initialScale = 0.006f;
        [SerializeField] private float maxScale = 0.3f;
        [SerializeField] private float minScale = 0.0001f;
        [SerializeField] private float smoothTime = 0.2f;
        [SerializeField] private float intensity = 0.1f;
        [SerializeField] private float inertiaDamping = 7f;

        public event Action<float> OnScaleChanged;
        public event Action<Vector2> OnOffsetChanged;

        public float Scale { get; private set; }
        public Vector2 Offset { get; private set; }
    
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

        private void Update()
        {
            var changed = false;

            changed |= UpdateScale();
            changed |= UpdateInertia();

            if (changed)
            {
                OnScaleChanged?.Invoke(Scale);
                OnOffsetChanged?.Invoke(Offset);
            }
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
        
        public void AdjustTargetScale(float delta, Vector2 pivotWorld)
        {
            _pivotWorld = pivotWorld;
            _targetScale += intensity * delta * _targetScale;
            _targetScale = Mathf.Clamp(_targetScale, minScale, maxScale);
        }

        public void AdjustOffset(Vector2 delta)
        {
            _inertiaVelocity = Vector2.zero;
            Offset += delta;
            _lastDragFrame = Time.frameCount;
            if (Time.deltaTime > 0f)
                _trackedVelocity = Vector2.Lerp(_trackedVelocity, delta / Time.deltaTime, 0.3f);
            OnOffsetChanged?.Invoke(Offset);
        }

        public void ReleaseInertia()
        {
            _inertiaVelocity = Time.frameCount - _lastDragFrame <= 2 ? _trackedVelocity : Vector2.zero;
            _trackedVelocity = Vector2.zero;
        }
    }
}