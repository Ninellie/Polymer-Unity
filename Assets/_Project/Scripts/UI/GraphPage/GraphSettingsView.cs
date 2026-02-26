using System;
using FDLayout;
using UnityEngine;
using VContainer;

namespace Polymer.UI.GraphPage
{
    public class GraphSettingsView : MonoBehaviour
    {
        [SerializeField] private Transform root;
        [SerializeField] private SettingSlider prefab;
        [Inject] private ForceDirectedLayout _layout;
        
        public void Start()
        {
            Create("Speed", _layout.SpeedMultiplier, 0f, 5f, v => { _layout.SpeedMultiplier = v; _layout.Start(); });
            Create("Gravity", _layout.Gravity, 0f, 1f, v => { _layout.Gravity = v; _layout.Start(); });
            Create("Link Distance", _layout.LinkDistance, 50f, 500f, v => { _layout.LinkDistance = v; _layout.Start(); });
            Create("Link Strength", _layout.LinkStrength, 0f, 5f, v => { _layout.LinkStrength = v; _layout.Start(); });
            Create("Friction", _layout.Friction, 0f, 0.1f, v => { _layout.Friction = v; _layout.Start(); });
            Create("Charge", _layout.Charge, 0f, 5f, v => { _layout.Charge = v; _layout.Start(); });
        }
        
        private void Create(string label, float value, float minValue, float maxValue, Action<float> setter)
        {
            var field = Instantiate(prefab, root);
            field.Init(label, value, minValue, maxValue, s => setter(s));
        }
    }
}