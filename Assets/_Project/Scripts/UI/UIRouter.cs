using UnityEngine;

namespace Polymer.UI
{
    public class UIRouter
    {
        private readonly PageVisitHistory _history;

        public UIRouter(PageVisitHistory history)
        {
            _history = history;
        }
        
        public void GoToPage(string route)
        {
            _history.Visit(route);
            GetPagePrefabByRoute(route);
        }

        public void Back()
        {
            var backRoute = _history.Back();
            GetPagePrefabByRoute(backRoute);
        }

        public GameObject Forward()
        {
            var forwardRoute = _history.Forward();
            return GetPagePrefabByRoute(forwardRoute);
        }

        private GameObject GetPagePrefabByRoute(string route)
        {
            return new GameObject();
        }
        
    }
}