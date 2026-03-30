using System.Text;
using Core.Models;

namespace Polymer.UI.GraphPage
{
    /// <summary>
    /// Builds TextMesh Pro markup for the device hover panel (excluding cable endpoint listing).
    /// </summary>
    public class GraphDeviceDetailsFormatter
    {
        public string Build(Device device)
        {
            if (device == null)
            {
                return "<color=#9E9E9E><i>No data for this node</i></color>";
            }

            var sb = new StringBuilder();
            var title = string.IsNullOrWhiteSpace(device.Name) ? $"Device #{device.Id}" : device.Name.Trim();
            sb.Append("<b>").Append(GraphHoverPanelText.EscapeForTmp(title)).Append("</b>");
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

            return sb.ToString().TrimEnd();
        }

        private static void AppendField(StringBuilder sb, string label, string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;

            sb.Append("<color=#B8B8B8>").Append(GraphHoverPanelText.EscapeForTmp(label)).Append(": </color>");
            sb.Append(GraphHoverPanelText.EscapeForTmp(value.Trim())).Append('\n');
        }
    }
}
