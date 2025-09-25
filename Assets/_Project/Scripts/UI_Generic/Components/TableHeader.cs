using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Polymer.UI.Components
{
    /// <summary>
    /// Заголовок таблицы с именованными колонками
    /// </summary>
    public class TableHeader : MonoBehaviour
    {
        private TextMeshProUGUI _nameColumnText;
        private TextMeshProUGUI _descriptionColumnText;

        public void Initialize(string nameColumnTitle, string descriptionColumnTitle)
        {
            CreateHeaderRow(nameColumnTitle, descriptionColumnTitle);
        }

        private void CreateHeaderRow(string nameColumnTitle, string descriptionColumnTitle)
        {
            // Создаем контейнер для заголовка
            GameObject headerContainer = new GameObject("HeaderContainer");
            headerContainer.transform.SetParent(transform, false);

            RectTransform headerRect = headerContainer.AddComponent<RectTransform>();
            headerRect.anchorMin = Vector2.zero;
            headerRect.anchorMax = Vector2.one;
            headerRect.offsetMin = Vector2.zero;
            headerRect.offsetMax = Vector2.zero;

            // Добавляем фон заголовка
            Image headerImage = headerContainer.AddComponent<Image>();
            headerImage.color = new Color(0.25f, 0.25f, 0.3f, 1f);

            // Создаем колонку "Название"
            CreateColumn(headerContainer, nameColumnTitle, 0f, 0.5f, out _nameColumnText);

            // Создаем колонку "Описание"
            CreateColumn(headerContainer, descriptionColumnTitle, 0.5f, 1f, out _descriptionColumnText);
        }

        private void CreateColumn(GameObject parent, string title, float anchorMinX, float anchorMaxX, out TextMeshProUGUI textComponent)
        {
            GameObject columnObject = new GameObject($"Column_{title}");
            columnObject.transform.SetParent(parent.transform, false);

            RectTransform columnRect = columnObject.AddComponent<RectTransform>();
            columnRect.anchorMin = new Vector2(anchorMinX, 0f);
            columnRect.anchorMax = new Vector2(anchorMaxX, 1f);
            columnRect.offsetMin = new Vector2(10f, 5f);
            columnRect.offsetMax = new Vector2(-10f, -5f);

            textComponent = columnObject.AddComponent<TextMeshProUGUI>();
            textComponent.text = title;
            textComponent.fontSize = 16f;
            textComponent.fontStyle = FontStyles.Bold;
            textComponent.color = Color.white;
            textComponent.alignment = TextAlignmentOptions.Left;
        }
    }
}
