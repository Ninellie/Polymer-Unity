// using UnityEngine;
// using TMPro;
// using Polymer.UI.Pages;
// using Polymer.Core.Services;
//
// namespace Polymer.UI.Navigation
// {
//     /// <summary>
//     /// Система навигации между страницами
//     /// </summary>
//     public class PageNavigation : MonoBehaviour
//     {
//         [SerializeField] private Transform _leftPanelParent;
//         [SerializeField] private Transform _centerPanelParent;
//         
//         private JsonDataService _jsonDataService;
//         private Pages.DeviceRolesPage _deviceRolesPage;
//         private Pages.ManufacturersPage _manufacturersPage;
//         private BasePage _currentPage;
//         private UnityEngine.UI.Button _currentSelectedButton;
//
//         public void Initialize(JsonDataService jsonDataService, Transform leftPanelParent, Transform centerPanelParent)
//         {
//             _jsonDataService = jsonDataService;
//             _leftPanelParent = leftPanelParent;
//             _centerPanelParent = centerPanelParent;
//             CreateNavigationButtons();
//             CreatePages();
//         }
//
//         private void CreateNavigationButtons()
//         {
//             // Создаем кнопку "Роли устройств"
//             var deviceRolesButton = CreateNavigationButton("Роли устройств", 0, () => ShowPage(_deviceRolesPage));
//             
//             // Создаем кнопку "Производители"
//             var manufacturersButton = CreateNavigationButton("Производители", 1, () => ShowPage(_manufacturersPage));
//         }
//
//         private UnityEngine.UI.Button CreateNavigationButton(string text, int index, System.Action onClick)
//         {
//             GameObject buttonObject = new GameObject($"NavButton_{text}");
//             buttonObject.transform.SetParent(_leftPanelParent, false);
//
//             // Настраиваем RectTransform
//             RectTransform buttonRect = buttonObject.AddComponent<RectTransform>();
//             buttonRect.anchorMin = new Vector2(0f, 1f);
//             buttonRect.anchorMax = new Vector2(1f, 1f);
//             buttonRect.pivot = new Vector2(0.5f, 1f);
//             buttonRect.sizeDelta = new Vector2(0f, 40f);
//             buttonRect.anchoredPosition = new Vector2(0f, -10f - (index * 40f));
//
//             // Добавляем Image для фона кнопки
//             var buttonImage = buttonObject.AddComponent<UnityEngine.UI.Image>();
//             // Цвет Image оставляем белым по умолчанию
//
//             // Добавляем Button компонент с хайлайтом
//             var button = buttonObject.AddComponent<UnityEngine.UI.Button>();
//             button.targetGraphic = buttonImage;
//             button.transition = UnityEngine.UI.Selectable.Transition.ColorTint;
//             
//             // Настраиваем цвета для хайлайта
//             var colors = button.colors;
//             colors.normalColor = new Color(1f, 1f, 1f, 0f); // Прозрачный
//             colors.highlightedColor = new Color(0.3f, 0.3f, 0.35f, 1f); // Серый
//             colors.pressedColor = new Color(0.3f, 0.3f, 0.35f, 1f); // Серый
//             colors.disabledColor = new Color(0.3f, 0.3f, 0.35f, 1f); // Серый
//             colors.selectedColor = new Color(0.3f, 0.3f, 0.35f, 1f); // Серый
//             colors.fadeDuration = 0f; // Без анимации
//             button.colors = colors;
//
//             // Создаем текст кнопки
//             GameObject textObject = new GameObject("Text");
//             textObject.transform.SetParent(buttonObject.transform, false);
//
//             RectTransform textRect = textObject.AddComponent<RectTransform>();
//             textRect.anchorMin = Vector2.zero;
//             textRect.anchorMax = Vector2.one;
//             textRect.offsetMin = new Vector2(10f, 0f); // Отступ слева
//             textRect.offsetMax = new Vector2(-10f, 0f); // Отступ справа
//
//             TextMeshProUGUI buttonText = textObject.AddComponent<TextMeshProUGUI>();
//             buttonText.text = text;
//             buttonText.fontSize = 16f;
//             buttonText.color = Color.white; // Обычный цвет
//             buttonText.alignment = TextAlignmentOptions.Left;
//
//             // Добавляем обработчик клика
//             button.onClick.AddListener(() => {
//                 SetSelectedButton(button, buttonText);
//                 onClick?.Invoke();
//             });
//
//             return button;
//         }
//
//         private void CreatePages()
//         {
//             // Создаем страницу ролей устройств
//             _deviceRolesPage = CreatePage<Pages.DeviceRolesPage>("DeviceRolesPage");
//             _deviceRolesPage.Initialize(_jsonDataService);
//
//             // Создаем страницу производителей
//             _manufacturersPage = CreatePage<Pages.ManufacturersPage>("ManufacturersPage");
//             _manufacturersPage.Initialize(_jsonDataService);
//
//             // Скрываем все страницы по умолчанию
//             _deviceRolesPage.Hide();
//             _manufacturersPage.Hide();
//         }
//
//         private T CreatePage<T>(string pageName) where T : BasePage
//         {
//             GameObject pageObject = new GameObject(pageName);
//             pageObject.transform.SetParent(_centerPanelParent, false);
//
//             // Настраиваем RectTransform для страницы
//             RectTransform pageRect = pageObject.AddComponent<RectTransform>();
//             pageRect.anchorMin = Vector2.zero;
//             pageRect.anchorMax = Vector2.one;
//             pageRect.offsetMin = Vector2.zero;
//             pageRect.offsetMax = Vector2.zero;
//
//             // Создаем страницу
//             T page = pageObject.AddComponent<T>();
//
//             // Создаем панель инструментов
//             Transform toolbarParent = CreateToolbar(pageObject);
//
//             // Создаем статичный контент
//             Transform contentParent = CreateStaticContent(pageObject);
//
//             // Устанавливаем родители для страницы
//             if (page is Pages.DeviceRolesPage deviceRolesPage)
//             {
//                 deviceRolesPage.ContentParent = contentParent;
//                 deviceRolesPage.ToolbarParent = toolbarParent;
//             }
//             else if (page is Pages.ManufacturersPage manufacturersPage)
//             {
//                 manufacturersPage.ContentParent = contentParent;
//                 manufacturersPage.ToolbarParent = toolbarParent;
//             }
//
//             return page;
//         }
//
//         private Transform CreateToolbar(GameObject parent)
//         {
//             // Создаем панель инструментов
//             GameObject toolbar = new GameObject("Toolbar");
//             toolbar.transform.SetParent(parent.transform, false);
//
//             RectTransform toolbarRect = toolbar.AddComponent<RectTransform>();
//             toolbarRect.anchorMin = new Vector2(0f, 0.94f);
//             toolbarRect.anchorMax = new Vector2(1f, 1f);
//             toolbarRect.offsetMin = Vector2.zero;
//             toolbarRect.offsetMax = Vector2.zero;
//
//             return toolbar.transform;
//         }
//
//         private Transform CreateStaticContent(GameObject parent)
//         {
//             // Создаем статичный контент без скролла
//             GameObject content = new GameObject("Content");
//             content.transform.SetParent(parent.transform, false);
//
//             RectTransform contentRect = content.AddComponent<RectTransform>();
//             contentRect.anchorMin = new Vector2(0f, 0f);
//             contentRect.anchorMax = new Vector2(1f, 0.94f);
//             contentRect.offsetMin = Vector2.zero;
//             contentRect.offsetMax = Vector2.zero;
//
//             return content.transform;
//         }
//
//         private void ShowPage(BasePage page)
//         {
//             if (_currentPage != null)
//                 _currentPage.Hide();
//
//             _currentPage = page;
//             _currentPage.Show();
//         }
//
//         private void SetSelectedButton(UnityEngine.UI.Button button, TextMeshProUGUI buttonText)
//         {
//             // Сбрасываем предыдущую выбранную кнопку
//             if (_currentSelectedButton != null)
//             {
//                 _currentSelectedButton.interactable = true;
//                 var previousText = _currentSelectedButton.GetComponentInChildren<TextMeshProUGUI>();
//                 if (previousText != null)
//                 {
//                     previousText.color = Color.white; // Обычный цвет
//                 }
//             }
//
//             // Устанавливаем новую выбранную кнопку
//             _currentSelectedButton = button;
//             button.interactable = false; // Делаем кнопку неактивной, чтобы она оставалась в состоянии selected
//             buttonText.color = new Color(0.8f, 0.8f, 1f, 1f); // Голубоватый цвет для выбранной кнопки
//         }
//     }
// }
