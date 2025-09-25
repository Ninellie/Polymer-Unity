using System.Collections.Generic;
using Core.Models;
using Core.Services;
using TMPro;
using UI.ListView;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace UI.DevicePage
{
    public class DeviceRolesPage : MonoBehaviour
    {
        [Header("Inner components links")]
        [SerializeField] private TMP_Text header;
        [SerializeField] private Button createButton;
        [SerializeField] private ItemListRenderer itemListRenderer;
        
        [Header("Prefabs")]
        [SerializeField] private ModalWindow modalWindowPrefab;
        
        [Inject] private DeviceRoleDataService DataService { get; set; }
        
        private ItemListDataProvider<DeviceRole> _itemListDataProvider;

        private const string NameField = "Name";
        private const string DescriptionField = "Description";
        
        private void Awake()
        {
            if (header == null) Debug.LogError("Header is null");
            if (createButton == null) Debug.LogError("CreateButton is null");
            if (itemListRenderer == null) Debug.LogError("No item list renderer found");
        }
        
        private void OnEnable()
        {
            var deviceRoles = DataService.Get();
            
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
            DataService.Create(deviceRoleName, deviceRoleDescription);
            itemListRenderer.AddItemAtEnd(deviceRoleName, deviceRoleDescription);
        }
        
        private void OnDisable()
        {
            Destroy(itemListRenderer);
        }
    }
}