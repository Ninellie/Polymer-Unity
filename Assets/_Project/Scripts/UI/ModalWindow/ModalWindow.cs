using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.DevicePage
{
    public class ModalWindow : MonoBehaviour
    {
        [SerializeField] private ModalWindowField fieldPrefab;
        
        [Header("Inner components links")]
        [SerializeField] private VerticalLayoutGroup contentGroup;
        [SerializeField] private Button submitButton;
        [SerializeField] private Button cancelButton;
        
        public Action<Dictionary<string, string>> OnSubmit; 
        
        private List<ModalWindowField> _fields;
        private Dictionary<string, string> _fieldsData;
        
        private void Awake()
        {
            if (fieldPrefab == null) Debug.LogError("Modal window prefab is null");
            if (contentGroup == null) Debug.LogError("Vertical layout content group is null");
            if (submitButton == null) Debug.LogError("Submit button is null");
            if (cancelButton == null) Debug.LogError("Cancel button is null");
            
            _fieldsData = new Dictionary<string, string>();
            _fields = new List<ModalWindowField>();
        }
        
        private void OnEnable()
        {
            submitButton.onClick.AddListener(() => OnSubmit?.Invoke(_fieldsData));
            submitButton.onClick.AddListener(() => Destroy(gameObject));
            
            cancelButton.onClick.AddListener(() => Destroy(gameObject));
        }

        private void OnDisable()
        {
            submitButton.onClick.RemoveAllListeners();
        }
        
        public void AddField(string key, string value)
        {
            var field = Instantiate(fieldPrefab, contentGroup.transform);
            field.SetFieldData(key, value);
            _fields.Add(field);
            _fieldsData.Add(key, value);
            
            field.InputField.onValueChanged.AddListener(newValue => _fieldsData[key] = newValue);
        }
    }
}