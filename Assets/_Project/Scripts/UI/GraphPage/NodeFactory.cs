using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace UI.DevicePage
{
    public class NodeFactory : MonoBehaviour
    {
        [SerializeField] private TextAsset jsonFile;
        [SerializeField] private ForceDirectedLayout layout;
        [SerializeField] private NetworkModel model;
        [SerializeField] private RectTransform container;
        [SerializeField] private float nodeWeighPerEdge = 100;
        [SerializeField] private float nodeRadiusPerEdge = 1;
        [SerializeField] private float creationGap = 0.1f;

        private void Awake()
        {
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

            var nodes = Graph.Instance.Nodes;
            
            foreach (var device in model.devices)
            {
                yield return new WaitForSeconds(creationGap);
                var node = new Node();
                node.Device = device;
                node.Position += Random.insideUnitCircle.normalized * Random.Range(100, 500);
                node.Id = device.id;
                node.Weight = 1;
                node.Radius = 15;
                
                if (ColorUtility.TryParseHtmlString(device.color, out var color))
                {
                    node.Color = color;
                }
                
                Graph.Instance.Nodes.Add(node);
                layout.StartSimulation();
            }
            
            foreach (var connection in model.connections)
            { 
                yield return new WaitForSeconds(creationGap);
                var a = nodes[connection.a];
                var b = nodes[connection.b];
                
                a.Weight += nodeWeighPerEdge;
                b.Weight += nodeWeighPerEdge;
                
                a.ConnectedNodes.Add(b);
                b.ConnectedNodes.Add(a);
                
                a.Radius += nodeRadiusPerEdge;
                b.Radius += nodeRadiusPerEdge;

                Graph.Instance.Connections.Add((a, b));
                
                layout.StartSimulation();
            }
        }
    }
}