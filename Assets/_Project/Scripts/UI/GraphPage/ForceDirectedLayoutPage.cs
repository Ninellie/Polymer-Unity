using Polymer.UI.Routing;
using TriInspector;
using UnityEngine;

namespace UI.DevicePage
{
    public class ForceDirectedLayout1
    {
        public void Tick(float dt)
        {
            
        } 
    }
    
    
    
    /// <summary>
    /// Applies physics forces to nodes
    /// </summary>
    public class ForceDirectedLayoutPage : PageBase
    {
        [SerializeField] private NodeFactory factory;
        [Header("Spring parameters")]

        [SerializeField] private float linkDistance;
        [SerializeField] private float springPower;
        
        [Header("Gravity parameters")]
        [SerializeField] private float gravityPower;
        
        [Header("Gravity parameters")]
        [SerializeField] private float repulsionPower;
        
        [Header("Velocity parameters")]
        [SerializeField] private float maxVelocity;
        
        [Header("Damping parameters")]
        [SerializeField] [ReadOnly] private float damping;
        [SerializeField] private float baseDamping;
        [SerializeField] private float dampingDecreasePerSecond = 3f;
        
        [SerializeField] private float overlapRepulsion;

        [SerializeField] [ReadOnly] private bool _isSimulated;
        
        // [SerializeField] [ReadOnly] private NodeComponent _selected;

        [SerializeField] private Graph graph;
        
        
        
        private void Start()
        {
            graph = Graph.Instance;
            StartSimulation();
        }

        public void StartSimulation()
        {
            damping = baseDamping;
            _isSimulated = true;
        }

        private void Update()
        {
            if (_isSimulated)
            {
                ApplyForces();
                
                TranslateNodes(Time.deltaTime);
                
                damping -= dampingDecreasePerSecond * Time.deltaTime;
                
                if (damping < 0)
                {
                    _isSimulated = false;
                }
            }
            
            ApplyExtraRepulsion();
        }


        private void TranslateNodes(float dt)
        {
            foreach (var node in graph.Nodes)
            {
                if (node.IsDragged) continue;
                node.Velocity = node.Force * dt;
                node.Velocity *= damping;
                node.Velocity = Vector2.ClampMagnitude(node.Velocity, maxVelocity);
                node.Position += node.Velocity;
            }
        }

        private void ApplyForces()
        {
            ClearForces();
            if (springPower > 0) ApplySpringForces();
            if (gravityPower > 0) ApplyGravity();
            if (repulsionPower > 0) ApplyRepulsion();
        }

        private void ClearForces()
        {
            foreach (var node in graph.Nodes)
            {
                node.Force = Vector2.zero;
            }
        }

        private void ApplyRepulsion()
        {
            var nodes = graph.Nodes;
            
            for (var i = 0; i < nodes.Count; i++)
            {
                for (var j = i + 1; j < nodes.Count; j++)
                {
                    var a = nodes[i];
                    var b = nodes[j];

                    var delta = a.Position - b.Position;
                    var distance = delta.magnitude + 0.0001f;
                    var force = delta / distance * repulsionPower;

                    a.Force += force;
                    b.Force -= force;
                }
            }
        }
        
        private void ApplySpringForces()
        {
            foreach (var connection in graph.Connections)
            {
                var delta = connection.b.Position - connection.a.Position;
                var distance = delta.magnitude;
                if (distance < 0.0001f) continue;

                var displacement = distance - linkDistance;
                var transition = delta.normalized * (displacement * springPower);
                var totalWeight = connection.a.Weight + connection.b.Weight;
                
                var aTransition = transition * (connection.b.Weight / totalWeight);
                var bTransition = transition * (connection.a.Weight / totalWeight);
                
                if (!connection.a.IsDragged)
                {
                    connection.a.Force += aTransition;
                }

                if (!connection.b.IsDragged)
                {
                    connection.b.Force -= bTransition;   
                }
            }
        }

        private void ApplyGravity()
        {
            foreach (var node in graph.Nodes)
            {
                var delta = Vector2.zero - node.Position;
                node.Force += delta * gravityPower;
            }
        }

        private void ApplyExtraRepulsion()
        {
            foreach (var node in graph.Nodes)
            {
                if (node.IsDragged) continue;
                var displacement = Vector2.zero;
                var posA = node.Position;
                var dominantRange = node.Radius + overlapRepulsion;
                var dominantRangeSqr = dominantRange * dominantRange;

                foreach (var other in graph.Nodes)
                {
                    if (other.Id == node.Id) continue;

                    var posB = other.Position;
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

                node.Position += displacement;
            }
        }
        
        public override void OnPageInit(PageArgs args)
        {
        }
        
        //
        // public void HoverNode(NodeComponent nodeComponent)
        // {
        //     if (!_isHighlightingAllowed) return;
        //     var selectedNodes = new List<NodeComponent>(nodeComponent.linkedNodes) { nodeComponent };
        //     var nonSelectedNodes = nodes.Except(selectedNodes);
        //     foreach (var nonSelectedNode in nonSelectedNodes)
        //     {
        //         nonSelectedNode.Fade();
        //     }
        // }
        //
        // public void UnhoverNode(NodeComponent nodeComponent)
        // {
        //     if (!_isHighlightingAllowed) return;
        //     var selectedNodes = new List<NodeComponent>(nodeComponent.linkedNodes) { nodeComponent };
        //     var nonSelectedNodes = nodes.Except(selectedNodes);
        //     foreach (var nonSelectedNode in nonSelectedNodes)
        //     {
        //         nonSelectedNode.UndoFade();
        //     }
        // }
        //
        // public void SetSelectedNode(NodeComponent nodeComponent)
        // {
        //     if (_selected == null)
        //     {
        //         _selected = nodeComponent;
        //     }
        //     else
        //     {
        //         // add edge
        //         if (nodeComponent.linkedNodes.Contains(_selected))
        //         {
        //             nodeComponent.linkedNodes.Remove(_selected);
        //             _selected.linkedNodes.Remove(nodeComponent);
        //
        //             var edge = edges.Find(x => 
        //                 x.a == nodeComponent && x.b == _selected ||
        //                 x.b == nodeComponent && x.a == _selected);
        //
        //             edges.Remove(edge);
        //
        //             _selected = null;
        //             return;
        //         }
        //         CreateEdge(_selected, nodeComponent);
        //         _selected = null;
        //         StartSimulation();
        //     }
        // }
    }
}