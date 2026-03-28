using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Models;
using FDLayout;

namespace Polymer.UI.GraphPage
{
    /// <summary>
    /// Builds TextMesh Pro markup for the graph hover panel from <see cref="ApplicationData"/> and the live <see cref="Node"/> graph.
    /// </summary>
    public class GraphDeviceDetailsFormatter
    {
        private readonly ApplicationData _appData;

        public GraphDeviceDetailsFormatter(ApplicationData appData)
        {
            _appData = appData ?? throw new ArgumentNullException(nameof(appData));
        }

        public string Build(Device device, Node graphNode)
        {
            if (device == null)
            {
                return "<color=#9E9E9E><i>No data for this node</i></color>";
            }

            var sb = new StringBuilder();
            var title = string.IsNullOrWhiteSpace(device.Name) ? $"Device #{device.Id}" : device.Name.Trim();
            sb.Append("<b>").Append(TmpEscape(title)).Append("</b>");
            sb.Append("\n<color=#A0A0A0>id ").Append(device.Id).Append("</color>\n");

            AppendField(sb, "Display", device.Display);
            AppendField(sb, "Status", device.Status);
            AppendField(sb, "Role", device.Role?.Name);
            AppendField(sb, "Manufacturer", device.Manufacturer?.Name);
            AppendField(sb, "Model", device.Model);
            AppendField(sb, "Device type", device.DeviceTypeDisplay);
            AppendField(sb, "Serial", device.Serial);
            AppendField(sb, "Asset tag", device.AssetTag);
            AppendField(sb, "Site", device.SiteName);
            AppendField(sb, "Location", device.LocationName);
            AppendField(sb, "Rack", device.RackName);
            AppendField(sb, "Rack slot", device.RackPosition);
            AppendField(sb, "Primary IPv4", device.PrimaryIp4);
            AppendField(sb, "Primary IPv6", device.PrimaryIp6);
            AppendField(sb, "Description", device.Description);

            AppendNeighborsFromGraph(sb, graphNode);
            AppendNeighborNamesFromCables(sb, graphNode, device.Id);

            return sb.ToString().TrimEnd();
        }

        private void AppendNeighborsFromGraph(StringBuilder sb, Node graphNode)
        {
            if (graphNode == null || graphNode.Links.Count == 0) return;

            var names = graphNode.Links
                .Select(n => ResolveDeviceLabel(n.Id))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(s => s, StringComparer.OrdinalIgnoreCase)
                .ToList();

            sb.Append("\n<color=#B8B8B8>Graph links (").Append(names.Count).Append("):</color>\n");
            AppendLines(sb, names);
        }

        private void AppendNeighborNamesFromCables(StringBuilder sb, Node graphNode, int deviceId)
        {
            var peers = CollectCablePeerLabels(deviceId);
            if (peers.Count == 0) return;

            if (graphNode != null && graphNode.Links.Count > 0)
            {
                var fromGraph = new HashSet<string>(
                    graphNode.Links.Select(n => ResolveDeviceLabel(n.Id)),
                    StringComparer.OrdinalIgnoreCase);
                if (fromGraph.SetEquals(peers)) return;
            }

            var list = peers.OrderBy(s => s, StringComparer.OrdinalIgnoreCase).ToList();
            sb.Append("\n<color=#B8B8B8>Inventory cables (").Append(list.Count).Append(" peers):</color>\n");
            AppendLines(sb, list);
        }

        private HashSet<string> CollectCablePeerLabels(int deviceId)
        {
            var peers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var c in _appData.Cables)
            {
                if (c.FromDeviceId == deviceId)
                {
                    peers.Add(ResolveDeviceLabel(c.ToDeviceId));
                }
                else if (c.ToDeviceId == deviceId)
                {
                    peers.Add(ResolveDeviceLabel(c.FromDeviceId));
                }
            }

            return peers;
        }

        private static void AppendField(StringBuilder sb, string label, string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;

            sb.Append("<color=#B8B8B8>").Append(TmpEscape(label)).Append(": </color>");
            sb.Append(TmpEscape(value.Trim())).Append('\n');
        }

        private static void AppendLines(StringBuilder sb, List<string> lines)
        {
            foreach (var line in lines)
            {
                sb.Append(TmpEscape(line)).Append('\n');
            }
        }

        private string ResolveDeviceLabel(int id)
        {
            var d = _appData.Devices.FirstOrDefault(x => x.Id == id);
            if (d == null)
            {
                return $"#{id}";
            }

            if (!string.IsNullOrWhiteSpace(d.Display))
            {
                return d.Display.Trim();
            }

            if (!string.IsNullOrWhiteSpace(d.Name))
            {
                return d.Name.Trim();
            }

            return $"#{id}";
        }

        private static string TmpEscape(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            return value
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;");
        }
    }
}
