using UnityEngine;

namespace UI.DevicePage
{
    public class NodeRenderer : MonoBehaviour
    {
        public Material nodeMaterial;
        public float pixelsPerUnit = 100f; // Укажи свой PPU (обычно 100)
        private Mesh _quadMesh;
    
        void Start()
        {
            // 1. Убираем лишние компоненты, если они есть, чтобы не рисовать мусор в 0,0,0
            if (TryGetComponent<MeshRenderer>(out var mr)) mr.enabled = false;

            // 2. Создаем нормализованный меш (размер 1x1, а не 2x2)
            // Это упрощает расчеты: масштаб в матрице будет равен реальному размеру
            _quadMesh = new Mesh();
            _quadMesh.vertices = new Vector3[]
            {
                new(-0.5f, -0.5f, 0),
                new(-0.5f,  0.5f, 0),
                new( 0.5f,  0.5f, 0),
                new( 0.5f, -0.5f, 0)
            };
            _quadMesh.triangles = new int[] {0, 1, 2, 0, 2, 3};
            _quadMesh.uv = new Vector2[] { new(0,0), new(0,1), new(1,1), new(1,0) };
            _quadMesh.RecalculateBounds();
        }

        void Update()
        {
            var nodes = Graph.Instance.Nodes;
            if (nodes == null || nodes.Count == 0) return;

            var matrices = new Matrix4x4[nodes.Count];
            for (var i = 0; i < nodes.Count; i++)
            {
                var n = nodes[i];

                // 3. PIXEL SNAPPING: Округляем позицию до ближайшего физического пикселя
                float x = Mathf.Round(n.Position.x * pixelsPerUnit) / pixelsPerUnit;
                float y = Mathf.Round(n.Position.y * pixelsPerUnit) / pixelsPerUnit;

                // 4. Масштаб тоже должен быть кратен пикселям, чтобы не было "дрожания" краев
                float size = Mathf.Round(n.Radius * 2 * pixelsPerUnit) / pixelsPerUnit;

                matrices[i] = Matrix4x4.TRS(
                    new Vector3(x, y, 0),
                    Quaternion.identity,
                    new Vector3(size, size, 1f)
                );
            }

            // 5. Рисуем. Убедись, что в материале включен "Enable GPU Instancing"!
            Graphics.DrawMeshInstanced(_quadMesh, 0, nodeMaterial, matrices, matrices.Length);
        }
    }
}