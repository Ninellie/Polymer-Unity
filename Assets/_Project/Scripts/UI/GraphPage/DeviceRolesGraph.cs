using System;
using System.Collections.Generic;
using Polymer.UI.Routing;
using TMPro;
using TriInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace UI.DevicePage
{
    [Serializable]
    public class Device
    {
        public int id;
        public string name;
        public string role;
        public string color;
    }

    [Serializable]
    public class Connection
    {
        public int a;
        public int b;
    }

    [Serializable]
    public class NetworkModel
    {
        public List<Device> devices;
        public List<Connection> connections;
    }
    
    public class NetworkLoader : MonoBehaviour
    {
        public TextAsset jsonFile;
        public NetworkModel Model { get; private set; }

    }
    
    [RequireComponent(typeof(RectTransform))]
    public class DeviceRolesGraph : PageBase
    {
        [Header("Json")]
        [SerializeField] private TextAsset jsonFile;
        private NetworkModel _model;
        
        [Header("Graph")]
        [SerializeField] private List<UINode> nodes = new();
        private readonly HashSet<(UINode, UINode)> _edges = new();

        [Header("Graph Container")]
        [SerializeField] private RectTransform container;
        
        [Header("Test Node Creation")]
        [Range(1, 500)] [SerializeField] private int testNodeCount = 100;
        [Range(1f, 100f)] [SerializeField] private float testNodeRadius = 5f;
        [Range(3, 128)] [SerializeField] private int testNodeSegments = 64;
        [Range(0.1f, 10f)] [SerializeField] private float testNodeLineWidth = 2f;
        [SerializeField] private Color testNodeColor = Color.white;
        [SerializeField] private Vector2 testNodeSize = new(100f, 100f);
        [SerializeField] private bool createTestNodesOnAwake = true;
        [Range(-1f,1f)] [SerializeField] private float attractionForce = 0.5f;
        [SerializeField] private float extraRepulsionFactor = 50f;
        
        [Header("Physics")]
        
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
        [SerializeField] [ReadOnly] private float lastAttractionForce;
        [SerializeField] [ReadOnly] private float lastExtraRepulsionFactor;
        [SerializeField] [ReadOnly] private float globalDamping;
        [SerializeField] [ReadOnly] private float simulationElapsedTime;

        private InputAction _midClick;
        private InputAction _point;

        private Vector2 _previousBlopPosition = Vector2.zero;
        
        private void Awake()
        {
            if (jsonFile != null)
            {
                _model = JsonUtility.FromJson<NetworkModel>(jsonFile.text);
                
                Debug.Log($"JSON: devices count: {_model.devices.Count}. Connections count: {_model.connections.Count}");
            }
            
            lastAttractionForce = attractionForce;
            lastExtraRepulsionFactor = extraRepulsionFactor;
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
                if (node.dragged)
                {
                    continue;
                }
                var nodePos = node.rectTransform.anchoredPosition;
                var delta = nodePos - _previousBlopPosition;
                if (!(delta.magnitude < blopRadius)) continue;
                var direction = delta.normalized;
                var distanceTo = delta.magnitude;
                var power = DistanceFalloff.Get(blopRadius, distanceTo, blopPower, mode);
                
                var repulsionForce = direction * power; 
                node.Velocity += repulsionForce;
            }
            
            RestartSimulation();
        }
        
        private void Update()
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
            if (!Mathf.Approximately(lastAttractionForce, attractionForce) ||
                !Mathf.Approximately(lastExtraRepulsionFactor, extraRepulsionFactor))
            {
                RestartSimulation();
            }
            
            lastAttractionForce = attractionForce;
            lastExtraRepulsionFactor = extraRepulsionFactor;
        }

        private void ApplyForces()
        {
            var deltaTime = Time.deltaTime * deltaTimeMultiplier;
            var thresholdSqr = (stabilizationThreshold * 0.1f) * (stabilizationThreshold * 0.1f);
            
            foreach (var node in nodes)
            {
                if (node.dragged)
                {
                    continue;
                }
                var softForce = Vector2.zero;
                softForce += ComputeAttraction(node);
                
                node.Velocity += softForce * deltaTime;
                node.Velocity *= (1f - globalDamping);
                
                if (node.Velocity.sqrMagnitude < thresholdSqr)
                {
                    node.Velocity = Vector2.zero;
                }
                
                node.rectTransform.anchoredPosition += node.Velocity * deltaTime;
            }
        }

        private void ApplyExtraRepulsion()
        {
            foreach (var node in nodes)
            {
                if (node.dragged)
                {
                    continue;
                }
                var extraRepulsion = ComputeExtraRepulsion(node);
                node.rectTransform.anchoredPosition += extraRepulsion;
            }
        }

        private void CheckStabilization()
        {
            if (nodes.Count == 0) return;

            if (!(globalDamping >= 1f)) return;
            foreach (var node in nodes)
            {
                node.Velocity = Vector2.zero;
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
            attractionForce = 0.5f;
            extraRepulsionFactor = 50f;
            deltaTimeMultiplier = 1f;
            stabilizationThreshold = 1f;
            stabilizationTime = 5f;
            
            lastAttractionForce = attractionForce;
            lastExtraRepulsionFactor = extraRepulsionFactor;
            
            RestartSimulation();
        }

        private Vector2 ComputeExtraRepulsion(UINode node)
        {
            var displacement = Vector2.zero;
            var posA = node.rectTransform.anchoredPosition;
            var dominantRange = node.radius + extraRepulsionFactor;
            var dominantRangeSqr = dominantRange * dominantRange;

            foreach (var other in nodes)
            {
                if (other == node) continue;

                var posB = other.rectTransform.anchoredPosition;
                var dir = posA - posB;
                var distanceSqr = dir.sqrMagnitude;

                if (!(distanceSqr < dominantRangeSqr)) continue;
                
                var distance = Mathf.Sqrt(distanceSqr);
                if (distance < 0.0001f)
                {
                    dir = Random.insideUnitCircle * 0.01f;
                    distance = dir.magnitude;
                }
                var pushForce = (dominantRange - distance) * 0.1f;
                displacement += dir / distance * pushForce;
            }

            return displacement;
        }

        private Vector2 ComputeAttraction(UINode node)
        {
            var result = Vector2.zero;
            var posA = node.rectTransform.anchoredPosition;

            foreach (var other in nodes)
            {
                if (other == node) continue;
                var posB = other.rectTransform.anchoredPosition;
                var dir = posB - posA;
                var distanceSqr = dir.sqrMagnitude;
                if (distanceSqr < 0.0001f) continue;
                var distance = Mathf.Sqrt(distanceSqr);
                result += dir / distance * attractionForce;
            }

            return result;
        }

        public override void OnPageInit(PageArgs args)
        {
        }
        
        public void AddNode(UINode node)
        {
            if (nodes.Contains(node)) return;
            nodes.Add(node);
            RestartSimulation();
        }

        public void RemoveNode(UINode node)
        {
            if (!nodes.Remove(node)) return;
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
            node.rectTransform.anchoredPosition = position;
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

            for (var i = 0; i < testNodeCount; i++)
            {
                var nodeObj = new GameObject("Node_" + i);
                
                nodeObj.transform.SetParent(container, false);

                var rt = nodeObj.AddComponent<RectTransform>();
                rt.anchoredPosition = center + Random.insideUnitCircle;
                rt.sizeDelta = testNodeSize;
                rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, testNodeRadius);
                rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, testNodeRadius);

                var node = nodeObj.AddComponent<UINode>();
                node.radius = testNodeRadius;
                node.segments = testNodeSegments;
                node.lineWidth = testNodeLineWidth;
                node.color = testNodeColor;
                node.Velocity = Vector2.zero;

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
                label.SetText("Node " + i);
                label.color = testNodeColor;
                label.autoSizeTextContainer = false;
                label.enableAutoSizing = false;
                label.fontSizeMax = 25;
                label.alignment = TextAlignmentOptions.Midline;
                label.horizontalAlignment = HorizontalAlignmentOptions.Center;
                label.verticalAlignment = VerticalAlignmentOptions.Middle;
                
                nodes.Add(node);
            }
            
            RestartSimulation();
        }

        private void ClearGraph()
        {
            foreach (var node in nodes)
            {
                if (node == null || node.gameObject == null) continue;
                if (Application.isPlaying)
                {
                    Destroy(node.gameObject);
                }
                else
                {
                    DestroyImmediate(node.gameObject);
                }
            }
            
            nodes.Clear();
            _edges.Clear();
        }

    }
}
