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
            Create("Gravity", layout.Gravity, v => { layout.Gravity = v; layout.Start(); });
            Create("Link Distance", layout.LinkDistance, v => { layout.LinkDistance = v; layout.Start(); });
            Create("Link Strength", layout.LinkStrength, v => { layout.LinkStrength = v; layout.Start(); });
            Create("Friction", layout.Friction, v => { layout.Friction = v; layout.Start(); });
            Create("Charge", layout.Charge, v => { layout.Charge = v; layout.Start(); });
            Create("Theta", layout.Theta, v => { layout.Theta = v; layout.Start(); });
            Create("Alpha", layout.Alpha, v => { layout.Alpha = v; layout.Start(); });
            Create("Max Velocity", layout.MaxVelocity, v => { layout.MaxVelocity = v; layout.Start(); });
            Create("Cell Size", layout.CellSize, v => { layout.CellSize = v; layout.Start(); });
            Create("Dominant Range", layout.DominantRange, v => { layout.DominantRange = v; layout.Start(); });
        }
        
        private void Create(string label, float value, Action<float> setter)
        {
            var field = Instantiate(prefab, root);
            field.Init(label, value, s => setter(float.Parse(s)));
        }
    }
}