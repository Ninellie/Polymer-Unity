using System;

namespace Polymer.Services.JsonLoader
{
    [Serializable]
    public class JsonTestDevice
    {
        public int id;
        public string name;
        public string role;
        public string color;
        public string display;
        public string serial;
        public string asset_tag;
        public string site;
        public string location;
        public string rack;
        public string rack_position;
        public string status;
        public string comments;
        public string manufacturer;
        public string model;
        public string device_type;
        public string primary_ip4;
        public string primary_ip6;
    }
}