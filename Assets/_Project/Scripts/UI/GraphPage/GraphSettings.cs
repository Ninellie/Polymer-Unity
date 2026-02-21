using System;
using FDLayout;
using UnityEngine;

namespace Polymer.UI.GraphPage
{
    public class GraphSettings : MonoBehaviour
    {
        [SerializeField] private Transform root;
        [SerializeField] private SettingInputField prefab;

        public void Init(ForceDirectedLayout layout)
        {
            Create("Speed", layout.SpeedMultiplier, v => { layout.SpeedMultiplier = v; layout.Start(); });
            Create("Gravity", layout.Gravity, v => { layout.Gravity = v; layout.Start(); });
            Create("Link Distance", layout.LinkDistance, v => { layout.LinkDistance = v; layout.Start(); });
            Create("Link Strength", layout.LinkStrength, v => { layout.LinkStrength = v; layout.Start(); });
            Create("Friction", layout.Friction, v => { layout.Friction = v; layout.Start(); });
            Create("Charge", layout.Charge, v => { layout.Charge = v; layout.Start(); });
            Create("Tangential Strength", layout.TangentialStrength, v => { layout.TangentialStrength = v; layout.Start(); });
        }
        
        private void Create(string label, float value, Action<float> setter)
        {
            var field = Instantiate(prefab, root);
            field.Init(label, value, s => setter(float.Parse(s)));
        }
    }
}