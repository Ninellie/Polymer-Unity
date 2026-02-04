using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace FDLayout
{
    public class ForceDirectedLayout
    {
        public List<Node> Nodes { get; set; }
        public List<(Node a, Node b)> Links { get; set; }

        public float Gravity { get; set; }
        public float LinkDistance { get; set; }
        public float LinkStrength { get; set; }
        public float Friction { get; set; } // alpha decrease per second
        public float Charge { get; set; }
        public float Theta { get; set; } // Berns-Hutt distance
        public float Alpha { get; set; } // Base dumping

        public float MaxVelocity { get; set; }

        public float CellSize { get; set; }
        public float DominantRange { get; set; }

        private float _damping;
        private bool _isSimulated;
        private bool _isColliding;

        private readonly Dictionary<Vector2Int, List<Node>> _grid = new();
        private readonly Stack<List<Node>> _listPool = new();

        // private readonly Dictionary<string, float> _settings = new();
        
        public ForceDirectedLayout(
            List<Node> nodes = null,
            List<(Node a, Node b)> links = null,
            float gravity = 1,
            float linkDistance = 75,
            float linkStrength = 1f,
            float friction = 0.05f,
            float charge = 5,
            float theta = 0.8f,
            float alpha = 1,
            float maxVelocity = 600,
            float cellSize = 40,
            float dominantRange = 5)
        {
            Nodes = nodes;
            Links = links;
            Gravity = gravity;
            LinkDistance = linkDistance;
            LinkStrength = linkStrength;
            Friction = friction;
            Charge = charge;
            Theta = theta;
            Alpha = alpha;
            MaxVelocity = maxVelocity;
            CellSize = cellSize;
            DominantRange = dominantRange;
        }

        public void Start()
        {
            _damping = Alpha;
            _isSimulated = true;
            _isColliding = true;
        }

        public void Tick(float dt)
        {
            if (!_isColliding) return;
            
            UpdateCellSize();

            ApplyCollisionRepulsion();

            if (!_isSimulated) return;

            ClearForces();

            ApplyForces();

            TranslateNodes(dt);

            _damping -= Friction * dt;
            
            if (_damping > 0) return;

            _isSimulated = false;
        }

        private void ApplyForces()
        {
            if (LinkStrength > 0) ApplySpringForces();
            if (Gravity > 0) ApplyGravity();
            if (Charge > 0) ApplyRepulsion();
        }

        private void ClearForces()
        {
            foreach (var node in Nodes)
            {
                node.Force = Vector2.zero;
            }
        }

        private void ApplyRepulsion()
        {
            var bounds = GetBounds();
            
            var root = new QuadTree(bounds);
            foreach (var node in Nodes)
                root.Insert(node);
            
            Parallel.ForEach(Nodes, node =>
            {
                if (node.IsFixed) return;
                var repulsionForce = root.ComputeRepulsion(node, Theta, Charge);
                
                node.Force += repulsionForce;
            });
        }

        private Rect GetBounds()
        {
            var minX = float.MaxValue;
            var minY = float.MaxValue;
            var maxX = float.MinValue;
            var maxY = float.MinValue;

            foreach (var node in Nodes)
            {
                minX = Mathf.Min(minX, node.Position.x);
                minY = Mathf.Min(minY, node.Position.y);
                maxX = Mathf.Max(maxX, node.Position.x);
                maxY = Mathf.Max(maxY, node.Position.y);
            }

            var size = Mathf.Max(maxX - minX, maxY - minY);
            return new Rect(minX, minY, size, size);
        }

        private void ApplySpringForces()
        {
            foreach (var connection in Links)
            {
                var delta = connection.b.Position - connection.a.Position;
                var distance = delta.magnitude;
                if (distance < 0.0001f) continue;
                
                var targetDistance = LinkDistance + connection.a.Radius + connection.b.Radius;
                var displacement = distance - targetDistance;
        
                var transition = (delta / distance) * (displacement * LinkStrength);
                var totalWeight = connection.a.Weight + connection.b.Weight;

                if (!connection.a.IsFixed)
                    connection.a.Force += transition * (connection.b.Weight / totalWeight);

                if (!connection.b.IsFixed)
                    connection.b.Force -= transition * (connection.a.Weight / totalWeight);
            }
        }

        private void ApplyGravity()
        {
            foreach (var node in Nodes)
            {
                var delta = Vector2.zero - node.Position;
                node.Force += delta * Gravity;
            }
        }

        private void TranslateNodes(float dt)
        {
            foreach (var node in Nodes)
            {
                if (node.IsFixed) continue;
                node.Velocity = node.Force * dt;
                node.Velocity *= _damping;
                node.Velocity = Vector2.ClampMagnitude(node.Velocity, MaxVelocity);
                node.Position += node.Velocity;
            }
        }

        private void ApplyCollisionRepulsion()
        {
            foreach (var list in _grid.Values)
            {
                list.Clear();
                _listPool.Push(list);
            }
            _grid.Clear();

            foreach (var node in Nodes)
            {
                var c = Cell(node.Position);
                if (!_grid.TryGetValue(c, out var list))
                {
                    list = _listPool.Count > 0 ? _listPool.Pop() : new List<Node>();
                    _grid[c] = list;
                }
                list.Add(node);
            }

            var hasMovement = false;

            Parallel.ForEach(Nodes, node =>
            {
                if (node.IsFixed) return;

                var displacement = Vector2.zero;
                var cell = Cell(node.Position);

                for (var dx = -1; dx <= 1; dx++)
                {
                    for (var dy = -1; dy <= 1; dy++)
                    {
                        var neighborCell = cell + new Vector2Int(dx, dy);
                        if (!_grid.TryGetValue(neighborCell, out var others))
                            continue;

                        foreach (var other in others)
                        {
                            if (other == node) continue;

                            var dir = node.Position - other.Position;
                            var distSqr = dir.sqrMagnitude;

                            var currentRange = node.Radius + other.Radius + DominantRange;
                            var rangeSqr = currentRange * currentRange;

                            if (distSqr > rangeSqr || distSqr < 0.0001f) continue;

                            var dist = Mathf.Sqrt(distSqr);
                            displacement += dir / dist * ((currentRange - dist) * 0.1f);
                        }
                    }
                }

                if (!(displacement.sqrMagnitude > 0.0001f)) return;
                node.Position += displacement;
                hasMovement = true;
            });

            if (hasMovement) return;
            if (_isSimulated) return;
            
            _isColliding = false;
        }
        
        private void UpdateCellSize()
        {
            float maxRadius = 0;
            foreach (var node in Nodes)
            {
                if (node.Radius > maxRadius) maxRadius = node.Radius;
            }
            CellSize = Mathf.Max(CellSize, maxRadius * 2f);
        }
        
        private Vector2Int Cell(Vector2 pos)
        {
            return new Vector2Int(
                Mathf.FloorToInt(pos.x / CellSize),
                Mathf.FloorToInt(pos.y / CellSize)
            );
        }
    }
}