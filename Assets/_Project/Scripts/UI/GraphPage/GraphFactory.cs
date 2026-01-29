using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Models;
using FDLayout;
using UnityEngine;
using VContainer;

namespace Polymer.UI.GraphPage
{
    public class GraphFactory : MonoBehaviour
    {
        [SerializeField] private ForceDirectedLayoutPage layoutPage;
        [SerializeField] private RectTransform container;
        [SerializeField] private float nodeWeighPerEdge = 100;
        [SerializeField] private float nodeRadiusPerEdge = 1;
        [SerializeField] private float creationGap = 0.1f;

        public List<Node> Nodes { get; } = new();
        public List<(Node a, Node b)> Connections { get; } = new();
        
        [Inject] private ApplicationData _appData; 
        
        private void Start()
        {
            StartCoroutine(CreateNodes());
        }
        
        private IEnumerator CreateNodes()
        {
            yield return new WaitWhile(() => !_appData.Loaded);
            
            Nodes.Clear();
            Connections.Clear();
            
            foreach (var device in _appData.Devices)
            {
                yield return new WaitForSeconds(creationGap);
                var node = new Node();
                node.Position += Random.insideUnitCircle.normalized * Random.Range(100, 500);
                node.Id = device.Id;
                node.Weight = 1;
                node.Radius = 15;
                
                if (ColorUtility.TryParseHtmlString(device.Role.Color, out var color))
                {
                    node.Color = color;
                }
                
                Nodes.Add(node);
                layoutPage.StartSimulation();
            }
            
            foreach (var connection in _appData.Cables)
            { 
                yield return new WaitForSeconds(creationGap);
                var a = Nodes.First(node => node.Id == connection.FromDeviceId);
                var b = Nodes.First(node => node.Id == connection.ToDeviceId);
                
                a.Weight += nodeWeighPerEdge;
                b.Weight += nodeWeighPerEdge;
                
                a.Links.Add(b);
                b.Links.Add(a);
                
                a.Radius += nodeRadiusPerEdge;
                b.Radius += nodeRadiusPerEdge;

                Connections.Add((a, b));
                
                layoutPage.StartSimulation();
            }
        }
    }
}