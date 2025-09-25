using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class EndlessScroll : MonoBehaviour, IBeginDragHandler, IEndDragHandler
    {

        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private HorizontalOrVerticalLayoutGroup contentLayoutGroup;
        [SerializeField] public UnityEvent<GameObject> onHorizontalShift;
        [SerializeField] public UnityEvent<GameObject> onVerticalShift;
        [SerializeField] private bool _dragging;
        [SerializeField] private float _baseElementHeight;
        
        private RectTransform _content;
        private float _spacing;
        private float _contentPosX;
        private float _contentPosY;
    
        private RectTransform _firstChild;
        private RectTransform _lastChild;
    
        private float _rightBorder;
        private float _leftBorder;
    
        private float _upBorder;
        private float _downBorder;
    
        private void Awake()
        {
            _content = scrollRect.content;
            _spacing = contentLayoutGroup.spacing;
        }
    
        private void OnEnable() => scrollRect.onValueChanged.AddListener(OnScrollMove);

        private void OnDisable() => scrollRect.onValueChanged.RemoveListener(OnScrollMove);

        public void OnBeginDrag(PointerEventData eventData) => _dragging = true;

        public void OnEndDrag(PointerEventData eventData) => _dragging = false;
        
        private void OnScrollMove(Vector2 delta)
        {
            var minElementsCountForScroll = Mathf.CeilToInt(scrollRect.viewport.rect.height / (_baseElementHeight + contentLayoutGroup.spacing)) + 1;
            Debug.Log($"min = {minElementsCountForScroll}");
            var elementsCount = _content.childCount;
            Debug.Log($"count = {elementsCount}");
            
            if (minElementsCountForScroll > elementsCount)
            {
                return;
            }
                
            if (_dragging)
            {
                return;
            }
        
            UpdateData();

            while (_contentPosY > _upBorder)
            {
                ShiftFrom(Side.Up);
                UpdateData();
            }
        
            while (_contentPosY < _downBorder)
            {
                ShiftFrom(Side.Down);
                UpdateData();
            }
        
            while (_contentPosX < _leftBorder)
            {
                ShiftFrom(Side.Left);
                UpdateData();
            }

            while (_contentPosX > _rightBorder)
            {
                ShiftFrom(Side.Right);
                UpdateData();
            }
        }
        
        // Правильная логика расчета границ:
        // Для горизонтальной прокрутки:
        // _rightBorder = _firstChild.rect.width - когда контент сдвинут вправо на ширину первого элемента
        // _leftBorder = -_lastChild.rect.width - когда контент сдвинут влево на ширину последнего элемента
        // Для вертикальной прокрутки:
        // _upBorder = _firstChild.rect.height - когда контент сдвинут вверх на высоту первого элемента
        // _downBorder = _lastChild.rect.height - когда контент сдвинут вниз на высоту последнего элемента
        // Логика работы:
        // Когда _contentPosY > _upBorder - контент слишком сдвинут вверх, нужно переместить первый элемент в конец
        // Когда _contentPosY < _downBorder - контент слишком сдвинут вниз, нужно переместить последний элемент в начало
        // Когда _contentPosX < _leftBorder - контент слишком сдвинут влево, нужно переместить первый элемент в конец
        // Когда _contentPosX > _rightBorder - контент слишком сдвинут вправо, нужно переместить последний элемент в начало
        
        private void UpdateData()
        {
            _contentPosX = _content.anchoredPosition.x;
            _contentPosY = _content.anchoredPosition.y;
        
            _firstChild = _content.GetChild(0) as RectTransform;
            var elementsCount = _content.childCount;
        
            _lastChild = _content.GetChild(elementsCount - 1) as RectTransform;
        
            if (_firstChild == null || _lastChild == null)
            {
                Debug.LogWarning("Content has no child with RectTransform component");
                return;
            }
        
            _rightBorder = _firstChild.rect.width;
            _leftBorder = -_lastChild.rect.width;
        
            _upBorder = _firstChild.rect.height;
            _downBorder = _lastChild.rect.height;
        }
    
        private void ShiftFrom(Side side)
        {
            var shiftedElement = ShiftContentElementFrom(side);
            var elementWidth = shiftedElement!.rect.width;
            var elementHeight = shiftedElement!.rect.height;
            var horizontalTranslation = elementWidth + _spacing;
            var verticalTranslation = elementHeight + _spacing;
        
            switch (side)
            {
                case Side.Left:
                    _content.anchoredPosition += new Vector2(horizontalTranslation, 0);
                    onHorizontalShift.Invoke(shiftedElement.gameObject);
                    break;
                case Side.Right:
                    _content.anchoredPosition -= new Vector2(horizontalTranslation, 0);
                    onHorizontalShift.Invoke(shiftedElement.gameObject);
                    break;
                case Side.Down:
                    _content.anchoredPosition += new Vector2(0, verticalTranslation);
                    onVerticalShift.Invoke(shiftedElement.gameObject);
                    break;
                case Side.Up:
                    _content.anchoredPosition -= new Vector2(0, verticalTranslation);
                    onVerticalShift.Invoke(shiftedElement.gameObject);
                    break;
            }
        }

        private RectTransform ShiftContentElementFrom(Side side)
        {
            if (side is Side.Left or Side.Up)
            {
                _firstChild!.SetAsLastSibling();
                return _firstChild;
            }
            _lastChild!.SetAsFirstSibling();
            return _lastChild;
        }
    }
}