using System;
using System.Collections.Generic;
using System.Text;
using Core.Models;

namespace Polymer.UI.GraphPage
{
    /// <summary>
    /// Builds TextMesh Pro lines for cable endpoints (local port, remote device, remote port), separated by ASCII arrows.
    /// </summary>
    public class GraphConnectionDetailsFormatter
    {
        private readonly ApplicationData _appData;

        public GraphConnectionDetailsFormatter(ApplicationData appData)
        {
            _appData = appData ?? throw new ArgumentNullException(nameof(appData));
        }

        public string Build(int deviceId)
        {
            var rows = new List<string>();
            foreach (var cable in _appData.Cables)
            {
                if (cable.FromDeviceId == deviceId)
                {
                    rows.Add(FormatTriple(
                        PortLabel(cable.FromPortName, cable.FromPortId),
                        GraphHoverPanelText.DeviceLabel(_appData, cable.ToDeviceId),
                        PortLabel(cable.ToPortName, cable.ToPortId)));
                }
                else if (cable.ToDeviceId == deviceId)
                {
                    rows.Add(FormatTriple(
                        PortLabel(cable.ToPortName, cable.ToPortId),
                        GraphHoverPanelText.DeviceLabel(_appData, cable.FromDeviceId),
                        PortLabel(cable.FromPortName, cable.FromPortId)));
                }
            }

            if (rows.Count == 0)
            {
                return string.Empty;
            }

            rows.Sort(StringComparer.OrdinalIgnoreCase);
            var sb = new StringBuilder();
            foreach (var row in rows)
            {
                sb.Append(row).Append('\n');
            }

            return sb.ToString().TrimEnd();
        }

        private static string FormatTriple(string localPort, string remoteDevice, string remotePort)
        {
            return $"{GraphHoverPanelText.EscapeForTmp(localPort)} -> {GraphHoverPanelText.EscapeForTmp(remoteDevice)} -> {GraphHoverPanelText.EscapeForTmp(remotePort)}";
        }

        private static string PortLabel(string portName, int portId)
        {
            if (!string.IsNullOrWhiteSpace(portName))
            {
                return portName.Trim();
            }

            if (portId != 0)
            {
                return $"#{portId}";
            }

            return "—";
        }
    }
}
