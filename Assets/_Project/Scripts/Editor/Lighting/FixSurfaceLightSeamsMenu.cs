#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Backrooms.EditorTools
{
    /// <summary>
    /// Как Floor_Unified: один меш на потолки и один на стены —
    /// URP не режет свет по швам отдельных кусков.
    /// Меню: Backrooms → Fix Ceiling + Wall Light Seams
    /// </summary>
    public static class FixSurfaceLightSeamsMenu
    {
        [MenuItem("Backrooms/Fix Ceiling + Wall Light Seams")]
        public static void Fix()
        {
            var corridor = GameObject.Find("Level0_Corridor");
            if (corridor == null)
            {
                EditorUtility.DisplayDialog("Surface seams", "Level0_Corridor не найден.", "OK");
                return;
            }

            int ceilings = MergeCeilings(corridor.transform);
            int walls = MergeWalls(corridor.transform);

            RenderSettings.ambientIntensity = Mathf.Max(RenderSettings.ambientIntensity, 1.15f);

            EditorUtility.DisplayDialog(
                "Surface seams",
                $"Потолки → Ceiling_Unified (было кусков: {ceilings})\n" +
                $"Стены → Wall_Unified (визуалов: {walls})\n" +
                "Коллайдеры Wall_* оставлены.\nCtrl+S → Play.",
                "OK");
        }

        private static int MergeCeilings(Transform corridor)
        {
            string[] names = { "Ceiling_Main", "Ceiling_Spur", "Ceiling_Hell" };
            var quads = new List<(Vector3 center, Vector2 size, float y)>();

            foreach (string n in names)
            {
                Transform t = corridor.Find(n);
                if (t == null)
                    continue;

                float w = Mathf.Abs(t.lossyScale.x) * 10f;
                float d = Mathf.Abs(t.lossyScale.z) * 10f;
                Vector3 c = t.position;
                quads.Add((new Vector3(c.x, 0f, c.z), new Vector2(w, d), c.y));

                Undo.RecordObject(t.gameObject, "Hide old ceiling");
                t.gameObject.SetActive(false);
            }

            if (quads.Count == 0)
                return 0;

            ReplaceChild(corridor, "Ceiling_Unified");

            var go = new GameObject("Ceiling_Unified");
            Undo.RegisterCreatedObjectUndo(go, "Ceiling_Unified");
            go.transform.SetParent(corridor, false);
            go.transform.position = Vector3.zero;

            var filter = go.AddComponent<MeshFilter>();
            var renderer = go.AddComponent<MeshRenderer>();
            Mesh mesh = BuildCeilingMesh(quads, metersPerTile: 4f);
            filter.sharedMesh = mesh;

            var mat = AssetDatabase.LoadAssetAtPath<Material>(
                "Assets/_Project/Art/Materials/Mat_Ceiling_Office.mat");
            if (mat != null)
                renderer.sharedMaterial = mat;

            return quads.Count;
        }

        private static int MergeWalls(Transform corridor)
        {
            var combines = new List<CombineInstance>();
            int count = 0;

            foreach (Transform child in corridor.GetComponentsInChildren<Transform>(true))
            {
                if (!child.name.StartsWith("Wall_"))
                    continue;
                if (child.parent != corridor)
                    continue;

                Transform visual = child.Find("Visual");
                if (visual == null)
                    continue;

                var mf = visual.GetComponent<MeshFilter>();
                var mr = visual.GetComponent<MeshRenderer>();
                if (mf == null || mf.sharedMesh == null || mr == null)
                    continue;

                // На всякий случай: UV-меш в метрах (если ещё старый stretched cube)
                Vector3 size = child.GetComponent<BoxCollider>() != null
                    ? child.GetComponent<BoxCollider>().size
                    : visual.localScale;
                if (visual.localScale != Vector3.one || mf.sharedMesh.name.StartsWith("Cube"))
                    WallMeshUtility.ApplyUvMeshToVisual(visual, size, 2.5f);

                combines.Add(new CombineInstance
                {
                    mesh = mf.sharedMesh,
                    transform = corridor.worldToLocalMatrix * visual.localToWorldMatrix
                });

                Undo.RecordObject(mr, "Hide wall visual");
                mr.enabled = false;
                count++;
            }

            if (count == 0)
                return 0;

            ReplaceChild(corridor, "Wall_Unified");

            var go = new GameObject("Wall_Unified");
            Undo.RegisterCreatedObjectUndo(go, "Wall_Unified");
            go.transform.SetParent(corridor, false);
            go.transform.position = Vector3.zero;

            var filter = go.AddComponent<MeshFilter>();
            var renderer = go.AddComponent<MeshRenderer>();
            var mesh = new Mesh { name = "Wall_Unified" };
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.CombineMeshes(combines.ToArray(), true, true);
            filter.sharedMesh = mesh;

            var mat = AssetDatabase.LoadAssetAtPath<Material>(
                "Assets/_Project/Art/Materials/Mat_Wallpaper_Office.mat");
            if (mat != null)
                renderer.sharedMaterial = mat;

            return count;
        }

        private static void ReplaceChild(Transform parent, string name)
        {
            Transform old = parent.Find(name);
            if (old != null)
                Undo.DestroyObjectImmediate(old.gameObject);
        }

        private static Mesh BuildCeilingMesh(
            List<(Vector3 center, Vector2 size, float y)> quads,
            float metersPerTile)
        {
            var verts = new List<Vector3>();
            var norms = new List<Vector3>();
            var uvs = new List<Vector2>();
            var tris = new List<int>();
            float uScale = 1f / Mathf.Max(0.01f, metersPerTile);

            foreach (var (center, size, y) in quads)
            {
                float hx = size.x * 0.5f;
                float hz = size.y * 0.5f;
                int i = verts.Count;
                float yy = y - 0.01f; // чуть ниже, без z-fight с лампами

                // Нормаль вниз (смотрим снизу вверх на потолок)
                Vector3 a = new Vector3(center.x - hx, yy, center.z - hz);
                Vector3 b = new Vector3(center.x + hx, yy, center.z - hz);
                Vector3 c = new Vector3(center.x + hx, yy, center.z + hz);
                Vector3 d = new Vector3(center.x - hx, yy, center.z + hz);

                verts.Add(a); verts.Add(b); verts.Add(c); verts.Add(d);
                for (int n = 0; n < 4; n++)
                    norms.Add(Vector3.down);

                uvs.Add(new Vector2(a.x * uScale, a.z * uScale));
                uvs.Add(new Vector2(b.x * uScale, b.z * uScale));
                uvs.Add(new Vector2(c.x * uScale, c.z * uScale));
                uvs.Add(new Vector2(d.x * uScale, d.z * uScale));

                // Wind order for downward normal (viewed from below)
                tris.Add(i); tris.Add(i + 1); tris.Add(i + 2);
                tris.Add(i); tris.Add(i + 2); tris.Add(i + 3);
            }

            var mesh = new Mesh { name = "Ceiling_Unified" };
            mesh.SetVertices(verts);
            mesh.SetNormals(norms);
            mesh.SetUVs(0, uvs);
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateBounds();
            return mesh;
        }
    }
}
#endif
