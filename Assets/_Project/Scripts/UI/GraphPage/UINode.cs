using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.DevicePage
{
    [RequireComponent(typeof(CanvasRenderer))]
    [RequireComponent(typeof(RectTransform))]
    public class UINode : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public CircleDrawer drawer;
        public int id;
        public Vector2 velocity;
        public string label = string.Empty;
        public bool dragged;

        public RectTransform RectTransform => drawer.rectTransform;

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