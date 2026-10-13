#if UNITY_EDITOR
using UnityEngine;

namespace Backrooms.EditorTools
{
    /// <summary>
    /// Куб с UV в метрах мира: на грани 24×3.2 м текстура не растягивается
    /// (в отличие от Scale на дефолтном Cube, где UV всегда 0..1).
    /// </summary>
    public static class WallMeshUtility
    {
        /// <param name="size">Размер бокса в метрах (как BoxCollider.size).</param>
        /// <param name="metersPerTile">Сколько метров на один повтор текстуры.</param>
        public static Mesh CreateBoxMesh(Vector3 size, float metersPerTile = 2.5f)
        {
            float hx = size.x * 0.5f;
            float hy = size.y * 0.5f;
            float hz = size.z * 0.5f;
            float u = 1f / Mathf.Max(0.01f, metersPerTile);

            var mesh = new Mesh { name = $"WallBox_{size.x:0.##}x{size.y:0.##}x{size.z:0.##}" };

            // 24 вершины (4 на грань), нормали + UV по размеру грани в метрах
            var verts = new Vector3[24];
            var norms = new Vector3[24];
            var uvs = new Vector2[24];
            var tris = new int[36];

            int v = 0;
            int t = 0;

            void Face(Vector3 n, Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector2 uvA, Vector2 uvB, Vector2 uvC, Vector2 uvD)
            {
                int i = v;
                verts[v] = a; norms[v] = n; uvs[v] = uvA; v++;
                verts[v] = b; norms[v] = n; uvs[v] = uvB; v++;
                verts[v] = c; norms[v] = n; uvs[v] = uvC; v++;
                verts[v] = d; norms[v] = n; uvs[v] = uvD; v++;
                tris[t++] = i; tris[t++] = i + 1; tris[t++] = i + 2;
                tris[t++] = i; tris[t++] = i + 2; tris[t++] = i + 3;
            }

            // +Z / -Z (ширина X, высота Y)
            Face(Vector3.forward,
                new Vector3(-hx, -hy, hz), new Vector3(hx, -hy, hz), new Vector3(hx, hy, hz), new Vector3(-hx, hy, hz),
                new Vector2(0, 0), new Vector2(size.x * u, 0), new Vector2(size.x * u, size.y * u), new Vector2(0, size.y * u));
            Face(Vector3.back,
                new Vector3(hx, -hy, -hz), new Vector3(-hx, -hy, -hz), new Vector3(-hx, hy, -hz), new Vector3(hx, hy, -hz),
                new Vector2(0, 0), new Vector2(size.x * u, 0), new Vector2(size.x * u, size.y * u), new Vector2(0, size.y * u));

            // +X / -X (ширина Z, высота Y)
            Face(Vector3.right,
                new Vector3(hx, -hy, hz), new Vector3(hx, -hy, -hz), new Vector3(hx, hy, -hz), new Vector3(hx, hy, hz),
                new Vector2(0, 0), new Vector2(size.z * u, 0), new Vector2(size.z * u, size.y * u), new Vector2(0, size.y * u));
            Face(Vector3.left,
                new Vector3(-hx, -hy, -hz), new Vector3(-hx, -hy, hz), new Vector3(-hx, hy, hz), new Vector3(-hx, hy, -hz),
                new Vector2(0, 0), new Vector2(size.z * u, 0), new Vector2(size.z * u, size.y * u), new Vector2(0, size.y * u));

            // +Y / -Y (ширина X, глубина Z)
            Face(Vector3.up,
                new Vector3(-hx, hy, hz), new Vector3(hx, hy, hz), new Vector3(hx, hy, -hz), new Vector3(-hx, hy, -hz),
                new Vector2(0, 0), new Vector2(size.x * u, 0), new Vector2(size.x * u, size.z * u), new Vector2(0, size.z * u));
            Face(Vector3.down,
                new Vector3(-hx, -hy, -hz), new Vector3(hx, -hy, -hz), new Vector3(hx, -hy, hz), new Vector3(-hx, -hy, hz),
                new Vector2(0, 0), new Vector2(size.x * u, 0), new Vector2(size.x * u, size.z * u), new Vector2(0, size.z * u));

            mesh.vertices = verts;
            mesh.normals = norms;
            mesh.uv = uvs;
            mesh.triangles = tris;
            mesh.RecalculateBounds();
            return mesh;
        }

        public static void ApplyUvMeshToVisual(Transform visual, Vector3 sizeMeters, float metersPerTile = 2.5f)
        {
            var filter = visual.GetComponent<MeshFilter>();
            if (filter == null)
                filter = visual.gameObject.AddComponent<MeshFilter>();

            filter.sharedMesh = CreateBoxMesh(sizeMeters, metersPerTile);
            visual.localScale = Vector3.one;
            visual.localPosition = Vector3.zero;
            visual.localRotation = Quaternion.identity;
        }
    }
}
#endif
