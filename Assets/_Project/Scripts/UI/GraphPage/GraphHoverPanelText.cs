using System.Linq;
using Core.Models;

namespace Polymer.UI.GraphPage
{
    /// <summary>
    /// Shared TMP-safe escaping and device labels for graph hover panels.
    /// </summary>
    public static class GraphHoverPanelText
    {
        public static string EscapeForTmp(string value)
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

        public static string DeviceLabel(ApplicationData appData, int deviceId)
        {
            if (appData == null)
            {
                return $"#{deviceId}";
            }

            var d = appData.Devices.FirstOrDefault(x => x.Id == deviceId);
            if (d == null)
            {
                return $"#{deviceId}";
            }

            if (!string.IsNullOrWhiteSpace(d.Display))
            {
                return d.Display.Trim();
            }

            if (!string.IsNullOrWhiteSpace(d.Name))
            {
                return d.Name.Trim();
            }

            return $"#{deviceId}";
        }
    }
}
