﻿using UnityEngine;
using VContainer;

namespace Polymer.UI.Routing
{
    /// <summary>
    /// Базовый класс UI страниц. Каждая страница должна наследователь этот класс.
    /// </summary>
    public abstract class PageBase : MonoBehaviour
    {
        [Inject]
        public PageRouter Router { get; }
        
        public abstract void OnPageInit(PageArgs args);
    }
}