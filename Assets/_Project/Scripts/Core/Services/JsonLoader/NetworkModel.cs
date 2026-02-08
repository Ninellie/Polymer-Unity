using System;
using System.Collections.Generic;

namespace Polymer.Services.JsonLoader
{
    [Serializable]
    public class NetworkModel
    {
        public List<JsonTestDevice> devices;
        public List<JsonTestConnection> connections;
    }
}