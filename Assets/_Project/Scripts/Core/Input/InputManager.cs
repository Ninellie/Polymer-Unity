using System;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace Polymer.Core.Input
{
    public class InputManager : IInitializable
    {
        public event Action<Vector2> OnScrollWheel;

        private InputActionMap _uiMap;
        
        public void Initialize()
        {
            InputSystem.actions.Disable();
            
            _uiMap = InputSystem.actions.FindActionMap("UI");
            _uiMap.FindAction("ScrollWheel").performed += context => OnScrollWheel?.Invoke(context.ReadValue<Vector2>());
            
            _uiMap.Enable();
        }
    }
}