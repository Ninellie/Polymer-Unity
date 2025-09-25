using System.Collections.Generic;
using TriInspector;
using UnityEngine;

namespace UI
{
    public class DataTable : MonoBehaviour
    {
        [SerializeField] private TableRow rowPrefab;
        [SerializeField] private TableRow headerRow;
        [SerializeField] private List<TableRow> rows;
        
        [SerializeField] private List<Column> columns;
            
        [SerializeField] private string newColumnName;
        [SerializeField] private float newColumnWidth;
        
        [Button]
        private void AddRow() => AddRow(new List<string>());

        
        public void Init()
        {
            Reset();
            foreach (var column in columns)
            {
                CreateColumn(column);
            }
        }

        public TableElement GetElement(int row,  int column)
        {
            return rows[row].Elements[column];
        }
        
        public TableRow AddRow(List<string> content)
        {
            var row = Instantiate(rowPrefab, transform);
            rows.Add(row);
            row.Row = rows.Count;
            
            var i = 0;
            foreach (var column in columns)
            {
                var contentString = "";
                
                if (content.Count >= i)
                {
                    if (content[i] != null)
                    {
                        contentString = content[i];
                    }
                }
                
                row.AddElement(column.name, contentString, column.width);
                i++;
            }
            
            return row;
        }
        
        [Button("Add Column")]
        public DataTable AddColumn(string columnName, float width = -1)
        {
            // Создать класс колонки и внести данные
            var column = new Column() { name = columnName, width = width };
            
            // Добавить колонку в список
            columns.Add(column);
            
            CreateColumn(column);

            return this;
        }

        public void SetRows(IEnumerable<IEnumerable<string>> rowsList)
        {
            foreach (var row in rowsList)
            {
                
                foreach (var element in row)
                {
                    
                }
            }
        }
        
        private void CreateColumn(Column column)
        {
            // Добавить в заголовочную строку
            var headerRowElement = headerRow.AddElement(column.name, column.name, column.width);
            headerRowElement.Column = columns.Count;
            headerRowElement.Row = 0; // Так как это header
            column.elements.Add(headerRowElement);
            
            // Добавить во все остальные строки по одному элементу
            foreach (var row in rows)
            {
                var element = row.AddElement(column.name, "", column.width);
                element.Column = columns.Count;
                column.elements.Add(element);
            }
        }
        
        private void Reset()
        {
            foreach (var row in rows)
            {
                DestroyImmediate(row.gameObject);
            }
            
            rows.Clear();
            
            if (headerRow == null)
            {
                headerRow = Instantiate(rowPrefab, transform);
            }
            headerRow.Clear();
            headerRow.Row = 0;
        }
    }
}