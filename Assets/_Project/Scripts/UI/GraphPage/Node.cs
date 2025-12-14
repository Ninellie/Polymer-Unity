using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.DevicePage
{
    [RequireComponent(typeof(CanvasRenderer))]
    [RequireComponent(typeof(RectTransform))]
    public class Node : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public int id;
        
        public CircleDrawer drawer;
        public TextMeshProUGUI label;
        
        public Vector2 velocity;
        public Vector2 force;
        
        public bool isDragged;

        public RectTransform RectTransform => drawer.rectTransform;
        
        public void OnDrag(PointerEventData eventData)
        {
            transform.position = eventData.position;
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
    }
}