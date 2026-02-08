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
            var model = JsonUtility.FromJson<NetworkModel>(_json.text);

            foreach (var device in model.devices)
            {
                _appData.Devices.Add(new Device()
                {
                    Id = device.id,
                    Name = device.name,
                    Role = new DeviceRole()
                    {
                        Color = device.color,
                        Name = device.role
                    }
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
    }
}