using FDLayout;
using Polymer.Core.Input;
using Polymer.UI.Routing;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

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
        [SerializeField] private Scaler scaler;
        [SerializeField] private bool isGeometric = true;
        
        // [SerializeField] [ReadOnly] private NodeComponent _selected;

        public ForceDirectedLayout Layout { get; private set; }

        [Inject] private InputManager _inputManager;
        
        private NodesRenderer _nodesRenderer;
        private LinksRenderer _linksRenderer;
        
        
        
        private void Start()
        {
            factory.CreateNodes();
            
            Layout = new ForceDirectedLayout(factory.Nodes, factory.Connections, isGeometric: isGeometric);
            settings.Init(Layout);
            
            _nodesRenderer = Instantiate(nodesRenderer);
            _nodesRenderer.SetNodes(factory.Nodes);
            
            _linksRenderer = Instantiate(linksRenderer);
            _linksRenderer.SetLinks(factory.Connections);

            _inputManager.OnScrollWheel += UpdateScale;
            _inputManager.OnDrag += UpdateOffset;
            _inputManager.OnDragEnd += OnDragEnd;
            scaler.OnScaleChanged += ApplyScale;
            scaler.OnOffsetChanged += ApplyOffset;
        }

        public void StartAnimation()
        {
            factory.CreateNodes();
        }

        public void Reload()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
        private void UpdateScale(Vector2 delta)
        {
            var cursorWorld = (Vector2)Camera.main.ScreenToWorldPoint(UnityEngine.InputSystem.Pointer.current.position.ReadValue());
            scaler.AdjustTargetScale(delta.y, cursorWorld);
        }

        private void UpdateOffset(Vector2 screenDelta)
        {
            var cam = Camera.main;
            var worldDelta = screenDelta * (2f * cam.orthographicSize / Screen.height);
            scaler.AdjustOffset(worldDelta);
        }

        private void OnDragEnd()
        {
            scaler.ReleaseInertia();
        }
        
        private void ApplyScale(float value)
        {
            _linksRenderer.Scale = value;
            _nodesRenderer.Scale = value;
            _linksRenderer.RecalculateMesh();
            _nodesRenderer.RecalculateMesh();
        }

        private void ApplyOffset(Vector2 offset)
        {
            _nodesRenderer.Offset = offset;
            _linksRenderer.Offset = offset;
            _nodesRenderer.RecalculateMesh();
            _linksRenderer.RecalculateMesh();
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