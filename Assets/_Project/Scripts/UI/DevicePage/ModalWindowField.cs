using TMPro;
using UnityEngine;

namespace UI.DevicePage
{
    public class ModalWindowField : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private TMP_Text placeholder;

        public TMP_InputField InputField => inputField;
        
        public void SetFieldData(string key, string value)
        {
            placeholder.SetText(key);
            inputField.text = value;
        }
        
        private void Awake()
        {
            if (inputField == null)
            {
                Debug.LogError("Input Field is null");
            }

            if (placeholder == null)
            {
                Debug.LogError("Placeholder is null");
            }
        }
    }
}