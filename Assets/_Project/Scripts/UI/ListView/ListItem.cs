using System;
using TMPro;
using TriInspector;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ListView
{
    public class ListItem : MonoBehaviour
    {
        [SerializeField] [ReadOnly] private int id;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private LayoutElement layoutElement;
        [SerializeField] private Toggle toggle;
        
        public int Id => id;
        public Toggle Toggle => toggle;
        public Action<int> OnItemSelected;
        public Action<int> OnItemDeselected;
        
        private string _title;
        private string _subTitle;
        
        private void OnEnable()
        {
            _title = string.Empty;
            _subTitle = string.Empty;
            toggle.onValueChanged.AddListener(InvokeSelectAction);
        }

        public ListItem SetId(int itemId)
        {
            id = itemId;
            return this;
        }

        public ListItem SetTitle(string title)
        {
            _title = title;
            Refresh();
            return this;
        }
        
        public ListItem SetSubTitle(string subTitle)
        {
            _subTitle = subTitle;
            Refresh();
            return this;
        }

        private void Refresh()
        {
            inputField.text = $"{_title} - {_subTitle}";
        }
        
        private void InvokeSelectAction(bool isSelected)
        {
            if (isSelected)
            {
                OnItemSelected?.Invoke(id);
            }
            else
            {
                OnItemDeselected?.Invoke(id);
            }
        }
    }
}