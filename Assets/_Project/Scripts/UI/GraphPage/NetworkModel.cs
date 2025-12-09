using System;
using System.Collections.Generic;

namespace UI.DevicePage
{
    [Serializable]
    public class NetworkModel
    {
        public List<Device> devices;
        public List<Connection> connections;
    }
}