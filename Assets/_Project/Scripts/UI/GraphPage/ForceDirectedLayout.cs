using System.Collections.Generic;
using System.Linq;
using Polymer.UI.Routing;
using TriInspector;
using UnityEngine;

namespace UI.DevicePage
{
    /// <summary>
    /// Applies physics forces to nodes
    /// </summary>
    public class ForceDirectedLayout : PageBase
    {
        [SerializeField] private NodeFactory factory;
        
        [Header("Nodes")]
        [SerializeField] private List<Edge> edges;
        [SerializeField] private List<Node> nodes;
        
        [Header("Spring parameters")]
        [SerializeField] private float linkDistance;
        [SerializeField] private float springPower;
        
        [Header("Gravity parameters")]
        [SerializeField] private float gravityPower;
        
        [Header("Gravity parameters")]
        [SerializeField] private float repulsionPower;
        
        [Header("Velocity parameters")]
        [SerializeField] private float maxVelocity;
        [SerializeField] [ReadOnly] private float damping = 0.99f;

        [SerializeField] private float overlapRepulsion;

        private void Start()
        {
            nodes = factory.Nodes;
            edges = factory.Edges;
        }

        private void FixedUpdate()
        {
            var isDragged = nodes.Any(x => x.isDragged);

            damping = isDragged ? 1f : 0.99f;
            
            ApplyForces();
            IntegrateForces(Time.deltaTime);
            ClampVelocity();
            MoveNodes(Time.deltaTime);
            ApplyExtraRepulsion();
        }

        private void ClampVelocity()
        {
            foreach (var node in nodes)
            {
                node.velocity = Vector2.ClampMagnitude(node.velocity, maxVelocity);
            }
        }

        private void IntegrateForces(float dt)
        {
            foreach (var node in nodes)
            {
                node.velocity += node.force * dt;
                node.velocity *= damping;
            }
        }

        private void MoveNodes(float dt)
        {
            foreach (var node in nodes)
            {
                if (node.isDragged) continue;
                node.RectTransform.anchoredPosition += node.velocity * dt;
            }
        }
        
        private void ApplyForces()
        {
            ClearForces();
            ApplySpringForces();
            ApplyGravity();
            ApplyRepulsion();
        }

        private void ClearForces()
        {
            foreach (var node in nodes)
            {
                node.force = Vector2.zero;
            }
        }

        private void ApplyRepulsion()
        {
            for (var i = 0; i < nodes.Count; i++)
            {
                for (var j = i + 1; j < nodes.Count; j++)
                {
                    var a = nodes[i];
                    var b = nodes[j];

                    var delta = a.RectTransform.anchoredPosition - b.RectTransform.anchoredPosition;
                    var distance = delta.magnitude + 0.0001f;
                    var force = delta.normalized * (repulsionPower / (distance * distance));

                    a.force += force;
                    b.force -= force;
                }
            }
        }
        
        private void ApplySpringForces()
        {
            foreach (var edge in edges)
            {
                var delta = edge.b.RectTransform.anchoredPosition - edge.a.RectTransform.anchoredPosition;
                var distance = delta.magnitude;
                if (distance < 0.0001f) continue;

                var displacement = distance - linkDistance;
                var force = delta.normalized * (displacement * springPower);

                edge.a.force += force;
                edge.b.force -= force;
            }
        }

        private void ApplyGravity()
        {
            foreach (var node in nodes)
            {
                var delta = Vector2.zero - node.RectTransform.anchoredPosition;
                node.force += delta * gravityPower;
            }
        }
        
        private void ApplyExtraRepulsion()
        {
            foreach (var node in nodes)
            {
                if (node.isDragged) continue;
                var displacement = Vector2.zero;
                var posA = node.RectTransform.anchoredPosition;
                var dominantRange = node.drawer.Radius + overlapRepulsion;
                var dominantRangeSqr = dominantRange * dominantRange;

                foreach (var other in nodes)
                {
                    if (other.id == node.id) continue;

                    var posB = other.RectTransform.anchoredPosition;
                    var direction = posA - posB;
                    var distanceSqr = direction.sqrMagnitude;

                    if (!(distanceSqr < dominantRangeSqr)) continue;
                
                    var distance = Mathf.Sqrt(distanceSqr);
                    
                    if (distance < 0.0001f)
                    {
                        direction = Random.insideUnitCircle * 0.01f;
                        distance = direction.magnitude;
                    }
                    
                    var pushForce = (dominantRange - distance) * 0.1f;
                    displacement += direction / distance * pushForce;
                }

                node.RectTransform.anchoredPosition += displacement;
            }
        }

        public override void OnPageInit(PageArgs args)
        {
        }
    }
}