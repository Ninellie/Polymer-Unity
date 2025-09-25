// using UnityEngine;
// using UnityEngine.UI;
// using VContainer.Unity;
// using Polymer.Core.Services;
// using Polymer.UI.Navigation;
//
// namespace Polymer.UI
// {
//     /// <summary>
//     /// Главное меню приложения с процедурно создаваемым интерфейсом
//     /// </summary>
//     public class MainMenu : IStartable
//     {
//         private Canvas _mainCanvas;
//         private GameObject _leftPanel;
//         private GameObject _topPanel;
//         private GameObject _centerPanel;
//         private PageNavigation _pageNavigation;
//         private JsonDataService _jsonDataService;
//
//         public MainMenu(JsonDataService jsonDataService)
//         {
//             _jsonDataService = jsonDataService;
//         }
//
//         public void Start()
//         {
//             CreateMainCanvas();
//             CreateLeftPanel();
//             CreateTopPanel();
//             CreateCenterPanel();
//             CreatePageNavigation();
//         }
//
//         private void CreateMainCanvas()
//         {
//             // Создаем главный Canvas
//             GameObject canvasObject = new GameObject("MainCanvas");
//             _mainCanvas = canvasObject.AddComponent<Canvas>();
//             _mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
//             _mainCanvas.sortingOrder = 0;
//
//             // Добавляем CanvasRenderer
//             canvasObject.AddComponent<CanvasRenderer>();
//
//             // Добавляем CanvasScaler
//             CanvasScaler canvasScaler = canvasObject.AddComponent<CanvasScaler>();
//             canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
//             canvasScaler.referenceResolution = new Vector2(1920, 1080);
//             canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
//             canvasScaler.matchWidthOrHeight = 0.5f;
//
//             // Добавляем GraphicRaycaster
//             canvasObject.AddComponent<GraphicRaycaster>();
//
//             Debug.Log("[MainMenu] Главный Canvas создан");
//         }
//
//         private void CreateLeftPanel()
//         {
//             // Создаем левую панель навигации
//             _leftPanel = new GameObject("LeftPanel");
//             _leftPanel.transform.SetParent(_mainCanvas.transform, false);
//
//             // Настраиваем RectTransform
//             RectTransform leftRect = _leftPanel.AddComponent<RectTransform>();
//             leftRect.anchorMin = new Vector2(0f, 0f);
//             leftRect.anchorMax = new Vector2(0.15f, 1f);
//             leftRect.offsetMin = Vector2.zero;
//             leftRect.offsetMax = Vector2.zero;
//
//             // Добавляем Image компонент с тёмным цветом
//             Image leftImage = _leftPanel.AddComponent<Image>();
//             leftImage.color = new Color(0.2f, 0.2f, 0.25f, 1f); // Тёмно-серый с фиолетовым оттенком
//
//             Debug.Log("[MainMenu] Левая панель создана (15% экрана слева)");
//         }
//
//         private void CreateTopPanel()
//         {
//             // Создаем верхнюю панель навигации
//             _topPanel = new GameObject("TopPanel");
//             _topPanel.transform.SetParent(_mainCanvas.transform, false);
//
//             // Настраиваем RectTransform
//             RectTransform topRect = _topPanel.AddComponent<RectTransform>();
//             topRect.anchorMin = new Vector2(0.15f, 0.98f);
//             topRect.anchorMax = new Vector2(1f, 1f);
//             topRect.offsetMin = Vector2.zero;
//             topRect.offsetMax = Vector2.zero;
//
//             // Добавляем Image компонент с тёмным цветом
//             Image topImage = _topPanel.AddComponent<Image>();
//             topImage.color = new Color(0.25f, 0.2f, 0.2f, 1f); // Тёмно-серый с красным оттенком
//
//             Debug.Log("[MainMenu] Верхняя панель создана (2% сверху)");
//         }
//
//         private void CreateCenterPanel()
//         {
//             // Создаем центральную панель с контентом
//             _centerPanel = new GameObject("CenterPanel");
//             _centerPanel.transform.SetParent(_mainCanvas.transform, false);
//
//             // Настраиваем RectTransform
//             RectTransform centerRect = _centerPanel.AddComponent<RectTransform>();
//             centerRect.anchorMin = new Vector2(0.15f, 0f);
//             centerRect.anchorMax = new Vector2(1f, 0.98f);
//             centerRect.offsetMin = Vector2.zero;
//             centerRect.offsetMax = Vector2.zero;
//
//             // Добавляем Image компонент с тёмным цветом
//             Image centerImage = _centerPanel.AddComponent<Image>();
//             centerImage.color = new Color(0.15f, 0.15f, 0.2f, 1f); // Очень тёмно-синий
//
//             Debug.Log("[MainMenu] Центральная панель создана (оставшееся место)");
//         }
//
//         private void CreatePageNavigation()
//         {
//             // Создаем систему навигации
//             GameObject navigationObject = new GameObject("PageNavigation");
//             navigationObject.transform.SetParent(_mainCanvas.transform, false);
//
//             _pageNavigation = navigationObject.AddComponent<PageNavigation>();
//             _pageNavigation.Initialize(_jsonDataService, _leftPanel.transform, _centerPanel.transform);
//
//             Debug.Log("[MainMenu] Система навигации создана");
//         }
//     }
// }
