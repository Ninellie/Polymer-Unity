using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        public float SpeedMultiplier { get; set; }
        public float CellSize { get; set; }
        public float DominantRange { get; set; }
        public bool IsGeometric { get; set; }
        
        public bool IsSimulated { get; set; }
        public bool IsColliding { get; set; }
        
        private float _damping;
        
        private readonly Dictionary<Vector2Int, List<Node>> _grid = new();
        private readonly Stack<List<Node>> _listPool = new();
        
        public ForceDirectedLayout(List<Node> nodes = null,
            List<(Node a, Node b)> links = null,
            float gravity = 1,
            float linkDistance = 75,
            float linkStrength = 1f,
            float friction = 0.05f,
            float charge = 5,
            float theta = 0.8f,
            float alpha = 1,
            float maxVelocity = 600,
            float speedMultiplier = 1,
            float cellSize = 40,
            float dominantRange = 5,
            bool isGeometric = true)
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
            SpeedMultiplier = speedMultiplier;
            CellSize = cellSize;
            DominantRange = dominantRange;
            IsGeometric = isGeometric;
        }
        
        private void ApplyTangentialRepulsion()
        {
            foreach (var center in Nodes)
            {
                // Все пары соседей этого узла
                var neighbors = center.Links.ToList();
                if (neighbors.Count < 2) continue;

                for (var i = 0; i < neighbors.Count; i++)
                {
                    for (var j = i + 1; j < neighbors.Count; j++)
                    {
                        var a = neighbors[i];
                        var b = neighbors[j];

                        if (a.IsFixed && b.IsFixed) continue;

                        var ca = a.Position - center.Position;
                        var cb = b.Position - center.Position;

                        var distA = ca.magnitude;
                        var distB = cb.magnitude;

                        if (distA < 0.1f || distB < 0.1f) continue;

                        // Найти текущий угол между ними. Скалярное произведение 
                        var dot = Vector2.Dot(ca.normalized, cb.normalized);
                        // Если узлы и так на разных сторонах (180 градусов), сила не нужна
                        if (dot < -0.99f) continue; 

                        // Определить "тангенциальное" направление для узла A
                        // Это вектор, перпендикулярный радиусу CA, направленный от B
                        var tangentA = new Vector2(-ca.y, ca.x).normalized;
                        // Проверка, в ту ли сторону направлен тангент (должен быть от B)
                        if (Vector2.Dot(tangentA, cb) > 0) tangentA = -tangentA;

                        // Сила тем выше, чем меньше угол (чем ближе они друг к другу на дуге)
                        // Можно использовать (1 + dot) как множитель (растет от 0 до 2, когда угол мал)
                        var strength = (1f + dot) * 20f;

                        if (!a.IsFixed) a.Force += tangentA * strength;
                        
                        // Для узла B тангент будет зеркальным
                        var tangentB = new Vector2(-cb.y, cb.x).normalized;
                        if (Vector2.Dot(tangentB, ca) > 0) tangentB = -tangentB;
                        
                        if (!b.IsFixed) b.Force += tangentB * strength;
                    }
                }
            }
        }

        public void Start()
        {
            _damping = Alpha;
            IsSimulated = true;
            IsColliding = true;
        }

        public void Tick(float dt)
        {
            if (!IsColliding) return;
            
            UpdateCellSize();

            ApplyCollisionRepulsion();

            if (!IsSimulated) return;

            ClearForces();

            ApplyForces();

            TranslateNodes(dt * SpeedMultiplier);

            _damping -= Friction * dt * SpeedMultiplier;
            
            if (_damping > 0) return;

            IsSimulated = false;
        }

        private void ApplyForces()
        {
            if (LinkStrength > 0) ApplySpringForces();
            if (Gravity > 0) ApplyGravity();
            if (Charge > 0) ApplyRepulsion();
            ApplyTangentialRepulsion();
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
                
                if (IsGeometric)
                    node.Velocity = node.Force * dt;
                else
                    node.Velocity += node.Force * dt;
                
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

            var moveCounter = 0;

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

                if (displacement.sqrMagnitude > 0.01f)
                {
                    node.Position += displacement;
                    Interlocked.Exchange(ref moveCounter, 1);
                }
            });

            if (moveCounter > 0) return;
            if (IsSimulated) return;
            
            IsColliding = false;
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