using System.Collections.Generic;
using Polymer.UI.Routing;

namespace Polymer.UI
{
    /// <summary>
    /// Хранит и позволяет просматривать историю посещений страниц. 
    /// </summary>
    public class PageVisitHistory
    {
        private readonly Stack<PageVisit> _history = new();
        private readonly Stack<PageVisit> _forwardHistory = new();
        private PageVisit _current;
        
        public void Init(PageVisit initialVisit)
        {
            _history.Clear();
            _forwardHistory.Clear();
            _current = initialVisit;
        }

        public PageVisit Back()
        {
            if (_history.Count == 0)
                return _current;

            _forwardHistory.Push(_current);
            _current = _history.Pop();
            return _current;
        }

        public PageVisit Forward()
        {
            if (_forwardHistory.Count == 0)
                return _current;

            _history.Push(_current);
            _current = _forwardHistory.Pop();
            return _current;
        }

        public void Visit(PageVisit visit)
        {
            _forwardHistory.Clear();
            _history.Push(_current);
            _current = visit;
        }
    }
}