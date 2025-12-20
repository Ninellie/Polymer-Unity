using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

namespace UI.DevicePage
{
    [RequireComponent(typeof(CanvasRenderer))]
    [RequireComponent(typeof(RectTransform))]
    public class CircleDrawer : Graphic
    {
        [SerializeField] private int segments = 64;
        [SerializeField] private float lineWidth = 2f;
        
        private TweenerCore<Color, Color, ColorOptions> _tween;
        
        public float Radius
        {
            get => rectTransform.rect.size.magnitude / 2;
            set
            {
                float diameter;
                
                if (value == 0)
                {
                    diameter = 0;
                }
                else
                { 
                    diameter = value / 2;
                }

                rectTransform.sizeDelta = new Vector2(diameter, diameter);
            }
        }
        
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            if (segments < 3) segments = 3;
        
            var angleStep = 2 * Mathf.PI / segments;
            var center = Vector2.zero;
        
            // Создание вершин внешнего и внутреннего радиуса для линий
            for (var i = 0; i < segments; i++)
            {
                var angle = i * angleStep;
                var cos = Mathf.Cos(angle);
                var sin = Mathf.Sin(angle);
                
                var outer = center + new Vector2(cos, sin) * Radius;
                var inner = center + new Vector2(cos, sin) * 0f; 
                    // (Radius - lineWidth);
            
                var vOuter = UIVertex.simpleVert;
                vOuter.color = color;
                vOuter.position = outer;
        
                var vInner = UIVertex.simpleVert;
                vInner.color = color;
                vInner.position = inner;
        
                vh.AddVert(vInner);
                vh.AddVert(vOuter);
            }
        
            // Соединение линий треугольниками
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
    }
}