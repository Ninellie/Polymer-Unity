using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.DevicePage
{
    [RequireComponent(typeof(CanvasRenderer))]
    [RequireComponent(typeof(RectTransform))]
    public class UINode : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public CircleDrawer drawer;
        public int id;
        public Vector2 velocity;
        public Vector2 force;
        public string label = string.Empty;
        public bool dragged;

        public DeviceRolesGraph Graph { get; set; }
        public RectTransform RectTransform => drawer.rectTransform;

        public void OnDrag(PointerEventData eventData)
        {
            Graph.RestartSimulation();
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