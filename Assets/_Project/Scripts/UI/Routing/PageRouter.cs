using System;
using UnityEngine;
using VContainer.Unity;

namespace Polymer.UI.Routing
{
    public class PageRouter : IInitializable
    {
        private readonly RouteTable _routeTable;
        private readonly PageResolver _pageResolver;
        private readonly PageVisitHistory _history;
        
        private readonly PageRoutingSettings _settings;
        
        public PageRouter(RouteTable routeTable, PageResolver pageResolver, PageRoutingSettings settings,
            PageVisitHistory history)
        {
            _routeTable = routeTable;
            _pageResolver = pageResolver;
            _settings = settings;
            _history = history;
        }
        
        public void Initialize()
        {
            var initialPath = _settings.initialPagePath;
            
            var initialMatch = _routeTable.Match(initialPath)
                               ?? throw new Exception($"Invalid initial route: {initialPath}");

            var initialVisit = new PageVisit(initialPath, initialMatch.PrefabPath, initialMatch.TypedParameters);
            _history.Init(initialVisit);

            ShowPage(initialVisit);
        }

        /// <summary>
        /// Перейти на страницу
        /// </summary>
        /// <param name="path">Путь к странице, может содержать аргументы</param>
        public void Navigate(string path)
        {
            var match = _routeTable.Match(path);
            if (match == null)
            {
                Debug.LogError($"[PageRouter] Route not found or parameters invalid: {path}");
                return;
            }

            var visit = new PageVisit(path, match.PrefabPath, match.TypedParameters);
            _history.Visit(visit);
            ShowPage(visit);
        }

        public void Back()
        {
            var visit = _history.Back();
            ShowPage(visit);
        }

        public void Forward()
        {
            var visit = _history.Forward();
            ShowPage(visit);
        }

        private void ShowPage(PageVisit visit)
        {
            var page = _pageResolver.Resolve(visit.PrefabPath, visit.Parameters);
            if (page == null)
            {
                Debug.LogError($"[PageRouter] Failed to resolve page: {visit.Path}");
            }
        }
    }
}