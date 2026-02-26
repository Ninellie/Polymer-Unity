using VContainer;
using VContainer.Unity;

namespace Polymer.UI.GraphPage
{
    public class GraphRendererInjector : IInitializable
    {
        private readonly NodesRenderer nodesRenderer;
        private readonly LinksRenderer linksRenderer;
        private readonly IObjectResolver resolver;
        
        public GraphRendererInjector(NodesRenderer nodesRenderer, LinksRenderer linksRenderer, IObjectResolver resolver)
        {
            this.nodesRenderer = nodesRenderer;
            this.linksRenderer = linksRenderer;
            this.resolver = resolver;
        }

        public void Initialize()
        {
            resolver.InjectGameObject(nodesRenderer.gameObject);
            resolver.InjectGameObject(linksRenderer.gameObject);
        }
    }
}