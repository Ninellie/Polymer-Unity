using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.DevicePage
{
    [RequireComponent(typeof(CanvasRenderer))]
    [RequireComponent(typeof(RectTransform))]
    public class UINode : Graphic, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public Vector2 Velocity;
        // public RectTransform rectTransform => gameObject.GetComponent<RectTransform>();
        public float radius = 50f;
        public int segments = 64;
        public float lineWidth = 2f;
        public string label = string.Empty;
        public bool dragged;

        public Color color;
        
        // private void Awake()
        // {
        //     var a = gameObject.AddComponent<Image>();
        //     a.color = color;
        // }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            if (segments < 3) segments = 3;
        
            var angleStep = 2 * Mathf.PI / segments;
            var center = Vector2.zero;
        
            // Создаем вершины внешнего и внутреннего радиуса для линий
            for (var i = 0; i < segments; i++)
            {
                var angle = i * angleStep;
                var cos = Mathf.Cos(angle);
                var sin = Mathf.Sin(angle);
        
                var outer = center + new Vector2(cos, sin) * radius;
                var inner = center + new Vector2(cos, sin) * (radius - lineWidth);
        
                var vOuter = UIVertex.simpleVert;
                vOuter.color = color;
                vOuter.position = outer;
        
                var vInner = UIVertex.simpleVert;
                vInner.color = color;
                vInner.position = inner;
        
                vh.AddVert(vInner);
                vh.AddVert(vOuter);
            }
        
            // Соединяем линии треугольниками
            for (var i = 0; i < segments; i++)
            {
                var nextI = (i + 1) % segments;
        
                var i0 = i * 2;
                var i1 = i * 2 + 1;
                var i2 = nextI * 2;
                var i3 = nextI * 2 + 1;
        
                vh.AddTriangle(i0, i2, i3);
                vh.AddTriangle(i0, i3, i1);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = eventData.position;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Debug.Log("drag" + name);
            dragged = true;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            dragged = false;
        }
    }
}