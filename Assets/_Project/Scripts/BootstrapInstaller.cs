using Core.Models;
using Core.Services;
using Polymer.UI;
using Polymer.UI.Routing;
using VContainer;
using VContainer.Unity;
using UI.DevicePage;
using UnityEngine;

namespace Polymer
{
    public class BootstrapInstaller : LifetimeScope
    {
        [SerializeField] private PageRoutingSettings pageRoutingSettings;
        [SerializeField] private Transform pageRoot;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<ApplicationDataProvider>(Lifetime.Singleton);
            builder.Register<DeviceRoleDataService>(Lifetime.Singleton);
            builder.RegisterEntryPoint<JsonDataService>().AsSelf();
            builder.RegisterEntryPoint<DataInitializationService>();
            builder.RegisterEntryPoint<DataTestService>();

            var deviceRolesPage = FindFirstObjectByType<DeviceRolesPage>();
            builder.RegisterComponent(deviceRolesPage);
            
            builder.Register<PageRouter>(Lifetime.Singleton);
            builder.Register<PageResolver>(Lifetime.Singleton);
            builder.Register<PageVisitHistory>(Lifetime.Singleton);
            builder.RegisterInstance(pageRoutingSettings);
            builder.RegisterComponent(typeof(PageBase));
            builder.RegisterInstance(pageRoot).Keyed("PageRoot");
            var routeTable = new RouteTable();
            CreateRouteTable(builder, routeTable);
        }

        private void CreateRouteTable(IContainerBuilder builder, RouteTable routeTable)
        {
            var routeBuilder = new RouteBuilder();
            AddDeviceRolesPages(routeBuilder).Build(routeTable);
            builder.RegisterInstance(routeBuilder);
        }

        private RouteBuilder AddDeviceRolesPages(RouteBuilder routeBuilder)
        {
            routeBuilder.Add("DeviceRoles", "Pages/DeviceRoles/List");
            routeBuilder.Add("DeviceRoles/Create/{id}", "Pages/DeviceRoles/Create");
            
            return routeBuilder;
        }
    }
}
