using System;

namespace Polymer.UI.Routing
{
    public class NavArgument
    {
        public string Name { get; }
        public Type Type { get; }
        public bool Optional { get; }

        public NavArgument(string name, Type type, bool optional = false)
        {
            Name = name;
            Type = type;
            Optional = optional;
        }
    }
}