using System.Collections.Generic;
using Core.Models;
using Core.Services;
using Polymer.UI.Routing;
using TMPro;
using UI.ListView;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace UI.DevicePage
{
    public class DeviceRolesPage : PageBase
    {
        [Header("Inner components links")]
        [SerializeField] private TMP_Text header;
        [SerializeField] private Button createButton;
        [SerializeField] private ItemListRenderer itemListRenderer;
        
        [Header("Prefabs")]
        [SerializeField] private ModalWindow modalWindowPrefab;

        [Inject] private DeviceRoleDataService _dataService;
        
        private ItemListDataProvider<DeviceRole> _itemListDataProvider;

        private const string NameField = "Name";
        private const string DescriptionField = "Description";
        
        private void Awake()
        {
            if (header == null) Debug.LogError("Header is null");
            if (createButton == null) Debug.LogError("CreateButton is null");
            if (itemListRenderer == null) Debug.LogError("No item list renderer found");
        }
        
        private void Start()
        {
            var deviceRoles = _dataService.Get();
            
            _itemListDataProvider = new ItemListDataProvider<DeviceRole>(
                deviceRoles,
                model => model.Name,
                model => model.Description);
            
            itemListRenderer.CreateFromDataProvider(_itemListDataProvider);
            
            createButton.onClick.AddListener(OnCreate);
        }

        private void OnCreate()
        {
            // Создать новое модальное окно
            var canvasTransform = GetComponentInParent<Canvas>().transform;
            
            // Расположить окно поверх остальных
            var modalWindow = Instantiate(modalWindowPrefab, canvasTransform);
            var rectTransform = modalWindow.GetComponent<RectTransform>();
            rectTransform.SetAsLastSibling();
            rectTransform.ForceUpdateRectTransforms();
            
            // Добавить нужные поля
            modalWindow.AddField(NameField, string.Empty);
            modalWindow.AddField(DescriptionField, string.Empty);
            
            // Привязать кнопку подтверждение к особому методу 
            modalWindow.OnSubmit += CreateNewItem;
        }

        private void CreateNewItem(Dictionary<string, string> fields)
        {
            var deviceRoleName = fields[NameField];
            var deviceRoleDescription = fields[DescriptionField];
            _dataService.Create(deviceRoleName, deviceRoleDescription);
            itemListRenderer.AddItemAtEnd(deviceRoleName, deviceRoleDescription);
        }

        public override void OnPageInit(PageArgs args)
        {
        }
    }
}