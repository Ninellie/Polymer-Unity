using System.Collections.Generic;
using TriInspector;
using UnityEngine;

namespace UI.ListView
{
    public class ExampleDataModelPage : MonoBehaviour
    {
        [SerializeField] private ItemListRenderer itemListRendererPrefab;
        [SerializeField] private List<ExampleDataModel> dataSet;
        
        private ItemListRenderer _itemListRenderer;
        private ItemListDataProvider<ExampleDataModel> _itemListDataProvider;
        
        [Button]
        public void Init()
        {
            _itemListDataProvider = new ItemListDataProvider<ExampleDataModel>(dataSet, model => model.name, model => model.description);
            
            if (_itemListRenderer != null)
            {
                Destroy(_itemListRenderer.gameObject);
            }
            _itemListRenderer = Instantiate(itemListRendererPrefab, transform);
            _itemListRenderer.CreateFromDataProvider(_itemListDataProvider);
        }
    }
}