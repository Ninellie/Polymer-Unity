using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Polymer.UI.GraphPage
{
    public class SettingSlider : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private TextMeshProUGUI title;

        public void Init(string titleText, float value, float minValue, float maxValue, UnityAction<float> callback)
        {
            title.text = titleText;
            slider.value = value;
            slider.minValue = minValue;
            slider.maxValue = maxValue;
            slider.onValueChanged.AddListener(callback);
        }
    }
}