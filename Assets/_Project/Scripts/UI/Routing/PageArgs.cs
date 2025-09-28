using System;
using System.Collections.Generic;

namespace Polymer.UI.Routing
{
    /// <summary>
    /// Аргументы страницы.
    /// </summary>
    public class PageArgs
    {
        private readonly Dictionary<string, object> _data;

        public PageArgs(Dictionary<string, object> data)
        {
            _data = data;
        }

        public T Get<T>(string key)
        {
            if (!_data.TryGetValue(key, out var value))
                throw new KeyNotFoundException($"PageArgs: key '{key}' not found");

            if (value is T tValue)
                return tValue;

            throw new InvalidCastException($"PageArgs: value for key '{key}' is not of type {typeof(T).Name}");
        }

        public bool TryGet<T>(string key, out T value)
        {
            if (_data.TryGetValue(key, out var raw) && raw is T casted)
            {
                value = casted;
                return true;
            }

            value = default;
            return false;
        }

        public bool Has(string key) => _data.ContainsKey(key);
    }
}