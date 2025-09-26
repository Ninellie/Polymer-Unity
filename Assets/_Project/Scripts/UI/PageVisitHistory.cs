using System.Collections.Generic;

namespace Polymer.UI
{
    public class PageVisitHistory
    {
        private readonly Stack<string> _history;
        private readonly Stack<string> _forwardHistory;
        private string _currentPage;

        public PageVisitHistory(string currentPage)
        {
            _history = new Stack<string>();
            _forwardHistory = new Stack<string>();
            _currentPage = currentPage;
        }

        public string Back()
        {
            if (!_history.TryPop(out var previousPageRoute))
            {
                return _currentPage;
            }
            
            _forwardHistory.Push(previousPageRoute);
            _currentPage = previousPageRoute;
            return _currentPage;
        }

        
        public string Forward()
        {
            if (!_forwardHistory.TryPop(out var forwardPageRoute))
            {
                return _currentPage;
            }
            
            _history.Push(forwardPageRoute);
            _currentPage = forwardPageRoute;
            return _currentPage;
        }
        
        public void Visit(string route)
        {
            _forwardHistory.Clear();
            _currentPage = route;
            _history.Push(route);
        }
    }
}