using System.Collections.Generic;
using TriInspector;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(HorizontalLayoutGroup))]
    public class TableRow : MonoBehaviour
    {
        [SerializeField] private TableElement elementPrefab;
        [SerializeField] private HorizontalLayoutGroup layoutGroup;
        [SerializeField] private List<TableElement> elements = new();
        
        [SerializeField] private int row;

        public int Row
        {
            get => row;
            set => row = value;
        }

        /// <summary>
        /// Задаёт конфигурацию колонок строки. Перезаписывает предыдущую конфигурацию.
        /// </summary>
        /// <param name="columns">Имена и ширина колонок</param>
        public void SetColumns(List<(string columnName, float width)> columns)
        {
            // Очистить предыдущее состояние
            Clear();

            
            foreach (var columnData in columns)
            { 
                // Создать инстансы элементов
                var element = Instantiate(elementPrefab, transform);
                
                // Установить имена игровых объектов элементов
                element.gameObject.name = columnData.columnName;
                
                // Установить ширину
                
                // Установить номер строки для элемента
                element.Row = row;
                
                
                elements.Add(element);

                // Установить номер колонки для элемента
                element.Column = elements.Count;
            }
            
            
            
            
        }
        
        public List<TableElement> Elements => elements;
        
        public void Clear()
        {
            foreach (var element in elements)
            {
                Destroy(element.gameObject);
            }
            elements.Clear();
            Configure();
        }
        
        [Button("Add element")]
        public TableElement AddElement(string elementName, string elementContent, float width = -1)
        {
            var element = Instantiate(elementPrefab, transform);
            elements.Add(element);
            
            element.SetText(elementContent);
            element.gameObject.name = elementName;
            element.SetWidth(width);
            element.Row = row;
            element.Column = elements.Count;
            
            return element;
        }

        private void Configure()
        {
            layoutGroup = GetComponent<HorizontalLayoutGroup>();
            layoutGroup.childControlHeight = true;
            layoutGroup.childControlWidth = true;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childScaleHeight = false;
            layoutGroup.childScaleWidth = false;
            layoutGroup.spacing = 10;
            layoutGroup.childAlignment = TextAnchor.UpperLeft;
        }
    }
}