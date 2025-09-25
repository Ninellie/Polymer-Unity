using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Polymer.UI.Components
{
    /// <summary>
    /// Строка таблицы с данными
    /// </summary>
    public class TableRow : MonoBehaviour
    {
        private TextMeshProUGUI _nameText;
        private TextMeshProUGUI _descriptionText;
        private System.Action _onEditClicked;

        public void Initialize(string name, string description, System.Action onEditClicked = null)
        {
            _onEditClicked = onEditClicked;
            CreateRow(name, description);
        }

        private void CreateRow(string name, string description)
        {
            // Создаем контейнер для строки
            GameObject rowContainer = new GameObject("RowContainer");
            rowContainer.transform.SetParent(transform, false);

            RectTransform rowRect = rowContainer.AddComponent<RectTransform>();
            rowRect.anchorMin = Vector2.zero;
            rowRect.anchorMax = Vector2.one;
            rowRect.offsetMin = Vector2.zero;
            rowRect.offsetMax = Vector2.zero;

            // Добавляем фон строки
            Image rowImage = rowContainer.AddComponent<Image>();
            rowImage.color = new Color(0.2f, 0.2f, 0.25f, 0.8f);

            // Создаем колонку "Название"
            CreateColumn(rowContainer, name, 0f, 0.5f, out _nameText);

            // Создаем колонку "Описание"
            CreateColumn(rowContainer, description, 0.5f, 0.8f, out _descriptionText);

            // Создаем кнопку "Редактировать" если есть обработчик
            if (_onEditClicked != null)
            {
                CreateEditButton(rowContainer);
            }
        }

        private void CreateColumn(GameObject parent, string text, float anchorMinX, float anchorMaxX, out TextMeshProUGUI textComponent)
        {
            GameObject columnObject = new GameObject($"Column");
            columnObject.transform.SetParent(parent.transform, false);

            RectTransform columnRect = columnObject.AddComponent<RectTransform>();
            columnRect.anchorMin = new Vector2(anchorMinX, 0f);
            columnRect.anchorMax = new Vector2(anchorMaxX, 1f);
            columnRect.offsetMin = new Vector2(10f, 5f);
            columnRect.offsetMax = new Vector2(-10f, -5f);

            textComponent = columnObject.AddComponent<TextMeshProUGUI>();
            textComponent.text = text;
            textComponent.fontSize = 14f;
            textComponent.color = Color.white;
            textComponent.alignment = TextAlignmentOptions.Left;
        }

        public void UpdateData(string name, string description)
        {
            _nameText.text = name;
            _descriptionText.text = description;
        }

        private void CreateEditButton(GameObject parent)
        {
            GameObject buttonObject = new GameObject("EditButton");
            buttonObject.transform.SetParent(parent.transform, false);

            RectTransform buttonRect = buttonObject.AddComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0.8f, 0.1f);
            buttonRect.anchorMax = new Vector2(0.95f, 0.9f);
            buttonRect.offsetMin = Vector2.zero;
            buttonRect.offsetMax = Vector2.zero;

            Image buttonImage = buttonObject.AddComponent<Image>();
            buttonImage.color = new Color(0.3f, 0.3f, 0.35f, 1f);

            Button button = buttonObject.AddComponent<Button>();
            button.targetGraphic = buttonImage;
            button.onClick.AddListener(() => _onEditClicked?.Invoke());

            // Создаем текст кнопки
            GameObject textObject = new GameObject("Text");
            textObject.transform.SetParent(buttonObject.transform, false);

            RectTransform textRect = textObject.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            TextMeshProUGUI buttonText = textObject.AddComponent<TextMeshProUGUI>();
            buttonText.text = "Редактировать";
            buttonText.fontSize = 10f;
            buttonText.color = Color.white;
            buttonText.alignment = TextAlignmentOptions.Center;
        }
    }
}
