using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
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
        
        [Header("Damping parameters")]
        [SerializeField] [ReadOnly] private float damping;
        [SerializeField] private float baseDamping;
        [SerializeField] private float dampingDecreasePerSecond = 3f;
        
        [SerializeField] private float overlapRepulsion;

        [SerializeField] [ReadOnly] private bool _isSimulated;
        
        [SerializeField] [ReadOnly] private Node _selected;
        
        private bool _isHighlightingAllowed;
        
        public void StartSimulation()
        {
            damping = baseDamping;
            _isSimulated = true;
        }
        
        private void Start()
        {
            nodes = factory.Nodes;
            edges = factory.Edges;
            StartSimulation();
            _isHighlightingAllowed = true;
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
            foreach (var node in nodes)
            {
                if (node.isDragged) continue;
                node.velocity = node.force * dt;
                node.velocity *= damping;
                node.velocity = Vector2.ClampMagnitude(node.velocity, maxVelocity);
                node.RectTransform.anchoredPosition += node.velocity;
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
                    var force = delta / distance * repulsionPower;

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
                var transition = delta.normalized * (displacement * springPower);
                var totalWeight = edge.a.weight + edge.b.weight;
                
                var aTransition = transition * (edge.b.weight / totalWeight);
                var bTransition = transition * (edge.a.weight / totalWeight);
                
                if (!edge.a.isDragged)
                {
                    edge.a.force += aTransition;
                }

                if (!edge.b.isDragged)
                {
                    edge.b.force -= bTransition;   
                }
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

        public void HoverNode(Node node)
        {
            if (!_isHighlightingAllowed) return;
            var selectedNodes = new List<Node>(node.linkedNodes) { node };
            var nonSelectedNodes = nodes.Except(selectedNodes);
            foreach (var nonSelectedNode in nonSelectedNodes)
            {
                nonSelectedNode.Fade();
            }
        }

        public void UnhoverNode(Node node)
        {
            if (!_isHighlightingAllowed) return;
            var selectedNodes = new List<Node>(node.linkedNodes) { node };
            var nonSelectedNodes = nodes.Except(selectedNodes);
            foreach (var nonSelectedNode in nonSelectedNodes)
            {
                nonSelectedNode.UndoFade();
            }
        }
        
        public void SetSelectedNode(Node node)
        {
            if (_selected == null)
            {
                _selected = node;
            }
            else
            {
                // add edge
                if (node.linkedNodes.Contains(_selected))
                {
                    node.linkedNodes.Remove(_selected);
                    _selected.linkedNodes.Remove(node);

                    var edge = edges.Find(x => 
                        x.a == node && x.b == _selected ||
                        x.b == node && x.a == _selected);

                    edges.Remove(edge);

                    _selected = null;
                    return;
                }
                CreateEdge(_selected, node);
                _selected = null;
                StartSimulation();
            }
        }
        
        private void CreateEdge(Node a, Node b)
        {
            var edge = new Edge();
            edge.a = a;
            edge.b = b;
            edges.Add(edge);
            a.linkedNodes.Add(b);
            b.linkedNodes.Add(a);
        }
    }
}