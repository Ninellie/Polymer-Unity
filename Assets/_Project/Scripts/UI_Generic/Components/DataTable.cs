using System.Collections.Generic;
using UnityEngine;

namespace Polymer.UI.Components
{
    /// <summary>
    /// Управление таблицей данных
    /// </summary>
    public class DataTable : MonoBehaviour
    {
        private TableHeader _header;
        private List<TableRow> _rows = new List<TableRow>();
        private Transform _contentParent;
        private System.Action<object> _onEditClicked;

        public void Initialize(Transform contentParent, string nameColumnTitle, string descriptionColumnTitle, System.Action<object> onEditClicked = null)
        {
            _contentParent = contentParent;
            _onEditClicked = onEditClicked;
            CreateHeader(nameColumnTitle, descriptionColumnTitle);
        }

        private void CreateHeader(string nameColumnTitle, string descriptionColumnTitle)
        {
            GameObject headerObject = new GameObject("TableHeader");
            headerObject.transform.SetParent(_contentParent, false);

            RectTransform headerRect = headerObject.AddComponent<RectTransform>();
            headerRect.anchorMin = new Vector2(0f, 1f);
            headerRect.anchorMax = new Vector2(1f, 1f);
            headerRect.pivot = new Vector2(0.5f, 1f);
            headerRect.sizeDelta = new Vector2(0f, 40f);
            headerRect.anchoredPosition = new Vector2(0f, -10f);

            _header = headerObject.AddComponent<TableHeader>();
            _header.Initialize(nameColumnTitle, descriptionColumnTitle);
        }

        public void AddRow(string name, string description, object dataObject = null)
        {
            GameObject rowObject = new GameObject($"TableRow_{_rows.Count}");
            rowObject.transform.SetParent(_contentParent, false);

            RectTransform rowRect = rowObject.AddComponent<RectTransform>();
            rowRect.anchorMin = new Vector2(0f, 1f);
            rowRect.anchorMax = new Vector2(1f, 1f);
            rowRect.pivot = new Vector2(0.5f, 1f);
            rowRect.sizeDelta = new Vector2(0f, 35f);
            rowRect.anchoredPosition = new Vector2(0f, -50f - (_rows.Count * 40f));

            TableRow row = rowObject.AddComponent<TableRow>();
            
            System.Action onEditClicked = null;
            if (_onEditClicked != null && dataObject != null)
            {
                onEditClicked = () => _onEditClicked(dataObject);
            }
            
            row.Initialize(name, description, onEditClicked);
            _rows.Add(row);
        }

        public void ClearRows()
        {
            foreach (var row in _rows)
            {
                if (row != null)
                    DestroyImmediate(row.gameObject);
            }
            _rows.Clear();
        }

        public void Refresh()
        {
            ClearRows();
        }
    }
}
