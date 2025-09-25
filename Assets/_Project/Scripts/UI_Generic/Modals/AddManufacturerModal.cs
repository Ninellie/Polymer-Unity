// using Core.Models;
// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;
// using Polymer.Core.Services;
//
// namespace Polymer.UI.Modals
// {
//     /// <summary>
//     /// Модальное окно для добавления нового производителя
//     /// </summary>
//     public class AddManufacturerModal : BaseModal
//     {
//         private TMP_InputField _nameInput;
//         private TMP_InputField _descriptionInput;
//         private JsonDataService _jsonDataService;
//         private System.Action _onManufacturerAdded;
//         private Manufacturer _editingManufacturer;
//         private bool _isEditMode;
//
//         public void Initialize(JsonDataService jsonDataService, System.Action onManufacturerAdded, Manufacturer editingManufacturer = null)
//         {
//             _jsonDataService = jsonDataService;
//             _onManufacturerAdded = onManufacturerAdded;
//             _editingManufacturer = editingManufacturer;
//             _isEditMode = editingManufacturer != null;
//         }
//
//         protected override void CreateModalContent()
//         {
//             // Создаем контент модального окна
//             _modalContent = new GameObject("ModalContent");
//             _modalContent.transform.SetParent(transform, false);
//
//             RectTransform contentRect = _modalContent.AddComponent<RectTransform>();
//             contentRect.anchorMin = new Vector2(0.5f, 0.5f);
//             contentRect.anchorMax = new Vector2(0.5f, 0.5f);
//             contentRect.sizeDelta = new Vector2(400f, 300f);
//             contentRect.anchoredPosition = Vector2.zero;
//
//             // Добавляем фон контента
//             Image contentImage = _modalContent.AddComponent<Image>();
//             contentImage.color = new Color(0.2f, 0.2f, 0.25f, 1f);
//
//             // Добавляем обработчик клика для предотвращения закрытия при клике на контент
//             Button contentButton = _modalContent.AddComponent<Button>();
//             contentButton.targetGraphic = contentImage;
//             contentButton.onClick.AddListener(() => { }); // Пустой обработчик - предотвращает всплытие события
//
//             CreateTitle();
//             CreateNameInput();
//             CreateDescriptionInput();
//             CreateButtons();
//         }
//
//         private void CreateTitle()
//         {
//             GameObject titleObject = new GameObject("Title");
//             titleObject.transform.SetParent(_modalContent.transform, false);
//
//             RectTransform titleRect = titleObject.AddComponent<RectTransform>();
//             titleRect.anchorMin = new Vector2(0f, 0.8f);
//             titleRect.anchorMax = new Vector2(1f, 1f);
//             titleRect.offsetMin = new Vector2(20f, 10f);
//             titleRect.offsetMax = new Vector2(-20f, -10f);
//
//             TextMeshProUGUI titleText = titleObject.AddComponent<TextMeshProUGUI>();
//             titleText.text = _isEditMode ? "Редактировать производителя" : "Добавить производителя";
//             titleText.fontSize = 20f;
//             titleText.fontStyle = FontStyles.Bold;
//             titleText.color = Color.white;
//             titleText.alignment = TextAlignmentOptions.Center;
//         }
//
//         private void CreateNameInput()
//         {
//             // Создаем контейнер для поля ввода имени
//             GameObject nameContainer = new GameObject("NameContainer");
//             nameContainer.transform.SetParent(_modalContent.transform, false);
//
//             RectTransform nameRect = nameContainer.AddComponent<RectTransform>();
//             nameRect.anchorMin = new Vector2(0f, 0.6f);
//             nameRect.anchorMax = new Vector2(1f, 0.75f);
//             nameRect.offsetMin = new Vector2(20f, 5f);
//             nameRect.offsetMax = new Vector2(-20f, -5f);
//
//             // Создаем поле ввода
//             _nameInput = CreateInputField(nameContainer, "Название производителя");
//         }
//
//         private void CreateDescriptionInput()
//         {
//             // Создаем контейнер для поля ввода описания
//             GameObject descContainer = new GameObject("DescriptionContainer");
//             descContainer.transform.SetParent(_modalContent.transform, false);
//
//             RectTransform descRect = descContainer.AddComponent<RectTransform>();
//             descRect.anchorMin = new Vector2(0f, 0.35f);
//             descRect.anchorMax = new Vector2(1f, 0.55f);
//             descRect.offsetMin = new Vector2(20f, 5f);
//             descRect.offsetMax = new Vector2(-20f, -5f);
//
//             // Создаем поле ввода
//             _descriptionInput = CreateInputField(descContainer, "Описание производителя");
//         }
//
//         private TMP_InputField CreateInputField(GameObject parent, string placeholder)
//         {
//             // Создаем поле ввода
//             GameObject inputObject = new GameObject("InputField");
//             inputObject.transform.SetParent(parent.transform, false);
//
//             RectTransform inputRect = inputObject.AddComponent<RectTransform>();
//             inputRect.anchorMin = Vector2.zero;
//             inputRect.anchorMax = Vector2.one;
//             inputRect.offsetMin = Vector2.zero;
//             inputRect.offsetMax = Vector2.zero;
//
//             // Добавляем фон
//             Image inputImage = inputObject.AddComponent<Image>();
//             inputImage.color = new Color(0.1f, 0.1f, 0.15f, 1f);
//
//             // Создаем TMP_InputField
//             TMP_InputField inputField = inputObject.AddComponent<TMP_InputField>();
//
//             // Создаем текст
//             GameObject textObject = new GameObject("Text");
//             textObject.transform.SetParent(inputObject.transform, false);
//
//             RectTransform textRect = textObject.AddComponent<RectTransform>();
//             textRect.anchorMin = Vector2.zero;
//             textRect.anchorMax = Vector2.one;
//             textRect.offsetMin = new Vector2(10f, 5f);
//             textRect.offsetMax = new Vector2(-10f, -5f);
//
//             TextMeshProUGUI textComponent = textObject.AddComponent<TextMeshProUGUI>();
//             textComponent.text = "";
//             textComponent.fontSize = 14f;
//             textComponent.color = Color.white;
//
//             // Создаем placeholder
//             GameObject placeholderObject = new GameObject("Placeholder");
//             placeholderObject.transform.SetParent(inputObject.transform, false);
//
//             RectTransform placeholderRect = placeholderObject.AddComponent<RectTransform>();
//             placeholderRect.anchorMin = Vector2.zero;
//             placeholderRect.anchorMax = Vector2.one;
//             placeholderRect.offsetMin = new Vector2(10f, 5f);
//             placeholderRect.offsetMax = new Vector2(-10f, -5f);
//
//             TextMeshProUGUI placeholderComponent = placeholderObject.AddComponent<TextMeshProUGUI>();
//             placeholderComponent.text = placeholder;
//             placeholderComponent.fontSize = 14f;
//             placeholderComponent.color = new Color(0.5f, 0.5f, 0.5f, 1f);
//             placeholderComponent.fontStyle = FontStyles.Italic;
//
//             // Настраиваем InputField
//             inputField.textComponent = textComponent;
//             inputField.placeholder = placeholderComponent;
//
//             // Заполняем поля при редактировании
//             if (_isEditMode && _editingManufacturer != null)
//             {
//                 if (placeholder.Contains("Название"))
//                     inputField.text = _editingManufacturer.Name;
//                 else if (placeholder.Contains("Описание"))
//                     inputField.text = _editingManufacturer.Description;
//             }
//
//             return inputField;
//         }
//
//         private void CreateButtons()
//         {
//             // Создаем контейнер для кнопок
//             GameObject buttonsContainer = new GameObject("ButtonsContainer");
//             buttonsContainer.transform.SetParent(_modalContent.transform, false);
//
//             RectTransform buttonsRect = buttonsContainer.AddComponent<RectTransform>();
//             buttonsRect.anchorMin = new Vector2(0f, 0f);
//             buttonsRect.anchorMax = new Vector2(1f, 0.3f);
//             buttonsRect.offsetMin = new Vector2(20f, 10f);
//             buttonsRect.offsetMax = new Vector2(-20f, -10f);
//
//             // Создаем кнопку "Отмена"
//             CreateButton(buttonsContainer, "Отмена", new Vector2(0f, 0f), new Vector2(0.5f, 1f), Cancel);
//
//             // Создаем кнопку "Создать" или "Подтвердить"
//             string buttonText = _isEditMode ? "Подтвердить" : "Создать";
//             CreateButton(buttonsContainer, buttonText, new Vector2(0.5f, 0f), new Vector2(1f, 1f), CreateManufacturer);
//         }
//
//         private void CreateButton(GameObject parent, string text, Vector2 anchorMin, Vector2 anchorMax, System.Action onClick)
//         {
//             GameObject buttonObject = new GameObject($"Button_{text}");
//             buttonObject.transform.SetParent(parent.transform, false);
//
//             RectTransform buttonRect = buttonObject.AddComponent<RectTransform>();
//             buttonRect.anchorMin = anchorMin;
//             buttonRect.anchorMax = anchorMax;
//             buttonRect.offsetMin = new Vector2(10f, 5f);
//             buttonRect.offsetMax = new Vector2(-10f, -5f);
//
//             Image buttonImage = buttonObject.AddComponent<Image>();
//             buttonImage.color = new Color(0.3f, 0.3f, 0.35f, 1f);
//
//             Button button = buttonObject.AddComponent<Button>();
//             button.targetGraphic = buttonImage;
//             button.onClick.AddListener(() => onClick?.Invoke());
//
//             // Создаем текст кнопки
//             GameObject textObject = new GameObject("Text");
//             textObject.transform.SetParent(buttonObject.transform, false);
//
//             RectTransform textRect = textObject.AddComponent<RectTransform>();
//             textRect.anchorMin = Vector2.zero;
//             textRect.anchorMax = Vector2.one;
//             textRect.offsetMin = Vector2.zero;
//             textRect.offsetMax = Vector2.zero;
//
//             TextMeshProUGUI buttonText = textObject.AddComponent<TextMeshProUGUI>();
//             buttonText.text = text;
//             buttonText.fontSize = 14f;
//             buttonText.color = Color.white;
//             buttonText.alignment = TextAlignmentOptions.Center;
//         }
//
//         private void Cancel()
//         {
//             Hide();
//         }
//
//         private void CreateManufacturer()
//         {
//             string name = _nameInput.text.Trim();
//             string description = _descriptionInput.text.Trim();
//
//             if (string.IsNullOrEmpty(name))
//             {
//                 ShowNameFieldError();
//                 return;
//             }
//
//             if (_isEditMode && _editingManufacturer != null)
//             {
//                 // Редактируем существующего производителя
//                 _editingManufacturer.Name = name;
//                 _editingManufacturer.Description = description;
//                 Debug.Log($"Обновлен производитель: {name}");
//             }
//             else
//             {
//                 // Создаем нового производителя
//                 var newManufacturer = new Manufacturer
//                 {
//                     Id = GetNextManufacturerId(),
//                     Name = name,
//                     Description = description
//                 };
//
//                 // Добавляем в данные
//                 _jsonDataService.Data.Manufacturers.Add(newManufacturer);
//                 Debug.Log($"Добавлен новый производитель: {name}");
//             }
//
//             // Сохраняем данные
//             _jsonDataService.SaveDataToFile();
//
//             // Обновляем UI
//             _onManufacturerAdded?.Invoke();
//
//             // Очищаем поля и закрываем окно
//             _nameInput.text = "";
//             _descriptionInput.text = "";
//             Hide();
//         }
//
//         private int GetNextManufacturerId()
//         {
//             int maxId = 0;
//             foreach (var manufacturer in _jsonDataService.Data.Manufacturers)
//             {
//                 if (manufacturer.Id > maxId)
//                     maxId = manufacturer.Id;
//             }
//             return maxId + 1;
//         }
//
//         private void ShowNameFieldError()
//         {
//             StartCoroutine(FlashNameFieldRed());
//         }
//
//         private System.Collections.IEnumerator FlashNameFieldRed()
//         {
//             Image nameFieldImage = _nameInput.GetComponent<Image>();
//             Color originalColor = nameFieldImage.color;
//             Color errorColor = new Color(0.8f, 0.2f, 0.2f, 1f); // Красный цвет
//
//             // Мигаем 3 раза
//             for (int i = 0; i < 3; i++)
//             {
//                 nameFieldImage.color = errorColor;
//                 yield return new WaitForSeconds(0.2f);
//                 nameFieldImage.color = originalColor;
//                 yield return new WaitForSeconds(0.2f);
//             }
//         }
//     }
// }
