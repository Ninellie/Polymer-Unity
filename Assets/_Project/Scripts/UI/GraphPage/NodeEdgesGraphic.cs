// using System.Collections.Generic;
// using TriInspector;
// using UnityEngine;
// using UnityEngine.UI;
//
// namespace UI.DevicePage
// {
//     [RequireComponent(typeof(CanvasRenderer))]
//     [RequireComponent(typeof(RectTransform))]
//     public class NodeEdgesGraphic : MaskableGraphic
//     {
//         [SerializeField] private NodeFactory factory;
//         [SerializeField] private float lineWidth = 1f;
//         
//         private List<Edge> _edges;
//         [SerializeField] [ReadOnly] private  bool _isInit;
//
//         
//         protected override void Start()
//         {
//             base.Start();
//             _edges = factory.Edges;
//             _isInit = true;
//         }
//         
//         private void Update()
//         {
//             // Перестроить меш каждый кадр, чтобы линии следовали за узлами
//             if (!_isInit) return;
//             SetVerticesDirty();
//         }
//         
//         protected override void OnPopulateMesh(VertexHelper vh)
//         {
//             vh.Clear();
//
//             foreach (var edge in _edges)
//             {
//                 var aNode = edge.a;
//                 var bNode = edge.b;
//
//                 var alpha = Mathf.Min(edge.a.drawer.color.a, edge.b.drawer.color.a);
//                 
//                 if (aNode == null || bNode == null) continue;
//
//                 DrawUILine(vh, aNode.RectTransform.anchoredPosition,
//                     bNode.RectTransform.anchoredPosition, lineWidth, new Color(color.r, color.g, color.b, alpha));
//             }
//         }
//
//         private static void DrawUILine(VertexHelper vh, Vector2 start, Vector2 end, float width, Color col)
//         {
//             var dir = (end - start).normalized;
//             var normal = new Vector2(-dir.y, dir.x) * width * 0.5f;
//
//             var v0 = UIVertex.simpleVert;
//             v0.color = col;
//             v0.position = start - normal;
//
//             var v1 = UIVertex.simpleVert;
//             v1.color = col;
//             v1.position = start + normal;
//
//             var v2 = UIVertex.simpleVert;
//             v2.color = col;
//             v2.position = end + normal;
//
//             var v3 = UIVertex.simpleVert;
//             v3.color = col;
//             v3.position = end - normal;
//
//             var idx = vh.currentVertCount;
//
//             vh.AddVert(v0);
//             vh.AddVert(v1);
//             vh.AddVert(v2);
//             vh.AddVert(v3);
//
//             vh.AddTriangle(idx, idx + 1, idx + 2);
//             vh.AddTriangle(idx, idx + 2, idx + 3);
//         }
//     }
// }