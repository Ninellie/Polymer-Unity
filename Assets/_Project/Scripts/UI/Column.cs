using System;
using System.Collections.Generic;

namespace UI
{
    [Serializable]
    public class Column
    {
        public string name;
        public float width;
        public List<TableElement> elements = new();
    }
}