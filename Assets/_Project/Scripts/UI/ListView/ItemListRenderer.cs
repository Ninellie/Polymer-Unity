using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ListView
{
    public class ItemListRenderer : MonoBehaviour
    {
        [SerializeField] private ListItem viewItemPrefab;
        
        [SerializeField] private List<ListItem> viewItems;
        [SerializeField] private RectTransform content;
        [SerializeField] private VerticalLayoutGroup contentLayoutGroup;
        [SerializeField] private ToggleGroup toggleGroup;
        
        private IItemListDataProvider _dataProvider;
        
        public void CreateFromDataProvider(IItemListDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
            
            if (viewItems != null)
            {
                foreach (var item in viewItems)
                {
                    Destroy(item.gameObject);
                }
            }
            
            viewItems = new List<ListItem>();
            
            for (var i = 0; i < _dataProvider.ItemsCount; i++)
            {
                var title = _dataProvider.GetTitle(i);
                var subTitle = _dataProvider.GetSubtitle(i);
                AddItem(i, title, subTitle);
            }
        }
        
        public void AddItemAtEnd(string title, string subTitle)
        {
            var itemIndex = viewItems.Count + 1;
            var item = Instantiate(viewItemPrefab, content);
            item.SetId(itemIndex).SetTitle(title).SetSubTitle(subTitle);
            item.OnItemSelected += _ => _dataProvider.OnItemSelected?.Invoke(itemIndex);
            toggleGroup.RegisterToggle(item.Toggle);
            item.Toggle.group = toggleGroup;
            viewItems.Add(item);
        }
        
        private void AddItem(int itemIndex, string title, string subTitle)
        {
            var item = Instantiate(viewItemPrefab, content);
            item.SetId(itemIndex).SetTitle(title).SetSubTitle(subTitle);
            item.OnItemSelected += _ => _dataProvider.OnItemSelected?.Invoke(itemIndex);
            toggleGroup.RegisterToggle(item.Toggle);
            item.Toggle.group = toggleGroup;
            viewItems.Add(item);
        }
    }
}