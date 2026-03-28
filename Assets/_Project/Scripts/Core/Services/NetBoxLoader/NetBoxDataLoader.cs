using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using Core.Models;
using UnityEngine;
using VContainer.Unity;

namespace Polymer.Services.NetBoxLoader
{
    public class NetBoxDataLoader : IInitializable
    {
        private const string Url = "http://localhost:8000";
        private const string Token = "3b4021bc664b8702e454f4e510da1eb2d80d14de";

        private readonly ApplicationData _appData;

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
            _appData.Loaded = false;
            _appData.LoadError = null;
            _appData.Devices.Clear();
            _appData.Cables.Clear();

            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", Token);

                var devicesUrl = $"{Url}/api/dcim/devices/?limit=0";
                var devicesTask = client.GetStringAsync(devicesUrl);
                var cablesTask = client.GetStringAsync($"{Url}/api/dcim/cables/?limit=0");
                var rolesTask = client.GetStringAsync($"{Url}/api/dcim/device-roles/?limit=0");

                await Task.WhenAll(devicesTask, cablesTask, rolesTask);

                var devicesData = DeserializeOrThrow<NetBoxResponse<DeviceDto>>(devicesTask.Result, "devices");
                var cablesData = DeserializeOrThrow<NetBoxResponse<CableDto>>(cablesTask.Result, "cables");
                var rolesData = DeserializeOrThrow<NetBoxResponse<RoleDto>>(rolesTask.Result, "device-roles");

                var rolesDict = new Dictionary<int, RoleDto>();
                if (rolesData.Results != null)
                {
                    foreach (var role in rolesData.Results)
                    {
                        rolesDict[role.Id] = role;
                    }
                }

                if (devicesData.Results != null)
                {
                    foreach (var item in devicesData.Results)
                    {
                        var parsedRole = ParseRoleToken(item.Role);
                        if (parsedRole != null && rolesDict.TryGetValue(parsedRole.Id, out var fullRole))
                        {
                            item.RoleResolved = fullRole;
                        }
                        else
                        {
                            item.RoleResolved = parsedRole;
                        }
                    }
                }

                if (devicesData.Results != null)
                {
                    foreach (var item in devicesData.Results)
                    {
                        var effectiveRole = item.RoleResolved;
                        var roleColor = effectiveRole?.Color;
                        if (!string.IsNullOrEmpty(roleColor) && !roleColor.StartsWith("#"))
                        {
                            roleColor = "#" + roleColor;
                        }

                        var rackPos = FormatRackSlot(PositionValue(item.Position), FaceLabel(item.Face));
                        var resolvedName = item.Name ?? item.Display;

                        ParseDeviceType(item.DeviceType, out var model, out var typeDisplay, out var manufacturer);

                        _appData.Devices.Add(new Device
                        {
                            Id = item.Id,
                            Name = resolvedName,
                            Display = HasDistinctDisplay(resolvedName, item.Display) ? item.Display.Trim() : null,
                            Description = string.IsNullOrWhiteSpace(item.Comments) ? null : item.Comments.Trim(),
                            Serial = NullIfEmpty(item.Serial),
                            AssetTag = NullIfEmpty(item.AssetTag),
                            Model = model,
                            DeviceTypeDisplay = typeDisplay,
                            Status = StatusLabel(item.Status),
                            SiteName = NamedFromToken(item.Site),
                            LocationName = NamedFromToken(item.Location),
                            RackName = NamedFromToken(item.Rack),
                            RackPosition = rackPos,
                            PrimaryIp4 = IpFromToken(item.PrimaryIp4),
                            PrimaryIp6 = IpFromToken(item.PrimaryIp6),
                            Role = effectiveRole == null
                                ? null
                                : new DeviceRole
                                {
                                    Id = effectiveRole.Id,
                                    Name = effectiveRole.Name,
                                    Color = roleColor
                                },
                            Manufacturer = manufacturer
                        });
                    }
                }

                if (cablesData.Results != null)
                {
                    foreach (var cable in cablesData.Results)
                    {
                        var idA = GetDeviceId(cable.ATerminations);
                        var idB = GetDeviceId(cable.BTerminations);

                        if (idA.HasValue && idB.HasValue)
                        {
                            _appData.Cables.Add(new Cable { FromDeviceId = idA.Value, ToDeviceId = idB.Value });
                        }
                    }
                }

