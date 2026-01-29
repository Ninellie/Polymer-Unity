using System;
using System.Collections.Generic;

namespace Polymer.Services.JsonLoader
{
    [Serializable]
    public class NetworkModel
    {
        public List<Device> devices;
        public List<Connection> connections;
    }
}