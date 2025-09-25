using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Style
{
    [CreateAssetMenu()]
    public class SelectableStyle : ScriptableObject
    {
        [SerializeField] private ColorBlock colorBlock;
        [SerializeField] private TMP_ColorGradient normalColor;
        [SerializeField] private TMP_ColorGradient highlightedColor;
        [SerializeField] private TMP_ColorGradient pressedColor;
        [SerializeField] private TMP_ColorGradient selectedColor;
        [SerializeField] private TMP_ColorGradient disabledColor;
        
        public ColorBlock ColorBlock => colorBlock;

        private void OnValidate()
        {
            colorBlock = new ColorBlock
            {
                normalColor = normalColor? normalColor.bottomLeft : Color.black,
                highlightedColor = highlightedColor ? highlightedColor.bottomLeft : Color.black,
                pressedColor = pressedColor ? pressedColor.bottomLeft : Color.black,
                selectedColor = selectedColor ? selectedColor.bottomLeft : Color.black,
                disabledColor = disabledColor ? disabledColor.bottomLeft : Color.black,
                colorMultiplier = colorBlock.colorMultiplier,
                fadeDuration = colorBlock.fadeDuration
            };
        }
    }
}