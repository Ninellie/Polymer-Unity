using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.DevicePage
{
    public class NodeFactory : MonoBehaviour
    {
        [SerializeField] private TextAsset jsonFile;
        [SerializeField] private NetworkModel model;
        [SerializeField] private Node prefab;
        [SerializeField] private RectTransform container;
        [SerializeField] private float creationGap;
        
        public List<Node> Nodes { get; } = new();
        public List<Edge> Edges { get; } = new();
        
        private void Awake()
        {
            if (jsonFile == null) return;
            model = JsonUtility.FromJson<NetworkModel>(jsonFile.text);
            Debug.Log($"JSON: devices count: {model.devices.Count}. Connections count: {model.connections.Count}");

            StartCoroutine(CreateNodes());
        }
            
        private IEnumerator CreateNodes()
        {
            foreach (var device in model.devices)
            {
                yield return new WaitForSeconds(creationGap);
                var node = Instantiate(prefab, container);
                node.RectTransform.anchoredPosition += Random.insideUnitCircle.normalized * Random.Range(100, 500);
                node.id = device.id;
                node.label.text = device.name;
                node.force = Vector2.zero;
                node.velocity = Vector2.zero;
                
                if (ColorUtility.TryParseHtmlString(device.color, out var color))
                {
                    node.drawer.color *= color;
                }
                
                Nodes.Add(node);
            }
            
            foreach (var connection in model.connections)
            { 
                yield return new WaitForSeconds(creationGap);
                var edge = new Edge();
                edge.a = Nodes.Find(x => x.id == connection.a);
                edge.b = Nodes.Find(x => x.id == connection.b);
                Edges.Add(edge);
            }
        }
    }
}