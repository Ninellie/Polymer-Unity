using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Polymer.UI.GraphPage
{
    public class SettingInputField : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private TextMeshProUGUI title;

        public void Init(string titleText, float value, UnityAction<string> callback)
        {
            title.text = titleText;
            inputField.text = value.ToString();
            inputField.onEndEdit.AddListener(callback);
        }
    }
}