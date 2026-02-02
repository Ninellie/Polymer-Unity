using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Core.Models;
using VContainer.Unity;

namespace Polymer.Services.NetBoxLoader
{
    public class NetBoxDataLoader : IInitializable
    {
        private const string Url = "http://localhost:8000";
        private const string Token = "3b4021bc664b8702e454f4e510da1eb2d80d14de";

        private ApplicationData _appData;

        public NetBoxDataLoader(ApplicationData appData)
        {
            _appData = appData;
        }

        public void Initialize()
        {
            _ = LoadAsync();
        }
        
        private async Task LoadAsync()
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", Token);

            var devicesTask = client.GetStringAsync($"{Url}/api/dcim/devices/?limit=0");
            var cablesTask = client.GetStringAsync($"{Url}/api/dcim/cables/?limit=0");
            var rolesTask = client.GetStringAsync($"{Url}/api/dcim/device-roles/?limit=0");

            await Task.WhenAll(devicesTask, cablesTask);

            var devicesData = JsonConvert.DeserializeObject<NetBoxResponse<DeviceDto>>(devicesTask.Result);
            var cablesData = JsonConvert.DeserializeObject<NetBoxResponse<CableDto>>(cablesTask.Result);
            var rolesData = JsonConvert.DeserializeObject<NetBoxResponse<RoleDto>>(rolesTask.Result);

            var rolesDict = new Dictionary<int, RoleDto>();

            foreach (var role in rolesData.Results)
            {
                rolesDict[role.Id] = role;
            }

            foreach (var item in devicesData.Results.Where(item => item.Role != null))
            {
                item.Role = rolesDict[item.Role.Id];
            }

            // Маппинг устройств
            foreach (var item in devicesData.Results)
            {
                _appData.Devices.Add(new Device
                {
                    Id = item.Id,
                    Name = item.Name ?? item.Display,
                    Role = new DeviceRole
                    {
                        Name = item.Role?.Name,
                        Color = "#" + item.Role?.Color
                    },
                });
            }

            // Маппинг соединений
            foreach (var cable in cablesData.Results)
            {
                var idA = GetDeviceId(cable.ATerminations);
                var idB = GetDeviceId(cable.BTerminations);

                if (idA.HasValue && idB.HasValue)
                {
                    _appData.Cables.Add(new Cable { FromDeviceId = idA.Value, ToDeviceId = idB.Value });
                }
            }

            _appData.Loaded = true;
        }

        private static int? GetDeviceId(List<TerminationDto> terminations)
        {
            if (terminations == null || terminations.Count == 0) return null;
            return terminations[0].Object?.Device?.Id;
        }

        private class NetBoxResponse<T>
        {
            [JsonProperty("results")] public List<T> Results { get; set; }
        }

        private class DeviceDto
        {
            [JsonProperty("id")] public int Id { get; set; }
            [JsonProperty("name")] public string Name { get; set; }
            [JsonProperty("display")] public string Display { get; set; }
            [JsonProperty("role")] public RoleDto Role { get; set; }
        }

        private class RoleDto
        {
            [JsonProperty("id")] public int Id { get; set; }
            [JsonProperty("name")] public string Name { get; set; }
            [JsonProperty("color")] public string Color { get; set; }
        }

        private class CableDto
        {
            [JsonProperty("a_terminations")] public List<TerminationDto> ATerminations { get; set; }
            [JsonProperty("b_terminations")] public List<TerminationDto> BTerminations { get; set; }
        }

        private class TerminationDto
        {
            [JsonProperty("object")] public TermObjectDto Object { get; set; }
        }

        private class TermObjectDto
        {
            [JsonProperty("device")] public DeviceRefDto Device { get; set; }
        }

        private class DeviceRefDto
        {
            [JsonProperty("id")] public int Id { get; set; }
        }
    }
}