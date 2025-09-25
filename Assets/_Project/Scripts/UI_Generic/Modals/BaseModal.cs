using UnityEngine;
using UnityEngine.UI;

namespace Polymer.UI.Modals
{
    /// <summary>
    /// Базовый класс для всех модальных окон
    /// </summary>
    public abstract class BaseModal : MonoBehaviour
    {
        protected GameObject _modalBackground;
        protected GameObject _modalContent;

        public virtual void Show()
        {
            gameObject.SetActive(true);
            CreateModalBackground();
            CreateModalContent();
        }

        public virtual void Hide()
        {
            Destroy(gameObject);
        }

        protected virtual void CreateModalBackground()
        {
            // Создаем затемняющий фон
            _modalBackground = new GameObject("ModalBackground");
            _modalBackground.transform.SetParent(transform, false);

            RectTransform bgRect = _modalBackground.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;

            Image bgImage = _modalBackground.AddComponent<Image>();
            bgImage.color = new Color(0f, 0f, 0f, 0.7f); // Полупрозрачный чёрный

            // Добавляем обработчик клика для закрытия
            Button bgButton = _modalBackground.AddComponent<Button>();
            bgButton.onClick.AddListener(Hide);
        }

        protected abstract void CreateModalContent();
    }
}
