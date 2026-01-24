using UnityEngine;

namespace FDLayout
{
    public class QuadTree
    {
        private Rect _bounds;
        private Node _body;
        private QuadTree[] _children;

        private Vector2 _centerOfMass;
        private int _mass;

        public QuadTree(Rect bounds)
        {
            _bounds = bounds;
        }

        public Vector2 ComputeRepulsion(Node node, float theta, float charge)
        {
            if (_mass == 0 || (_body == node && _children == null))
                return Vector2.zero;

            var delta = node.Position - _centerOfMass;
            var distance = delta.magnitude + 0.001f;
            var size = _bounds.width;

            // Barnes–Hut условие
            if (_children == null || size / distance < theta)
            {
                return delta / distance * (charge * _mass);
            }

            var force = Vector2.zero;
            foreach (var child in _children)
            {
                force += child.ComputeRepulsion(node, theta, charge);
            }

            return force;
        }
    
        public void Insert(Node node)
        {
            if (_mass == 0 && _body == null)
            {
                _body = node;
                _centerOfMass = node.Position;
                _mass = 1;
                return;
            }

            if (_children == null)
                Subdivide();

            if (_body != null)
            {
                InsertIntoChild(_body);
                _body = null;
            }

            InsertIntoChild(node);

            _mass++;
            _centerOfMass = (_centerOfMass * (_mass - 1) + node.Position) / _mass;
        }

        private void InsertIntoChild(Node node)
        {
            foreach (var child in _children)
            {
                if (!child._bounds.Contains(node.Position)) continue;
                child.Insert(node);
                return;
            }
        }

        private void Subdivide()
        {
            _children = new QuadTree[4];

            var hw = _bounds.width / 2f;
            var hh = _bounds.height / 2f;

            _children[0] = new QuadTree(new Rect(_bounds.xMin, _bounds.yMin, hw, hh)); // SW
            _children[1] = new QuadTree(new Rect(_bounds.xMin + hw, _bounds.yMin, hw, hh)); // SE
            _children[2] = new QuadTree(new Rect(_bounds.xMin, _bounds.yMin + hh, hw, hh)); // NW
            _children[3] = new QuadTree(new Rect(_bounds.xMin + hw, _bounds.yMin + hh, hw, hh)); // NE
        }
    }
}