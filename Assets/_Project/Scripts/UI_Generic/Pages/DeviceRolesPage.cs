// using Core.Models;
// using UnityEngine;
// using Polymer.Core.Services;
// using UnityEngine.UI;
// using TMPro;
// using UI.Modals;
//
// namespace Polymer.UI.Pages
// {
//     /// <summary>
//     /// Страница для отображения списка ролей устройств
//     /// </summary>
//     public class DeviceRolesPage : BasePage
//     {
//         public Transform ContentParent { get; set; }
//         public Transform ToolbarParent { get; set; }
//         
//         private JsonDataService _jsonDataService;
//         private Polymer.UI.Components.DataTable _dataTable;
//         private System.Action _onDeviceRoleAdded;
//
//         public void Initialize(JsonDataService jsonDataService)
//         {
//             _jsonDataService = jsonDataService;
//             _onDeviceRoleAdded = () => Refresh();
//         }
//
//         public override void Show()
//         {
//             base.Show();
//             CreateToolbar();
//             CreateDataTable();
//             Refresh();
//         }
//
//         private void CreateDataTable()
//         {
//             if (_dataTable != null) return;
//
//             GameObject tableObject = new GameObject("DataTable");
//             tableObject.transform.SetParent(ContentParent, false);
//
//             RectTransform tableRect = tableObject.AddComponent<RectTransform>();
//             tableRect.anchorMin = Vector2.zero;
//             tableRect.anchorMax = Vector2.one;
//             tableRect.offsetMin = Vector2.zero;
//             tableRect.offsetMax = Vector2.zero;
//
//             _dataTable = tableObject.AddComponent<Polymer.UI.Components.DataTable>();
//             _dataTable.Initialize(ContentParent, "Название роли", "Описание", OnEditDeviceRole);
//         }
//
//         private void CreateToolbar()
//         {
//             if (ToolbarParent == null) return;
//
//             // Создаем кнопку "Создать"
//             GameObject createButton = new GameObject("CreateButton");
//             createButton.transform.SetParent(ToolbarParent, false);
//
//             RectTransform buttonRect = createButton.AddComponent<RectTransform>();
//             buttonRect.anchorMin = new Vector2(0f, 0f);
//             buttonRect.anchorMax = new Vector2(0f, 1f);
//             buttonRect.sizeDelta = new Vector2(120f, 0f);
//             buttonRect.anchoredPosition = new Vector2(70f, 0f);
//
//             Image buttonImage = createButton.AddComponent<Image>();
//             buttonImage.color = new Color(0.3f, 0.3f, 0.35f, 1f);
//
//             Button button = createButton.AddComponent<Button>();
//             button.targetGraphic = buttonImage;
//             button.onClick.AddListener(ShowAddModal);
//
//             // Создаем текст кнопки
//             GameObject textObject = new GameObject("Text");
//             textObject.transform.SetParent(createButton.transform, false);
//
//             RectTransform textRect = textObject.AddComponent<RectTransform>();
//             textRect.anchorMin = Vector2.zero;
//             textRect.anchorMax = Vector2.one;
//             textRect.offsetMin = Vector2.zero;
//             textRect.offsetMax = Vector2.zero;
//
//             TextMeshProUGUI buttonText = textObject.AddComponent<TextMeshProUGUI>();
//             buttonText.text = "Создать";
//             buttonText.fontSize = 14f;
//             buttonText.color = Color.white;
//             buttonText.alignment = TextAlignmentOptions.Center;
//         }
//
//         private void ShowAddModal()
//         {
//             // Создаем модальное окно процедурно как дочерний объект Canvas
//             GameObject modalObject = new GameObject("AddDeviceRoleModal");
//             modalObject.transform.SetParent(transform.root, false); // Родитель - Canvas
//             
//             // Настраиваем RectTransform для модального окна
//             RectTransform modalRect = modalObject.AddComponent<RectTransform>();
//             modalRect.anchorMin = Vector2.zero;
//             modalRect.anchorMax = Vector2.one;
//             modalRect.offsetMin = Vector2.zero;
//             modalRect.offsetMax = Vector2.zero;
//             
//             var modal = modalObject.AddComponent<AddDeviceRoleModal>();
//             modal.Initialize(_jsonDataService, _onDeviceRoleAdded);
//             modal.Show();
//         }
//
//         public override void Refresh()
//         {
//             if (_dataTable == null) return;
//
//             _dataTable.Refresh();
//             CreateDeviceRoleRows();
//         }
//
//         private void CreateDeviceRoleRows()
//         {
//             if (_jsonDataService?.Data?.DeviceRoles == null)
//                 return;
//
//             foreach (var role in _jsonDataService.Data.DeviceRoles)
//             {
//                 _dataTable.AddRow(role.Name, role.Description, role);
//             }
//         }
//
//         private void OnEditDeviceRole(object deviceRoleObject)
//         {
//             if (deviceRoleObject is DeviceRole deviceRole)
//             {
//                 ShowEditModal(deviceRole);
//             }
//         }
//
//         private void ShowEditModal(DeviceRole deviceRole)
//         {
//             // Создаем модальное окно процедурно как дочерний объект Canvas
//             GameObject modalObject = new GameObject("EditDeviceRoleModal");
//             modalObject.transform.SetParent(transform.root, false); // Родитель - Canvas
//             
//             // Настраиваем RectTransform для модального окна
//             RectTransform modalRect = modalObject.AddComponent<RectTransform>();
//             modalRect.anchorMin = Vector2.zero;
//             modalRect.anchorMax = Vector2.one;
//             modalRect.offsetMin = Vector2.zero;
//             modalRect.offsetMax = Vector2.zero;
//             
//             var modal = modalObject.AddComponent<AddDeviceRoleModal>();
//             modal.Initialize(_jsonDataService, _onDeviceRoleAdded, deviceRole);
//             modal.Show();
//         }
//     }
// }
