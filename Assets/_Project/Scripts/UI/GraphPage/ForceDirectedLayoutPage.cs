using FDLayout;
using Polymer.UI.Routing;
using UnityEngine;

namespace Polymer.UI.GraphPage
{
    /// <summary>
    /// Applies physics forces to nodes
    /// </summary>
    public class ForceDirectedLayoutPage : PageBase
    {
        [SerializeField] private GraphFactory factory;
        [SerializeField] private NodesRenderer nodesRenderer;
        [SerializeField] private LinksRenderer linksRenderer;
        [SerializeField] private GraphSettings settings;
        [SerializeField] private bool isGeometric = true;
        
        // [SerializeField] [ReadOnly] private NodeComponent _selected;

        public ForceDirectedLayout Layout { get; private set; }
        
        private NodesRenderer _nodesRenderer;
        private LinksRenderer _linksRenderer;
        
        private void Start()
        {
            StartCoroutine(factory.CreateNodes());
            
            Layout = new ForceDirectedLayout(factory.Nodes, factory.Connections, isGeometric: isGeometric);
            settings.Init(Layout);
            
            _nodesRenderer = Instantiate(nodesRenderer);
            _nodesRenderer.SetNodes(factory.Nodes);

            _linksRenderer = Instantiate(linksRenderer);
            _linksRenderer.SetLinks(factory.Connections);
        }

        private void Update()
        {
            Layout.Tick(Time.deltaTime);
            Layout.IsGeometric = isGeometric;
            _nodesRenderer.IsRendering = Layout.IsColliding;
            _linksRenderer.IsRendering = Layout.IsColliding;
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