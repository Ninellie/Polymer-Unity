using TMPro;
using TriInspector;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(TMP_Text))]
    [RequireComponent(typeof(LayoutElement))]
    public class TableElement : MonoBehaviour
    {
        [SerializeField] private TMP_Text tmpText;
        [SerializeField] private LayoutElement layoutElement;
        
        [Tooltip("Устанавливает фиксированную ширину элемента в строке.")]
        [SerializeField] private float elementWidth;
        
        [SerializeField] private TableRow tableRow;
        
        [SerializeField] [ReadOnly] private int column;
        [SerializeField] [ReadOnly] private int row;
        
        public int Column
        {
            get => column;
            set => column = value;
        }

        public int Row
        {
            get => row;
            set => row = value;
        }

        private void OnEnable()
        {
            Configure();
        }

        private void Configure()
        {
            tmpText = GetComponent<TMP_Text>();
            layoutElement = GetComponent<LayoutElement>();
            
            layoutElement.minHeight = -1;
            layoutElement.minWidth = -1;
            layoutElement.preferredHeight = -1;
            layoutElement.preferredWidth = elementWidth;
            layoutElement.flexibleHeight = -1;
            layoutElement.flexibleWidth = -1;
        }

        public void SetText(string text)
        {
            tmpText.text = text;
        }

        public void SetWidth(float width)
        {
            layoutElement.preferredWidth = width > 0f ? width : -1f;
        }
    }
}