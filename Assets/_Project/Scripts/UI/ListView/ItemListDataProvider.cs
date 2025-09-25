using System;
using System.Collections.Generic;

namespace UI.ListView
{
    public class ItemListDataProvider<T> : IItemListDataProvider 
    {
        public int ItemsCount => _items.Count;
        public Action<int> OnItemSelected { get; set; }
        
        private readonly List<T> _items;
        private readonly Func<T, string> _getTitle; 
        private readonly Func<T, string> _getSubtitle;

        public ItemListDataProvider(List<T> items, Func<T, string> getTitle, Func<T, string> getSubtitle)
        {
            _items = items;
            _getTitle = getTitle;
            _getSubtitle = getSubtitle;
        }
        
        public string GetTitle(int index)
        {
            return _getTitle(_items[index]);
        }

        public string GetSubtitle(int index)
        {
            return _getSubtitle(_items[index]);
        }
    }
}