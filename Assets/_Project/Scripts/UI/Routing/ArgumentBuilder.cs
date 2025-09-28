#nullable enable
using System.Collections.Generic;

namespace Polymer.UI.Routing
{
    public class ArgumentBuilder
    {
        private readonly List<NavArgument> _args = new();

        public ArgumentBuilder Int(string name, bool optional = false)
        {
            _args.Add(new NavArgument(name, typeof(int), optional));
            return this;
        }

        public ArgumentBuilder String(string name, bool optional = false)
        {
            _args.Add(new NavArgument(name, typeof(string), optional));
            return this;
        }

        public ArgumentBuilder Bool(string name, bool optional = false)
        {
            _args.Add(new NavArgument(name, typeof(bool), optional));
            return this;
        }

        public List<NavArgument> Build() => _args;
    }
}