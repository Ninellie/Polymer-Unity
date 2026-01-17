using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UI.DevicePage;
using System.Net.Http.Headers;

public class NetBoxDataProvider
{
    private const string Url = "http://localhost:8000";
    private const string Token = "3b4021bc664b8702e454f4e510da1eb2d80d14de";

    public static async Task<NetworkModel> GetNetworkModel()
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", Token);

        // Получаем устройства и кабели параллельно для ускорения
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
        
        var model = new NetworkModel
        {
            devices = new List<Device>(),
            connections = new List<Connection>()
        };
        
        foreach (var item in devicesData.Results.Where(item => item.Role != null))
        {
            item.Role = rolesDict[item.Role.Id];
        }

        // Маппинг устройств
        foreach (var item in devicesData.Results)
        {
            model.devices.Add(new Device
            {
                id = item.Id,
                name = item.Name ?? item.Display,
                role = item.Role?.Name,
                color = "#" + item.Role?.Color
            });
        }

        // Маппинг соединений
        foreach (var cable in cablesData.Results)
        {
            var idA = GetDeviceId(cable.ATerminations);
            var idB = GetDeviceId(cable.BTerminations);

            if (idA.HasValue && idB.HasValue)
            {
                model.connections.Add(new Connection { a = idA.Value, b = idB.Value });
            }
        }

        return model;
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