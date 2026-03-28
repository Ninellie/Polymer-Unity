using Core.Models;
using UnityEngine;
using VContainer.Unity;

namespace Polymer.Services.JsonLoader
{
    public class JsonDataLoader : IInitializable
    {
        private readonly TextAsset _json;
        private readonly ApplicationData _appData;

        public JsonDataLoader(TextAsset json, ApplicationData appData)
        {
            _json = json;
            _appData = appData;
        }

        public void Initialize()
        {
            _appData.LoadError = null;

            var model = JsonUtility.FromJson<NetworkModel>(_json.text);

            foreach (var device in model.devices)
            {
                var name = string.IsNullOrWhiteSpace(device.name) ? device.display : device.name;
                _appData.Devices.Add(new Device
                {
                    Id = device.id,
                    Name = name,
                    Display = string.IsNullOrWhiteSpace(device.display) || device.display == name
                        ? null
                        : device.display.Trim(),
                    Description = string.IsNullOrWhiteSpace(device.comments) ? null : device.comments.Trim(),
                    Serial = NullIfEmpty(device.serial),
                    AssetTag = NullIfEmpty(device.asset_tag),
                    SiteName = NullIfEmpty(device.site),
                    LocationName = NullIfEmpty(device.location),
                    RackName = NullIfEmpty(device.rack),
                    RackPosition = NullIfEmpty(device.rack_position),
                    Status = NullIfEmpty(device.status),
                    Model = NullIfEmpty(device.model),
                    DeviceTypeDisplay = NullIfEmpty(device.device_type),
                    PrimaryIp4 = NullIfEmpty(device.primary_ip4),
                    PrimaryIp6 = NullIfEmpty(device.primary_ip6),
                    Role = string.IsNullOrEmpty(device.role)
                        ? null
                        : new DeviceRole
                        {
                            Color = device.color,
                            Name = device.role
                        },
                    Manufacturer = string.IsNullOrEmpty(device.manufacturer)
                        ? null
                        : new Manufacturer { Name = device.manufacturer.Trim() }
                });
            }
            
            foreach (var connection in model.connections)
            {
                _appData.Cables.Add(new Cable()
                {
                    FromDeviceId = connection.a,
                    ToDeviceId = connection.b
                });
            }

            _appData.Loaded = true;
        }

        private static string NullIfEmpty(string s) =>
            string.IsNullOrWhiteSpace(s) ? null : s.Trim();
    }
}