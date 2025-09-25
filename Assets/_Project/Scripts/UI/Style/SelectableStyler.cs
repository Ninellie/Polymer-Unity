using UnityEngine;
using UnityEngine.UI;

namespace UI.Style
{
    public class SelectableStyler : MonoBehaviour
    {
        [SerializeField] private SelectableStyle style;

        private Selectable _selectable;

        private void OnValidate()
        {
            if (style == null)
            {
                Debug.LogWarning("Style is null");
                return;
            }

            if (_selectable == null)
            {
                if (!TryGetComponent(out Selectable selectable)) return;
                _selectable = selectable;
            }

            _selectable.colors = style.ColorBlock;
        }

        public void SetStyle(SelectableStyle newStyle)
        {
            style = newStyle;
            OnValidate();
        }
    }
}