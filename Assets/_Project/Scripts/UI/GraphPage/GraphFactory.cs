using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Models;
using FDLayout;
using UnityEngine;
using VContainer;
using Random = UnityEngine.Random;

namespace Polymer.UI.GraphPage
{
    public class GraphFactory : MonoBehaviour
    {
        [SerializeField] private ForceDirectedLayoutPage layoutPage;
        [SerializeField] private RectTransform container;
        [SerializeField] private float baseRadius = 15f;
        [SerializeField] private float creationGap = 0.1f;

        [Inject] private ApplicationData _appData;
        [Inject] private ForceDirectedLayout _layout;
        [Inject] private List<Node> _nodes;
        [Inject] private List<(Node a, Node b)> _connections;

        private Coroutine _creating;
        
        private void Start()
        {
            RecreateNodes();
        }

        public void RecreateNodes()
        {
            _nodes.Clear();
            _connections.Clear();
            
            if (_creating != null)
            {
                StopCoroutine(_creating);
                _creating = null;
            }

            _creating = StartCoroutine(Create());
        }
        
        private IEnumerator Create()
        {
            yield return new WaitWhile(() => !_appData.Loaded && string.IsNullOrEmpty(_appData.LoadError));
            if (!string.IsNullOrEmpty(_appData.LoadError))
            {
                Debug.LogError("[GraphFactory] Stopping graph build: " + _appData.LoadError);
                yield break;
            }

            var uniqueConnections = new HashSet<(int minId, int maxId)>();

            foreach (var device in _appData.Devices)
            {
                yield return new WaitForSeconds(creationGap);
                var node = new Node();
                node.Position += Random.insideUnitCircle.normalized * Random.Range(100, 500);
                node.Id = device.Id;
                node.BaseRadius = baseRadius;
                ApplyRoleColor(node, device);
                _nodes.Add(node);
                _layout.Start();
            }
            
            foreach (var connection in _appData.Cables)
            { 
                yield return new WaitForSeconds(creationGap);
                var a = _nodes.First(node => node.Id == connection.FromDeviceId);
                var b = _nodes.First(node => node.Id == connection.ToDeviceId);
                
                a.Links.Add(b);
                b.Links.Add(a);

                var minId = Mathf.Min(a.Id, b.Id);
                var maxId = Mathf.Max(a.Id, b.Id);
                if (!uniqueConnections.Add((minId, maxId)))
                {
                    continue;
                }

                _connections.Add((a, b));
                
                _layout.Start();
            }
        }

        private static void ApplyRoleColor(Node node, Device device)
        {
            var role = device.Role;
            if (role == null) return;

            if (string.IsNullOrEmpty(role.Color)) return;

            if (!ColorUtility.TryParseHtmlString(role.Color, out var parsed)) return;

            node.Color = parsed;
            node.DisplayColor = parsed;
            node.TargetDisplayColor = parsed;
        }
    }
}