using System.Collections.Generic;
using Polymer.UI.Routing;
using TMPro;
using TriInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace UI.DevicePage
{
    [RequireComponent(typeof(RectTransform))]
    public class DeviceRolesGraph : PageBase
    {
        [Header("Json")]
        [SerializeField] private TextAsset jsonFile;
        private NetworkModel _model;
        
        private readonly Dictionary<int, UINode> nodes = new();

        [Header("Graph Container")]
        [SerializeField] private RectTransform container;
        
        [Header("Test Node Creation")]
        [Range(1f, 100f)]  [SerializeField] private float testNodeRadius = 5f;
        
        [SerializeField] private Color testNodeColor = Color.white;
        [SerializeField] private Vector2 testNodeSize = new(100f, 100f);
        [SerializeField] private bool createTestNodesOnAwake = true;
        
        [Header("Physics")]
        [Range(0, 10f)] [SerializeField] private float repulsion = 0.5f;
        [Range(0, 10f)] [SerializeField] private float link = 0.5f;
        [Range(0, 1f)] [SerializeField] private float gravity = 0.5f;
        [Range(50, 500f)] [SerializeField] private float linkDistance = 50f;
        [SerializeField] private float extraRepulsionFactor = 70f;
        
        [Range(0.01f, 10f)] [SerializeField] private float deltaTimeMultiplier = 1f;
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

        private const float MaxVelocity = 200f;

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

        private void FixedUpdate()
        {
            // HandleClick();
            // CheckParameterChanges();
            ApplyExtraRepulsion();
            //
            // if (currentState == SimulationState.Stabilized) return;
            //
            // simulationElapsedTime += Time.deltaTime;
            // globalDamping = Mathf.Clamp01(simulationElapsedTime / stabilizationTime);

            foreach (var node in nodes)
            {
                node.Value.force = Vector2.zero;
            }
            
            ApplySpringForces();
            ApplyGravity();
            ComputeRepulsion();

            
            
            foreach (var node in nodes)
            {
                node.Value.velocity += node.Value.force * Time.deltaTime;
                node.Value.velocity *= 1 - globalDamping;
            }
            
            // ClampVelocity();
            MoveNodes();
                
            // CheckStabilization();
        }

        public void RestartSimulation()
        {
            currentState = SimulationState.Simulating;
            globalDamping = 0f;
            simulationElapsedTime = 0f;
        }

        private void ComputeRepulsion()
        {
            foreach (var node in nodes)
            {
                var result = Vector2.zero;
                var posA = node.Value.RectTransform.anchoredPosition;

                foreach (var other in nodes)
                {
                    if (other.Key == node.Key) continue;
                    var posB = other.Value.RectTransform.anchoredPosition;
                    var dir = posA - posB;
                    var distanceSqr = dir.sqrMagnitude;
                    if (distanceSqr < 0.0001f) continue;
                    var distance = Mathf.Sqrt(distanceSqr);
                    result += dir / distance * repulsion;
                }

                node.Value.force += result;
            }
        }

        private void ApplyGravity()
        {
            foreach (var node in nodes)
            {
                var direction = Vector2.zero - node.Value.RectTransform.anchoredPosition;
                node.Value.force += direction * gravity;
            }
        }

        private void MoveNodes()
        {
            foreach (var node in nodes)
            {
                if (node.Value.dragged) continue;
                node.Value.RectTransform.anchoredPosition += node.Value.velocity * Time.deltaTime;
            }
        }

        private void ClampVelocity()
        {
            foreach (var node in nodes)
            {
                if (node.Value.dragged) continue;
                node.Value.velocity = Vector2.ClampMagnitude(node.Value.velocity, MaxVelocity);
            }
        }

        private void ApplyExtraRepulsion()
        {
            foreach (var node in nodes)
            {
                if (node.Value.dragged) continue;
                var displacement = Vector2.zero;
                var posA = node.Value.RectTransform.anchoredPosition;
                var dominantRange = node.Value.drawer.Radius + extraRepulsionFactor;
                var dominantRangeSqr = dominantRange * dominantRange;

                foreach (var other in nodes)
                {
                    if (other.Value.id == node.Value.id) continue;

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

                node.Value.RectTransform.anchoredPosition += displacement;
            }
        }

        private void CheckStabilization()
        {
            if (!(globalDamping >= 1f)) return;
            
            foreach (var node in nodes)
            {
                node.Value.velocity = Vector2.zero;
            }
            
            currentState = SimulationState.Stabilized;
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

                a.force += force;
                b.force -= force;
            }
        }

        public override void OnPageInit(PageArgs args)
        {
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
                    
                    foreach (var connection in _model.connections)
                    {
                        if (connection.a == device.id)
                        {
                            circleDrawer.Radius *= 1.01f;
                        }
                        if (connection.b == device.id)
                        {
                            circleDrawer.Radius *= 1.01f;
                        }
                    }
                    
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

        private void DeserializeData()
        {
            if (jsonFile == null) return;
            _model = JsonUtility.FromJson<NetworkModel>(jsonFile.text);
            Debug.Log($"JSON: devices count: {_model.devices.Count}. Connections count: {_model.connections.Count}");
        }
    }
}
