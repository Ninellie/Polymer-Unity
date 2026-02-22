using System;
using FDLayout;
using UnityEngine;

namespace Polymer.UI.GraphPage
{
    public class GraphSettings : MonoBehaviour
    {
        [SerializeField] private Transform root;
        [SerializeField] private SettingSlider prefab;

        public void Init(ForceDirectedLayout layout)
        {
            Create("Speed", layout.SpeedMultiplier, 0f, 5f, v => { layout.SpeedMultiplier = v; layout.Start(); });
            Create("Gravity", layout.Gravity, 0f, 1f, v => { layout.Gravity = v; layout.Start(); });
            Create("Link Distance", layout.LinkDistance, 50f, 500f, v => { layout.LinkDistance = v; layout.Start(); });
            Create("Link Strength", layout.LinkStrength, 0f, 5f, v => { layout.LinkStrength = v; layout.Start(); });
            Create("Friction", layout.Friction, 0f, 0.1f, v => { layout.Friction = v; layout.Start(); });
            Create("Charge", layout.Charge, 0f, 5f, v => { layout.Charge = v; layout.Start(); });
        }
        
        private void Create(string label, float value, float minValue, float maxValue, Action<float> setter)
        {
            var field = Instantiate(prefab, root);
            field.Init(label, value, minValue, maxValue, s => setter(s));
        }
    }
}