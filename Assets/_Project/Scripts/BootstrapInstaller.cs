using Core.Models;
using Core.Services;
using Polymer.UI;
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
        
        protected override void Configure(IContainerBuilder builder)
        {
            // Data access object
            builder.Register<ApplicationDataProvider>(Lifetime.Singleton);
            builder.Register<DeviceRoleDataService>(Lifetime.Singleton);
            builder.RegisterEntryPoint<JsonDataService>().AsSelf();
            builder.RegisterEntryPoint<DataInitializationService>();
            builder.RegisterEntryPoint<DataTestService>();
            
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