                _appData.Loaded = true;
                Debug.Log($"[NetBoxDataLoader] Loaded {_appData.Devices.Count} devices, {_appData.Cables.Count} cables.");
            }
            catch (Exception ex)
            {
                _appData.LoadError = ex.ToString();
                _appData.Loaded = false;
                _appData.Devices.Clear();
                _appData.Cables.Clear();
                Debug.LogError("[NetBoxDataLoader] Failed to load data from NetBox. See ApplicationData.LoadError and exception below.");
                Debug.LogException(ex);
            }
        }

        private static T DeserializeOrThrow<T>(string json, string endpointLabel)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json)
                       ?? throw new InvalidOperationException($"NetBox {endpointLabel}: empty deserialize result.");
            }
            catch (JsonException jx)
            {
                var snippet = json == null ? "null" : json.Substring(0, Math.Min(400, json.Length));
                throw new InvalidOperationException($"NetBox {endpointLabel}: JSON parse failed. Body starts with: {snippet}", jx);
            }
        }

        private static int? GetDeviceId(List<TerminationDto> terminations)
        {
            if (terminations == null || terminations.Count == 0)
            {
                return null;
            }

            return terminations[0].Object?.Device?.Id;
        }

        private class NetBoxResponse<T>
        {
            [JsonProperty("results")] public List<T> Results { get; set; }
        }

        private static string NullIfEmpty(string s) =>
            string.IsNullOrWhiteSpace(s) ? null : s.Trim();

        private static bool HasDistinctDisplay(string resolvedName, string display)
        {
            if (string.IsNullOrWhiteSpace(display) || string.IsNullOrWhiteSpace(resolvedName))
            {
                return false;
            }

            return !string.Equals(display.Trim(), resolvedName.Trim(), StringComparison.Ordinal);
        }

        /// <summary>NetBox 4 <c>face</c> is often <c>{ "value", "label" }</c>; older APIs use a plain string.</summary>
        private static string FaceLabel(JToken token)
        {
            if (token == null || token.Type == JTokenType.Null)
            {
                return null;
            }

            if (token.Type == JTokenType.String)
            {
                return NullIfEmpty(token.ToString());
            }

            if (token.Type == JTokenType.Object)
            {
                return NullIfEmpty(token["label"]?.ToString() ?? token["value"]?.ToString());
            }

            return null;
        }

        private static string NamedFromToken(JToken token)
        {
            if (token == null || token.Type == JTokenType.Null)
            {
                return null;
            }

            if (token.Type == JTokenType.Object)
            {
                return NullIfEmpty(token["display"]?.ToString() ?? token["name"]?.ToString());
            }

            return null;
        }

        private static void ParseDeviceType(JToken token, out string model, out string typeDisplay, out Manufacturer manufacturer)
        {
            model = null;
            typeDisplay = null;
            manufacturer = null;
            if (token == null || token.Type != JTokenType.Object) return;

            model = NullIfEmpty(token["model"]?.ToString());
            typeDisplay = NullIfEmpty(token["display"]?.ToString());
            var m = token["manufacturer"];
            if (m != null && m.Type == JTokenType.Object)
            {
                var name = NullIfEmpty(m["display"]?.ToString() ?? m["name"]?.ToString());
                if (name != null)
                {
                    manufacturer = new Manufacturer { Name = name };
                }
            }
        }

        private static string StatusLabel(JToken token)
        {
            if (token == null || token.Type == JTokenType.Null)
            {
                return null;
            }

            if (token.Type == JTokenType.String)
            {
                return NullIfEmpty(token.ToString());
            }

            return NullIfEmpty(token["label"]?.ToString() ?? token["value"]?.ToString());
        }

        private static string IpFromToken(JToken token)
        {
            if (token == null || token.Type == JTokenType.Null)
            {
                return null;
            }

            if (token.Type == JTokenType.String)
            {
                return NullIfEmpty(token.ToString());
            }

            if (token.Type == JTokenType.Integer)
            {
                return null;
            }

            return NullIfEmpty(token["address"]?.ToString() ?? token["display"]?.ToString());
        }

        private static float? PositionValue(JToken token)
        {
            if (token == null || token.Type == JTokenType.Null)
            {
                return null;
            }

            if (token.Type == JTokenType.Integer || token.Type == JTokenType.Float)
            {
                return token.Value<float>();
            }

            if (token.Type == JTokenType.String && float.TryParse(token.ToString(), out var f))
            {
                return f;
            }

            return null;
        }

        private static string FormatRackSlot(float? position, string face)
        {
            if (!position.HasValue)
            {
                return null;
            }

            var u = position.Value;
            var uLabel = Math.Abs(u - (float)Math.Round(u)) < 0.0001f
                ? ((int)Math.Round(u)).ToString()
                : u.ToString(System.Globalization.CultureInfo.InvariantCulture);
            var f = NullIfEmpty(face);
            return f == null ? $"U{uLabel}" : $"U{uLabel} - {f}";
        }

        private static RoleDto ParseRoleToken(JToken token)
        {
            if (token == null || token.Type == JTokenType.Null)
            {
                return null;
            }

            if (token.Type == JTokenType.Object)
            {
                var idTok = token["id"];
                var id = idTok != null && idTok.Type != JTokenType.Null ? idTok.Value<int>() : 0;
                return new RoleDto
                {
                    Id = id,
                    Name = token["name"]?.ToString(),
                    Color = token["color"]?.ToString()
                };
            }

            return null;
        }

        private class DeviceDto
        {
            [JsonProperty("id")] public int Id { get; set; }
            [JsonProperty("name")] public string Name { get; set; }
            [JsonProperty("display")] public string Display { get; set; }
            [JsonProperty("serial")] public string Serial { get; set; }
            [JsonProperty("asset_tag")] public string AssetTag { get; set; }
            [JsonProperty("comments")] public string Comments { get; set; }
            [JsonProperty("role")] public JToken Role { get; set; }

            [JsonIgnore] public RoleDto RoleResolved { get; set; }
            [JsonProperty("device_type")] public JToken DeviceType { get; set; }
            [JsonProperty("site")] public JToken Site { get; set; }
            [JsonProperty("location")] public JToken Location { get; set; }
            [JsonProperty("rack")] public JToken Rack { get; set; }
            [JsonProperty("position")] public JToken Position { get; set; }
            [JsonProperty("face")] public JToken Face { get; set; }
            [JsonProperty("status")] public JToken Status { get; set; }
            [JsonProperty("primary_ip4")] public JToken PrimaryIp4 { get; set; }
            [JsonProperty("primary_ip6")] public JToken PrimaryIp6 { get; set; }
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
