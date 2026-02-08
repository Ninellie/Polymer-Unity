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
        // [SerializeField] private float recoverySpeed = 2.0f;

        /// <summary>
        /// Передаёт текущее значение Scale
        /// </summary>
        public event Action<float> OnScaleChanged;

        public float Scale { get; private set; }
    
        private float _targetScale;
        private float _currentVelocity;

        private void Awake()
        {
            _targetScale = initialScale;
            Scale = initialScale;
        }

        private void Update()
        {
            var newScale = Mathf.SmoothDamp(Scale, _targetScale, ref _currentVelocity, smoothTime);

            if (Mathf.Approximately(Scale, newScale)) return;
            Scale = newScale;
            OnScaleChanged?.Invoke(Scale);
        }
        
        public void AdjustTargetScale(float delta)
        {
            _targetScale += intensity * delta * _targetScale;
            _targetScale = Mathf.Clamp(_targetScale, minScale, maxScale);
        }
    }
}