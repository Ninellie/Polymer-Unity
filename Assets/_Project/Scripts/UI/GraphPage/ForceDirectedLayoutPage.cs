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
        [SerializeField] private bool isGeometric = true;

        [Inject] private ForceDirectedLayout _layout;
        [Inject] private NodesRenderer _nodesRenderer;
        [Inject] private LinksRenderer _linksRenderer;
        [Inject] private InputManager _inputManager;
        [Inject] private Camera _mainCamera;

        public void Reload()
        {
            //todo prefab reload, not scene reload
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
        private void Update()
        {
            _layout.Tick(Time.deltaTime);
            _layout.IsGeometric = isGeometric;
            _nodesRenderer.IsRendering = _layout.IsColliding;
            _linksRenderer.IsRendering = _layout.IsColliding;
        }
        
        public override void OnPageInit(PageArgs args)
        {
        }
    }
}