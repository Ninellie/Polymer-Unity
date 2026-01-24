using System.Collections.Generic;
using UnityEngine;

namespace FDLayout
{
    public class ForceDirectedLayout
    {
        public List<Node> Nodes { get; set; }
        public List<(Node a, Node b)> Links { get; set; }
        
        public float Gravity { get; set; }
        public float LinkDistance { get; set; }
        public float LinkStrength { get; set; }
        public float Friction { get; set; } // alpha decrease per second
        public float Charge { get; set; }
        public float ChargeDistance { get; set; }
        public float Theta { get; set; }
        public float Alpha { get; set; }
        
        public float MaxVelocity { get; set; }
        
        public bool IsSimulated { get; set; }

        private float _dumping;
        
        public ForceDirectedLayout(
            List<Node> nodes = null,
            List<(Node a, Node b)> links = null,
            float gravity = 1,
            float linkDistance = 75,
            float linkStrength = 0.4f,
            float friction = 1,
            float charge = 2,
            float theta = 0.8f,
            float alpha = 1f,
            float maxVelocity = 600)
        {
            Nodes = nodes;
            Links = links;
            Gravity = gravity;
            LinkDistance = linkDistance;
            LinkStrength = linkStrength;
            Friction = friction;
            Charge = charge;
            Theta = theta;
            Alpha = alpha;
            MaxVelocity = maxVelocity;
        }

        public void Start()
        {
            _dumping = Alpha;
            IsSimulated = true;
        }
        
        public void Tick(float dt)
        {
            if (!IsSimulated) return;
            
            ClearForces();
            
            ApplyForces();
                
            TranslateNodes(dt);
                
            _dumping -= Friction * dt;
                
            if (_dumping > 0) return;
            
            IsSimulated = false;
        }
        
        private void ApplyForces()
        {
            if (LinkStrength > 0) ApplySpringForces();
            if (Gravity > 0) ApplyGravity();
            if (Charge > 0) ApplyRepulsion();
        }
        
        private void ClearForces()
        {
            foreach (var node in Nodes)
            {
                node.Force = Vector2.zero;
            }
        }

        private void ApplyRepulsion()
        {
            for (var i = 0; i < Nodes.Count; i++)
            {
                for (var j = i + 1; j < Nodes.Count; j++)
                {
                    var a = Nodes[i];
                    var b = Nodes[j];

                    var delta = a.Position - b.Position;
                    var distance = delta.magnitude + 0.0001f;
                    var force = delta / distance * Charge;

                    a.Force += force;
                    b.Force -= force;
                }
            }
        }
        
        private void ApplySpringForces()
        {
            foreach (var connection in Links)
            {
                var delta = connection.b.Position - connection.a.Position;
                var distance = delta.magnitude;
                if (distance < 0.0001f) continue;

                var displacement = distance - LinkDistance;
                var transition = delta.normalized * (displacement * LinkStrength);
                var totalWeight = connection.a.Weight + connection.b.Weight;
                
                var aTransition = transition * (connection.b.Weight / totalWeight);
                var bTransition = transition * (connection.a.Weight / totalWeight);
                
                if (!connection.a.IsFixed)
                {
                    connection.a.Force += aTransition;
                }

                if (!connection.b.IsFixed)
                {
                    connection.b.Force -= bTransition;   
                }
            }
        }

        private void ApplyGravity()
        {
            foreach (var node in Nodes)
            {
                var delta = Vector2.zero - node.Position;
                node.Force += delta * Gravity;
            }
        }
        
        private void TranslateNodes(float dt)
        {
            foreach (var node in Nodes)
            {
                if (node.IsFixed) continue;
                node.Velocity = node.Force * dt;
                node.Velocity *= _dumping;
                node.Velocity = Vector2.ClampMagnitude(node.Velocity, MaxVelocity);
                node.Position += node.Velocity;
            }
        }
        
        // private void ApplyExtraRepulsion()
        // {
        //     foreach (var node in graph.Nodes)
        //     {
        //         if (node.IsFixed) continue;
        //         var displacement = Vector2.zero;
        //         var posA = node.Position;
        //         var dominantRange = node.Radius + overlapRepulsion;
        //         var dominantRangeSqr = dominantRange * dominantRange;
        //
        //         foreach (var other in graph.Nodes)
        //         {
        //             if (other.Id == node.Id) continue;
        //
        //             var posB = other.Position;
        //             var direction = posA - posB;
        //             var distanceSqr = direction.sqrMagnitude;
        //
        //             if (!(distanceSqr < dominantRangeSqr)) continue;
        //         
        //             var distance = Mathf.Sqrt(distanceSqr);
        //             
        //             if (distance < 0.0001f)
        //             {
        //                 direction = Random.insideUnitCircle * 0.01f;
        //                 distance = direction.magnitude;
        //             }
        //             
        //             var pushForce = (dominantRange - distance) * 0.1f;
        //             displacement += direction / distance * pushForce;
        //         }
        //
        //         node.Position += displacement;
        //     }
        // }
    }
}