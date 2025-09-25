using Core.Models;
using Core.Services;
using VContainer;
using VContainer.Unity;
using UI.DevicePage;

namespace Polymer
{
    public class BootstrapInstaller : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<ApplicationDataProvider>(Lifetime.Singleton);
            builder.Register<DeviceRoleDataService>(Lifetime.Singleton);
            builder.RegisterEntryPoint<JsonDataService>().AsSelf();
            builder.RegisterEntryPoint<DataInitializationService>();
            builder.RegisterEntryPoint<DataTestService>();

            var deviceRolesPage = FindFirstObjectByType<DeviceRolesPage>();
            builder.RegisterComponent(deviceRolesPage);
        }
    }
}
