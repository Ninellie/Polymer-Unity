using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Style
{
    public class ElementStyler : MonoBehaviour
    {
        [SerializeField] private TMP_ColorGradient color;

        private Image _image;

        private void OnValidate()
        {
            if (color == null)
            {
                return;
            }

            if (_image == null)
            {
                if (!TryGetComponent(out Image image)) return;
                _image = image;
            }

            _image.color = color.bottomLeft;
        }

        public void SetStyle(TMP_ColorGradient colorGradient)
        {
            color = colorGradient;
            OnValidate();
        }
    }
}