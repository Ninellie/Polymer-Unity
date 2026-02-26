using System.Collections.Generic;
using Core.Models;
using FDLayout;
using Polymer.Core.Input;
using Polymer.Services.JsonLoader;
using Polymer.Services.NetBoxLoader;
using Polymer.UI;
using Polymer.UI.GraphPage;
using Polymer.UI.Routing;
using VContainer;
using VContainer.Unity;
using UnityEngine;

namespace Polymer
{
    public class BootstrapInstaller : LifetimeScope
    {
        [SerializeField] private PageRoutingSettings pageRoutingSettings;
        [SerializeField] private Transform pageRoot;
        [SerializeField] private bool useJsonFile;
        [SerializeField] private TextAsset jsonFile;
        
        [SerializeField] private NodesRenderer nodesRendererPrefab;
        [SerializeField] private LinksRenderer linksRendererPrefab;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<InputManager>().AsSelf();
            
            builder.Register<ApplicationData>(Lifetime.Singleton);
            
            if (useJsonFile)
            {
                builder.RegisterInstance(jsonFile);
                builder.RegisterEntryPoint<JsonDataLoader>();
            }
            else
            {
                builder.RegisterEntryPoint<NetBoxDataLoader>();
            }
            
            // Graph page
            var nodes = new List<Node>();
            var links = new List<(Node a, Node b)>();
            var layout = new ForceDirectedLayout(nodes, links, isGeometric: true);
            builder.RegisterInstance(Camera.main);
            builder.RegisterInstance(nodes);
            builder.RegisterInstance(links);
            builder.RegisterInstance(layout);
            builder.RegisterEntryPoint<GraphRendererInjector>();
            var graphRenderer = new GameObject();
            graphRenderer.name = "Graph Renderer";
            var nodesRenderer = Instantiate(nodesRendererPrefab, graphRenderer.transform);
            builder.RegisterInstance(nodesRenderer);
            var linksRenderer = Instantiate(linksRendererPrefab, graphRenderer.transform);
            builder.RegisterInstance(linksRenderer);
            
            // Page routing
            var routeTable = CreateRouteTable();
            builder.RegisterInstance(routeTable);
            builder.RegisterEntryPoint<PageRouter>().AsSelf();
            builder.Register<PageResolver>(Lifetime.Singleton);
            builder.Register<PageVisitHistory>(Lifetime.Singleton);
            builder.RegisterInstance(pageRoutingSettings);
            
            builder.RegisterInstance(pageRoot).Keyed("PageRoot");
        }

        private static RouteTable CreateRouteTable()
        {
            var routeTable = new RouteTable();
            var routeBuilder = new RouteBuilder();
            AddDeviceRolesPages(routeBuilder).Build(routeTable);
            return routeTable;
        }

        private static RouteBuilder AddDeviceRolesPages(RouteBuilder routeBuilder)
        {
            routeBuilder.Add("Devices/Graph", "Pages/Devices/Graph");
            
            return routeBuilder;
        }
    }
}
