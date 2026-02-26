using System;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace Polymer.Core.Input
{
    public class InputManager : IInitializable
    {
        public event Action<Vector2> OnScrollWheel;
        public event Action<Vector2> OnDrag;
        public event Action OnDragEnd;
        public event Action OnPrimaryDown;
        public event Action OnPrimaryUp;

        private InputActionMap _uiMap;
        private bool _isDragging;
        private Vector2 _lastPointerPosition;
        
        public void Initialize()
        {
            InputSystem.actions.Disable();
            
            _uiMap = InputSystem.actions.FindActionMap("UI");
            _uiMap.FindAction("ScrollWheel").performed += context => OnScrollWheel?.Invoke(context.ReadValue<Vector2>());

            var middleClick = _uiMap.FindAction("MiddleClick");
            middleClick.started += _ => StartDrag();
            middleClick.canceled += _ => EndDrag();

            var point = _uiMap.FindAction("Point");
            point.performed += context => UpdateDrag(context.ReadValue<Vector2>());

            var click = _uiMap.FindAction("Click");
            click.started += _ => OnPrimaryDown?.Invoke();
            click.canceled += _ => OnPrimaryUp?.Invoke();
            
            _uiMap.Enable();
        }

        private void StartDrag()
        {
            _isDragging = true;
            _lastPointerPosition = Pointer.current.position.ReadValue();
        }

        private void EndDrag()
        {
            _isDragging = false;
            OnDragEnd?.Invoke();
        }

        private void UpdateDrag(Vector2 pointerPosition)
        {
            if (!_isDragging) return;
            var delta = pointerPosition - _lastPointerPosition;
            _lastPointerPosition = pointerPosition;
            OnDrag?.Invoke(delta);
        }
    }
}