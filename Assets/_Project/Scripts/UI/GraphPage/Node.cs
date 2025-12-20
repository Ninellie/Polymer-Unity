using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.DevicePage
{
    [RequireComponent(typeof(CanvasRenderer))]
    [RequireComponent(typeof(RectTransform))]
    public class Node : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private float fadeAlpha = 0.3f;
        [SerializeField] private float fadeDuration = 1f;
        
        public int id;
        public float weight;
        public ForceDirectedLayout layout;
        public CircleDrawer drawer;
        public TextMeshProUGUI label;
        
        public Vector2 velocity;
        public Vector2 force;
        
        public bool isDragged;

        public List<Node> linkedNodes = new();

        public RectTransform RectTransform => drawer.rectTransform;
        
        public void OnDrag(PointerEventData eventData)
        {
            transform.position = eventData.position;
            layout.StartSimulation();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Debug.Log("drag" + name);
            isDragged = true;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            isDragged = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            layout.HoverNode(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            layout.UnhoverNode(this);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                layout.SetSelectedNode(this);
            }
        }
        
        public void Fade()
        {
            drawer.color = new Color(drawer.color.r, drawer.color.g, drawer.color.b, fadeAlpha);
            label.color =  new Color(label.color.r, label.color.g, label.color.b, fadeAlpha);
            
            // if (_tween.IsActive())
            // {
            //     _tween.Kill();
            // }
            //
            // _tween = this.DOFade(fadeAlpha, fadeDuration);
        }

        public void UndoFade()
        {
            drawer.color = new Color(drawer.color.r, drawer.color.g, drawer.color.b, 1);
            label.color =  new Color(label.color.r, label.color.g, label.color.b, 1);
            
            // if (_tween.IsActive())
            // {
            //     _tween.Kill();
            // }
            //
            // _tween = this.DOFade(1, fadeDuration);
        }
    }
}