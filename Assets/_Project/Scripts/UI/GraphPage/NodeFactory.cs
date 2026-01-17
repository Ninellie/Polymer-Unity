using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Services;
using UnityEngine;

namespace UI.DevicePage
{
    public class NodeFactory : MonoBehaviour
    {
        [SerializeField] private TextAsset jsonFile;
        [SerializeField] private ForceDirectedLayout layout;
        [SerializeField] private NetworkModel model;
        [SerializeField] private Node prefab;
        [SerializeField] private RectTransform container;
        [SerializeField] private float nodeWeighPerEdge = 100;
        [SerializeField] private float nodeRadiusPerEdge = 1;
        [SerializeField] private float creationGap = 0.1f;
        
        public List<Node> Nodes { get; } = new();
        public List<Edge> Edges { get; } = new();
        
        private void Awake()
        {
            // if (jsonFile == null) return;
            // model = JsonUtility.FromJson<NetworkModel>(jsonFile.text);
            // Debug.Log($"JSON: devices count: {model.devices.Count}. Connections count: {model.connections.Count}");

            StartCoroutine(CreateNodes());
        }
            
        private IEnumerator CreateNodes()
        {
            Debug.Log("Node creating started");
            Debug.Log("Getting devices");
            var task = NetBoxDataProvider.GetNetworkModel();
            yield return new WaitUntil(() => task.Status == TaskStatus.RanToCompletion);
            model = task.Result;
            Debug.Log($"GetDevices task is complete. Device count: {model.devices.Count}. Connections: {model.connections.Count}");
            
            foreach (var device in model.devices)
            {
                yield return new WaitForSeconds(creationGap);
                var node = Instantiate(prefab, container);
                node.name = device.name;
                node.layout = layout;
                node.RectTransform.anchoredPosition += Random.insideUnitCircle.normalized * Random.Range(100, 500);
                node.id = device.id;
                node.label.text = device.name;
                node.force = Vector2.zero;
                node.velocity = Vector2.zero;
                node.weight = 1;
                
                if (ColorUtility.TryParseHtmlString(device.color, out var color))
                {
                    node.drawer.color = color;
                }
                
                Nodes.Add(node);
                layout.StartSimulation();
            }
            
            foreach (var connection in model.connections)
            { 
                yield return new WaitForSeconds(creationGap);
                var edge = new Edge();
                edge.a = Nodes.Find(x => x.id == connection.a);
                edge.b = Nodes.Find(x => x.id == connection.b);
                
                edge.a.weight += nodeWeighPerEdge;
                edge.b.weight += nodeWeighPerEdge;
                
                edge.a.linkedNodes.Add(edge.b);
                edge.b.linkedNodes.Add(edge.a);
                
                edge.a.RectTransform.sizeDelta += Vector2.one * nodeRadiusPerEdge;
                edge.b.RectTransform.sizeDelta += Vector2.one * nodeRadiusPerEdge;
                
                Edges.Add(edge);
                layout.StartSimulation();
            }
        }
    }
}