using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.DevicePage
{
    [RequireComponent(typeof(CanvasRenderer))]
    [RequireComponent(typeof(RectTransform))]
    public class NodeConnectionsGraphic : MaskableGraphic
    {
        public List<Connection> Connections { get; set; }
        public Dictionary<int, UINode> Nodes { get; set; }
        public float lineWidth = 1f;

        private void Update()
        {
            // Перестроить меш каждый кадр, чтобы линии следовали за узлами
            SetVerticesDirty();
        }
        
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            foreach (var connection in Connections)
            {
                var aNode = Nodes[connection.a];
                var bNode = Nodes[connection.b];

                if (aNode == null || bNode == null) continue;

                DrawUILine(vh, aNode.RectTransform.anchoredPosition,
                    bNode.RectTransform.anchoredPosition, lineWidth, color);
            }
        }

        private static void DrawUILine(VertexHelper vh, Vector2 start, Vector2 end, float width, Color col)
        {
            var dir = (end - start).normalized;
            var normal = new Vector2(-dir.y, dir.x) * width * 0.5f;

            var v0 = UIVertex.simpleVert;
            v0.color = col;
            v0.position = start - normal;

            var v1 = UIVertex.simpleVert;
            v1.color = col;
            v1.position = start + normal;

            var v2 = UIVertex.simpleVert;
            v2.color = col;
            v2.position = end + normal;

            var v3 = UIVertex.simpleVert;
            v3.color = col;
            v3.position = end - normal;

            var idx = vh.currentVertCount;

            vh.AddVert(v0);
            vh.AddVert(v1);
            vh.AddVert(v2);
            vh.AddVert(v3);

            vh.AddTriangle(idx, idx + 1, idx + 2);
            vh.AddTriangle(idx, idx + 2, idx + 3);
        }
    }
}