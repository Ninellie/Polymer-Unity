using System.Collections.Generic;
using Polymer.UI.Routing;
using UnityEngine;

namespace UI.DevicePage
{
    [RequireComponent(typeof(RectTransform))]
    public class DeviceRolesGraph : PageBase
    {
        [Header("Силы")]
        public float repulsionForce = 100f;   // отталкивание между узлами
        public float attractionForce = 10f;   // притяжение по связям
        public float centralGravity = 5f;     // притяжение к центру контейнера
        public float damping = 0.85f;         // затухание скорости
        public float maxVelocity = 500f;      // ограничение скорости узла

        [Header("Граф")]
        public List<UINode> nodes = new();
        public List<(UINode, UINode)> edges = new();

        [Header("Центр графа")]
        public RectTransform container;

        private void Update()
        {
            if (nodes.Count == 0) return;

            ApplyForces();
            UpdatePositions();
        }

        private void ApplyForces()
        {
            var center = container.rect.center;

            // Отталкивание между узлами
            for (var i = 0; i < nodes.Count; i++)
            {
                var force = Vector2.zero;
                var rtA = nodes[i].GetComponent<RectTransform>();

                for (var j = 0; j < nodes.Count; j++)
                {
                    if (i == j) continue;

                    var rtB = nodes[j].GetComponent<RectTransform>();
                    var dir = rtA.anchoredPosition - rtB.anchoredPosition;
                    var distance = dir.magnitude + 0.1f;
                    force += dir.normalized * (repulsionForce / (distance * distance));
                }

                // Притяжение к центру
                var toCenter = center - rtA.anchoredPosition;
                force += toCenter.normalized * centralGravity;

                nodes[i].Velocity += force * Time.deltaTime;
            }

            // Притяжение по связям
            foreach (var edge in edges)
            {
                var rtA = edge.Item1.GetComponent<RectTransform>();
                var rtB = edge.Item2.GetComponent<RectTransform>();

                var dir = rtB.anchoredPosition - rtA.anchoredPosition;
                var f = dir * (attractionForce * Time.deltaTime);

                edge.Item1.Velocity += f;
                edge.Item2.Velocity -= f;
            }
        }

        private void UpdatePositions()
        {
            foreach (var node in nodes)
            {
                // Ограничение скорости
                if (node.Velocity.magnitude > maxVelocity)
                    node.Velocity = node.Velocity.normalized * maxVelocity;

                RectTransform rt = node.GetComponent<RectTransform>();
                rt.anchoredPosition += node.Velocity * Time.deltaTime;

                node.Velocity *= damping;
            }
        }
        private void Awake()
        {
            CreateTenNodes();
        }

        public override void OnPageInit(PageArgs args)
        {
        }
        
        private void CreateTenNodes()
        {
            var center = container.rect.center;

            for (var i = 0; i < 10; i++)
            {
                var nodeObj = new GameObject("Node_" + i);
                nodeObj.transform.SetParent(container, false);

                var rt = nodeObj.AddComponent<RectTransform>();
                rt.anchoredPosition = center;
                rt.sizeDelta = Vector2.one * 100f; // размер узла

                var node = nodeObj.AddComponent<UINode>();
                node.radius = 50f;
                node.segments = 64;
                node.lineWidth = 2f;
                node.color = Color.white;
                nodes.Add(node);
            }
        }

    }
}