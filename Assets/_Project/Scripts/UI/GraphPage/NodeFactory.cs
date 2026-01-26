using System.Collections;
using System.Threading.Tasks;
using Core.Models;
using FDLayout;
using UnityEngine;
using VContainer;

namespace UI.DevicePage
{
    public class NodeFactory : MonoBehaviour
    {
        [SerializeField] private TextAsset jsonFile;
        [SerializeField] private ForceDirectedLayoutPage layoutPage;
        [SerializeField] private RectTransform container;
        [SerializeField] private float nodeWeighPerEdge = 100;
        [SerializeField] private float nodeRadiusPerEdge = 1;
        [SerializeField] private float creationGap = 0.1f;

        [Inject]
        private ApplicationData _appData;
        
        private void Awake()
        {
            StartCoroutine(CreateNodes());
        }
        
        private IEnumerator CreateNodes()
        {
            var task = NetBoxDataLoader.GetNetworkModel();
            yield return new WaitUntil(() => task.Status == TaskStatus.RanToCompletion);
            
            var data = task.Result;
            Debug.Log($"GetDevices task is complete. Device count: {data.devices.Count}. Connections: {data.connections.Count}");

            var nodes = Graph.Instance.Nodes;
            nodes.Clear();
            
            foreach (var device in data.devices)
            {
                yield return new WaitForSeconds(creationGap);
                var node = new Node();
                node.Position += Random.insideUnitCircle.normalized * Random.Range(100, 500);
                node.Id = device.id;
                node.Weight = 1;
                node.Radius = 15;
                
                if (ColorUtility.TryParseHtmlString(device.color, out var color))
                {
                    node.Color = color;
                }
                
                Graph.Instance.Nodes.Add(node);
                layoutPage.StartSimulation();
            }
            
            foreach (var connection in data.connections)
            { 
                yield return new WaitForSeconds(creationGap);
                var a = nodes[connection.a];
                var b = nodes[connection.b];
                
                a.Weight += nodeWeighPerEdge;
                b.Weight += nodeWeighPerEdge;
                
                a.Links.Add(b);
                b.Links.Add(a);
                
                a.Radius += nodeRadiusPerEdge;
                b.Radius += nodeRadiusPerEdge;

                Graph.Instance.Connections.Add((a, b));
                
                layoutPage.StartSimulation();
            }
        }
    }
}