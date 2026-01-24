using FDLayout;
using Polymer.UI.Routing;
using TriInspector;
using UnityEngine;

namespace UI.DevicePage
{
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

        private ForceDirectedLayout _layout;
        
        private void Start()
        {
            _layout = new ForceDirectedLayout(
                Graph.Instance.Nodes,
                Graph.Instance.Connections,
                friction: dampingDecreasePerSecond,
                charge: repulsionPower);
        }

        public void StartSimulation()
        {
            _layout.Start();
            damping = baseDamping;
            _isSimulated = true;
        }

        private void Update()
        {
            _layout.Tick(Time.deltaTime);
        }
        
        public override void OnPageInit(PageArgs args)
        {
        }
        
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