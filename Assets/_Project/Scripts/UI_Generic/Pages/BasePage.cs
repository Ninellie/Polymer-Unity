using UnityEngine;

namespace Polymer.UI.Pages
{
    /// <summary>
    /// Базовый класс для всех страниц приложения
    /// </summary>
    public abstract class BasePage : MonoBehaviour
    {
        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        public virtual void Refresh()
        {
            // Переопределяется в наследниках для обновления данных
        }
    }
}
