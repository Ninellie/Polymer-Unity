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
        public float Theta { get; set; }
        public float Alpha { get; set; }

        public float MaxVelocity { get; set; }

        public bool IsSimulated { get; set; }
        public float CellSize { get; set; }
        public float DominantRange { get; set; }

        private float _damping;

        private readonly Dictionary<Vector2Int, List<Node>> _grid = new();
        private readonly Stack<List<Node>> _listPool = new();

        public ForceDirectedLayout(
            List<Node> nodes = null,
            List<(Node a, Node b)> links = null,
            float gravity = 1,
            float linkDistance = 75,
            float linkStrength = 0.4f,
            float friction = 1,
            float charge = 2,
            float theta = 0.8f,
            float alpha = 1,
            float maxVelocity = 600,
            float cellSize = 40,
            float dominantRange = 40)
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
            IsSimulated = true;
        }

        public void Tick(float dt)
        {
            ApplyCollisionRepulsion();

            if (!IsSimulated) return;

            ClearForces();

            ApplyForces();

            TranslateNodes(dt);

            _damping -= Friction * dt;

            if (_damping > 0) return;

            IsSimulated = false;
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

                var displacement = distance - LinkDistance;
                
                var transition = (delta / distance) * (displacement * LinkStrength);
                var totalWeight = connection.a.Weight + connection.b.Weight;

                var aTransition = transition * (connection.b.Weight / totalWeight);
                var bTransition = transition * (connection.a.Weight / totalWeight);

                if (!connection.a.IsFixed)
                {
                    connection.a.Force += aTransition;
                }

                if (!connection.b.IsFixed)
                {
                    connection.b.Force -= bTransition;
                }
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

            Parallel.ForEach(Nodes, node =>
            {
                if (node.IsFixed) return;

                var displacement = Vector2.zero;
                var cell = Cell(node.Position);
                var rangeSqr = DominantRange * DominantRange;

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

                            if (distSqr > rangeSqr || distSqr < 0.0001f) continue;

                            var dist = Mathf.Sqrt(distSqr);
                            
                            displacement += dir / dist * ((DominantRange - dist) * 0.1f);
                        }
                    }
                }

                node.Position += displacement;
            });
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