using System.Collections.Generic;
using System.Linq;
using Polymer.UI.Routing;
using TMPro;
using TriInspector;
using UnityEditor;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace UI.DevicePage
{
    public class ForceDirectedLayoutForces
    {
        
    }
    
    
    [RequireComponent(typeof(RectTransform))]
    public class DeviceRolesGraph : PageBase
    {
        [Header("Json")]
        [SerializeField] private TextAsset jsonFile;
        private NetworkModel _model;
        
        private readonly Dictionary<int, UINode> nodes = new();
        private readonly HashSet<(UINode, UINode)> _edges = new();

        [Header("Graph Container")]
        [SerializeField] private RectTransform container;
        
        [Header("Test Node Creation")]
        [Range(1f, 100f)]  [SerializeField] private float testNodeRadius = 5f;
        
        [SerializeField] private Color testNodeColor = Color.white;
        [SerializeField] private Vector2 testNodeSize = new(100f, 100f);
        [SerializeField] private bool createTestNodesOnAwake = true;
        
        [Header("Physics")]
        [Range(0, 1f)] [SerializeField] private float repulsion = 0.5f;
        [Range(0, 100f)] [SerializeField] private float link = 0.5f;
        [Range(0, 1f)] [SerializeField] private float gravity = 0.5f;
        [Range(50, 500f)] [SerializeField] private float linkDistance = 50f;
        [SerializeField] private float extraRepulsionFactor = 70f;
        
        [Range(0.01f, 10f)] [SerializeField] private float deltaTimeMultiplier = 1f;
        [Range(1f, 10f)] [SerializeField] private float processCountPerFrame = 1;
        [Range(0.01f, 5f)] [SerializeField] private float stabilizationThreshold = 1f;
        [Range(0.1f, 10f)] [SerializeField] private float stabilizationTime = 5f;
        
        [Header("Blop")]
        [Range(0f, 500f)] [SerializeField] private float blopPower = 250f;
        [Range(0f, 500f)] [SerializeField] private float blopRadius = 150f;
        [SerializeField] private DistanceFalloff.Mode mode = DistanceFalloff.Mode.Linear;
        
        [Header("State")]
        [SerializeField] [ReadOnly] private SimulationState currentState = SimulationState.Simulating;
        [SerializeField] [ReadOnly] private float lastGravity;
        [SerializeField] [ReadOnly] private float lastRepulsion;
        [SerializeField] [ReadOnly] private float lastLink;
        [SerializeField] [ReadOnly] private float lastExtraRepulsionFactor;
        [SerializeField] [ReadOnly] private float globalDamping;
        [SerializeField] [ReadOnly] private float simulationElapsedTime;

        private InputAction _midClick;
        private InputAction _point;

        private Vector2 _previousBlopPosition = Vector2.zero;

        private void DeserializeData()
        {
            if (jsonFile == null) return;
            _model = JsonUtility.FromJson<NetworkModel>(jsonFile.text);
            Debug.Log($"JSON: devices count: {_model.devices.Count}. Connections count: {_model.connections.Count}");
        }
        
        private void Awake()
        {
            DeserializeData();
            
            lastRepulsion = repulsion;
            lastExtraRepulsionFactor = extraRepulsionFactor;
            lastLink = link;
            lastGravity = gravity;
            
            currentState = SimulationState.Simulating;
            globalDamping = 0f;
            simulationElapsedTime = 0f;
            
            if (createTestNodesOnAwake)
            {
                CreateTestNodes();
            }
        }
        
        private void Start()
        {
            var uiActionMap = InputSystem.actions.FindActionMap("UI");
            _midClick = uiActionMap.FindAction("Click");
            _point = uiActionMap.FindAction("Point");
            uiActionMap.Enable();
        }
        
        private void HandleClick()
        {
            if (!_midClick.WasReleasedThisFrame()) return;
            _previousBlopPosition = _point.ReadValue<Vector2>();
            _previousBlopPosition *= new Vector2(0.9f, 1f);
            _previousBlopPosition -= new Vector2(Screen.width, Screen.height) * 0.5f;
            
            foreach (var node in nodes)
            {
                if (node.Value.dragged)
                {
                    continue;
                }
                var nodePos = node.Value.RectTransform.anchoredPosition;
                var delta = nodePos - _previousBlopPosition;
                if (!(delta.magnitude < blopRadius)) continue;
                var direction = delta.normalized;
                var distanceTo = delta.magnitude;
                var power = DistanceFalloff.Get(blopRadius, distanceTo, blopPower, mode);
                
                var repulsionForce = direction * power; 
                node.Value.velocity += repulsionForce;
            }
            
            RestartSimulation();
        }
        
        private void FixedUpdate()
        {
            // HandleClick();
            CheckParameterChanges();

            for (var i = 0; i < processCountPerFrame; i++)
            {
                if (currentState == SimulationState.Simulating)
                {
                    simulationElapsedTime += Time.deltaTime;
                    globalDamping = Mathf.Clamp01(simulationElapsedTime / stabilizationTime);
                    
                    ApplyForces();
                    CheckStabilization();
                }
                
                ApplyExtraRepulsion();
            }
        }

        private void CheckParameterChanges()
        {
            if (
                !Mathf.Approximately(lastRepulsion, repulsion) ||
                !Mathf.Approximately(lastGravity, gravity) ||
                !Mathf.Approximately(lastLink, link) ||
                !Mathf.Approximately(lastExtraRepulsionFactor, extraRepulsionFactor))
            {
                RestartSimulation();
            }
            
            lastRepulsion = repulsion;
            lastExtraRepulsionFactor = extraRepulsionFactor;
            lastLink = link;
            lastGravity = gravity;
        }

        private void ApplyForces()
        {
            var deltaTime = Time.deltaTime * deltaTimeMultiplier;
            var thresholdSqr = (stabilizationThreshold * 0.1f) * (stabilizationThreshold * 0.1f);
            ApplySpringForces();
            foreach (var node in nodes)
            {
                if (node.Value.dragged) continue;
                
                var softForce = Vector2.zero;
                var nodePosition = node.Value.RectTransform.anchoredPosition;
                
                softForce += ComputeRepulsion(node.Value);
                softForce += ComputeGravity(node.Value);
                // softForce += GetSpringForce(node.Value.id, nodePosition, _model.connections, nodes, link);
                
                node.Value.velocity += softForce * deltaTime;
                node.Value.velocity *= (1f - globalDamping);
                
                if (node.Value.velocity.sqrMagnitude < thresholdSqr)
                {
                    node.Value.velocity = Vector2.zero;
                }
                
                node.Value.RectTransform.anchoredPosition += node.Value.velocity * deltaTime;
            }
            
        }

        private void ApplyExtraRepulsion()
        {
            foreach (var node in nodes)
            {
                if (node.Value.dragged) continue;
                var extraRepulsion = ComputeExtraRepulsion(node.Value);
                node.Value.RectTransform.anchoredPosition += extraRepulsion;
            }
        }

        private void CheckStabilization()
        {
            if (nodes.Count == 0) return;
            if (!(globalDamping >= 1f)) return;
            
            foreach (var node in nodes)
            {
                node.Value.velocity = Vector2.zero;
            }
            
            currentState = SimulationState.Stabilized;
        }

        public void RestartSimulation()
        {
            currentState = SimulationState.Simulating;
            globalDamping = 0f;
            simulationElapsedTime = 0f;
        }

        [ContextMenu("Reset Parameters to Default")]
        public void ResetParametersToDefault()
        {
            repulsion = 0.5f;
            extraRepulsionFactor = 50f;
            deltaTimeMultiplier = 1f;
            stabilizationThreshold = 1f;
            stabilizationTime = 5f;
            
            lastRepulsion = repulsion;
            lastExtraRepulsionFactor = extraRepulsionFactor;
            lastLink = link;
            lastGravity = gravity;
            
            RestartSimulation();
        }

        private Vector2 ComputeExtraRepulsion(UINode node)
        {
            var displacement = Vector2.zero;
            var posA = node.RectTransform.anchoredPosition;
            var dominantRange = node.drawer.Radius + extraRepulsionFactor;
            var dominantRangeSqr = dominantRange * dominantRange;

            foreach (var other in nodes)
            {
                if (other.Value == node) continue;

                var posB = other.Value.RectTransform.anchoredPosition;
                var direction = posA - posB;
                var distanceSqr = direction.sqrMagnitude;

                if (!(distanceSqr < dominantRangeSqr)) continue;
                
                var distance = Mathf.Sqrt(distanceSqr);
                if (distance < 0.0001f)
                {
                    direction = Random.insideUnitCircle * 0.01f;
                    distance = direction.magnitude;
                }
                var pushForce = (dominantRange - distance) * 0.1f;
                displacement += direction / distance * pushForce;
            }

            return displacement;
        }

        private Vector2 ComputeGravity(UINode node)
        {
            var direction = Vector2.zero - node.RectTransform.anchoredPosition;
            return direction * gravity;
        }
        
        
        private void ApplySpringForces()
        {
            foreach (var edge in _model.connections)
            {
                var a = nodes[edge.a];
                var b = nodes[edge.b];

                var delta = b.RectTransform.anchoredPosition - a.RectTransform.anchoredPosition;
                var distance = delta.magnitude;
                if (distance < 0.0001f) continue; // защита от деления на ноль

                var displacement = distance - linkDistance;
                var force = delta.normalized * (displacement * link);

                a.velocity += force;
                b.velocity -= force;
            }
        }
        
        private static Vector2 GetSpringForce(int nodeId,
            Vector2 nodePosition,
            List<Connection> allConnections,
            Dictionary<int, UINode> nodesById,
            float power)
        {
            var result = Vector2.zero;

            foreach (var connection in allConnections)
            {
                var otherId = connection.a == nodeId ? connection.b : connection.b == nodeId ? connection.a : -1;
                if (otherId < 0) continue;
                if (nodesById.TryGetValue(otherId, out var other))
                {
                    result += other.RectTransform.anchoredPosition - nodePosition;
                }
            }
            
            return result.sqrMagnitude > 0 ? result.normalized * power : Vector2.zero;
        }
        
        private Vector2 ComputeRepulsion(UINode node)
        {
            var result = Vector2.zero;
            var posA = node.RectTransform.anchoredPosition;

            foreach (var other in nodes)
            {
                if (other.Value == node) continue;
                var posB = other.Value.RectTransform.anchoredPosition;
                var dir = posA - posB;
                var distanceSqr = dir.sqrMagnitude;
                if (distanceSqr < 0.0001f) continue;
                var distance = Mathf.Sqrt(distanceSqr);
                result += dir / distance * repulsion;
            }

            return result;
        }

        public override void OnPageInit(PageArgs args)
        {
        }
        
        public void AddNode(UINode node)
        {
            if (!nodes.TryAdd(node.id, node)) return;
            RestartSimulation();
        }

        public void RemoveNode(UINode node)
        {
            if (!nodes.Remove(node.id)) return;
            _edges.RemoveWhere(e => e.Item1 == node || e.Item2 == node);
            RestartSimulation();
        }

        public void AddEdge(UINode from, UINode to)
        {
            if (_edges.Contains((from, to)) || _edges.Contains((to, from))) return;
            _edges.Add((from, to));
            RestartSimulation();
        }

        public void RemoveEdge(UINode from, UINode to)
        {
            var removed = _edges.Remove((from, to)) || _edges.Remove((to, from));
            if (!removed) return;
            RestartSimulation();
        }

        public void SetNodePosition(UINode node, Vector2 position)
        {
            if (node == null) return;
            node.RectTransform.anchoredPosition = position;
            RestartSimulation();
        }

        [ContextMenu("Create Test Nodes")]
        public void CreateTestNodes()
        {
            if (container == null)
            {
                Debug.LogError("Container is not set!");
                return;
            }
            
            ClearGraph();
            
            var center = container.rect.center;
            
            if (_model == null) DeserializeData();

            if (_model != null)
            {
                foreach (var device in _model.devices)
                {
                    var nodeObj = new GameObject(device.name);

                    nodeObj.transform.SetParent(container, false);

                    var nodeRectTransform = nodeObj.AddComponent<RectTransform>();
                    nodeRectTransform.anchoredPosition = center + Random.insideUnitCircle;
                    nodeRectTransform.sizeDelta = testNodeSize;
                    nodeRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, testNodeRadius);
                    nodeRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, testNodeRadius);

                    var circleDrawer = nodeObj.AddComponent<CircleDrawer>();
                    var node = nodeObj.AddComponent<UINode>();
                    
                    node.drawer = circleDrawer;
                    circleDrawer.Radius = testNodeRadius;
                    node.id = device.id;

                    if (ColorUtility.TryParseHtmlString(device.color, out var color))
                    {
                        circleDrawer.color = color;
                    }

                    node.velocity = Vector2.zero;

                    var textObj = new GameObject("Text");
                    textObj.transform.SetParent(nodeObj.transform);
                    textObj.transform.localScale = new Vector3(1, 1, 1);
                    textObj.transform.position = new Vector3(0, 0, 0);

                    var rectTransform = textObj.AddComponent<RectTransform>();
                    rectTransform.anchoredPosition = Vector2.zero;
                    rectTransform.pivot = new Vector2(0.5f, 1f);
                    rectTransform.anchorMin = new Vector2(0.5f, 0);
                    rectTransform.anchorMax = new Vector2(0.5f, 0);
                    rectTransform.sizeDelta = new Vector2(300, 50);

                    var label = textObj.AddComponent<TextMeshProUGUI>();
                    label.SetText(device.role);
                    label.color = testNodeColor;
                    label.autoSizeTextContainer = true;
                    label.enableAutoSizing = false;
                    label.fontSizeMax = 25;
                    label.alignment = TextAlignmentOptions.Midline;
                    label.horizontalAlignment = HorizontalAlignmentOptions.Center;
                    label.verticalAlignment = VerticalAlignmentOptions.Middle;

                    node.Graph = this;
                    nodes.Add(node.id, node);
                }

                var nodeConnectionsGraphic = gameObject.AddComponent<NodeConnectionsGraphic>();
                nodeConnectionsGraphic.color = new Color(0.35f, 0.35f, 0.35f, 0.7f);
                nodeConnectionsGraphic.Connections = _model.connections;
                nodeConnectionsGraphic.Nodes = nodes;
            }

            RestartSimulation();
        }

        private void ClearGraph()
        {
            foreach (var node in nodes)
            {
                if (node.Value == null || node.Value.gameObject == null) continue;
                if (Application.isPlaying)
                {
                    Destroy(node.Value.gameObject);
                }
                else
                {
                    DestroyImmediate(node.Value.gameObject);
                }
            }
            
            nodes.Clear();
            _edges.Clear();
        }
    }
}
