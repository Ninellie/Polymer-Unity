using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Polymer.UI.Routing
{
    /// <summary>
    /// Отвечает за создание и инициализацию страниц.
    /// </summary>
    public class PageResolver
    {
        private readonly Transform _root;
        private readonly IObjectResolver _resolver;
        
        public PageResolver([Key("PageRoot")] Transform root, IObjectResolver resolver)
        {
            _root = root;
            _resolver = resolver;
        }
        
        /// <summary>
        /// Находит и возвращает в ресурсах нужную страницу, создаёт инстанс и даёт ей параметры.
        /// </summary>
        public PageBase Resolve(string prefabPath, Dictionary<string, object> parameters)
        {
            var prefab = Resources.Load<GameObject>(prefabPath);
            
            if (prefab == null)
            {
                Debug.LogError($"[PageResolver] Prefab not found at path: {prefabPath}");
                return null;
            }

            var instance = Object.Instantiate(prefab, _root);
            _resolver.InjectGameObject(instance);
            var page = instance.GetComponent<PageBase>();
            if (page == null)
            {
                Debug.LogError($"[PageResolver] Prefab at path '{prefabPath}' does not contain PageBase component.");
                return null;
            }

            page.OnPageInit(new PageArgs(parameters));
            return page;
        }
    }
}