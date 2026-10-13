#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Backrooms.EditorTools
{
    /// <summary>
    /// Меню: Backrooms → Fix Floor Light Seams (merge floors).
    /// Стык света на полу = разные MeshRenderer (Main/Spur/Hell).
    /// Один объединённый пол → одна выборка ламп в URP → полоса пропадает.
    /// </summary>
    public static class FixFloorLightSeamsMenu
    {
        private const string UnifiedName = "Floor_Unified";

        [MenuItem("Backrooms/Fix Floor Light Seams (merge floors)")]
        public static void Fix()
        {
            var corridor = GameObject.Find("Level0_Corridor");
            if (corridor == null)
            {
                EditorUtility.DisplayDialog("Floor seams", "Level0_Corridor не найден.", "OK");
                return;
            }

            string[] names = { "Floor_Main", "Floor_Spur", "Floor_Hell" };
            var quads = new List<(Vector3 center, Vector2 size)>();

            foreach (string n in names)
            {
                Transform t = corridor.transform.Find(n);
                if (t == null)
                    continue;

                // Plane default 10x10, scale.x/z * 10 = размер в метрах
                float w = Mathf.Abs(t.lossyScale.x) * 10f;
                float d = Mathf.Abs(t.lossyScale.z) * 10f;
                Vector3 c = t.position;
                c.y = 0f;
                quads.Add((c, new Vector2(w, d)));

                Undo.RecordObject(t.gameObject, "Hide old floor");
                t.gameObject.SetActive(false);
            }

            if (quads.Count == 0)
            {
                EditorUtility.DisplayDialog("Floor seams", "Полы Floor_* не найдены.", "OK");
                return;
            }

            Transform oldUnified = corridor.transform.Find(UnifiedName);
            if (oldUnified != null)
                Undo.DestroyObjectImmediate(oldUnified.gameObject);

            var go = new GameObject(UnifiedName);
            Undo.RegisterCreatedObjectUndo(go, "Floor_Unified");
            go.transform.SetParent(corridor.transform, false);
            go.transform.position = Vector3.zero;

            var filter = go.AddComponent<MeshFilter>();
            var renderer = go.AddComponent<MeshRenderer>();
            var col = go.AddComponent<MeshCollider>();

            Mesh mesh = BuildCombinedFloorMesh(quads, metersPerTile: 4f);
            filter.sharedMesh = mesh;
            col.sharedMesh = mesh;

            var floorMat = AssetDatabase.LoadAssetAtPath<Material>(
                "Assets/_Project/Art/Materials/Mat_Carpet_Office.mat");
            if (floorMat != null)
                renderer.sharedMaterial = floorMat;

            // Доп. заливка на T-стыке Main/Spur (там тоже часто полоса)
            EnsureFill(corridor.transform, "Light_SpurJunctionFill", new Vector3(0f, 2.6f, 2f), 1.2f, 14f);
            EnsureFill(corridor.transform, "Light_JunctionFill", new Vector3(-12f, 2.6f, 0f), 1.5f, 18f);

            RenderSettings.ambientIntensity = 1.15f;

            EditorUtility.SetDirty(go);
            EditorUtility.DisplayDialog(
                "Floor seams",
                "Полы Main/Spur/Hell склеены в Floor_Unified.\nСтарые полы выключены.\nCtrl+S → Play — полоса на полу должна уйти.",
                "OK");
        }

        private static void EnsureFill(Transform parent, string name, Vector3 localPos, float intensity, float range)
        {
            Transform t = parent.Find(name);
            GameObject go;
            if (t != null)
            {
                go = t.gameObject;
            }
            else
            {
                go = new GameObject(name);
                Undo.RegisterCreatedObjectUndo(go, name);
                go.transform.SetParent(parent, false);
            }

            go.transform.localPosition = localPos;
            var light = go.GetComponent<Light>();
            if (light == null)
                light = Undo.AddComponent<Light>(go);

            light.type = LightType.Point;
            light.color = new Color(1f, 0.93f, 0.72f);
            light.intensity = intensity;
            light.range = range;
            light.shadows = LightShadows.None;
        }

        private static Mesh BuildCombinedFloorMesh(List<(Vector3 center, Vector2 size)> quads, float metersPerTile)
        {
            var verts = new List<Vector3>();
            var norms = new List<Vector3>();
            var uvs = new List<Vector2>();
            var tris = new List<int>();
            float uScale = 1f / Mathf.Max(0.01f, metersPerTile);

            foreach (var (center, size) in quads)
            {
                float hx = size.x * 0.5f;
                float hz = size.y * 0.5f;
                int i = verts.Count;

                Vector3 a = new Vector3(center.x - hx, 0.01f, center.z - hz);
                Vector3 b = new Vector3(center.x + hx, 0.01f, center.z - hz);
                Vector3 c = new Vector3(center.x + hx, 0.01f, center.z + hz);
                Vector3 d = new Vector3(center.x - hx, 0.01f, center.z + hz);

                verts.Add(a); verts.Add(b); verts.Add(c); verts.Add(d);
                for (int n = 0; n < 4; n++)
                    norms.Add(Vector3.up);

                uvs.Add(new Vector2(a.x * uScale, a.z * uScale));
                uvs.Add(new Vector2(b.x * uScale, b.z * uScale));
                uvs.Add(new Vector2(c.x * uScale, c.z * uScale));
                uvs.Add(new Vector2(d.x * uScale, d.z * uScale));

                tris.Add(i); tris.Add(i + 2); tris.Add(i + 1);
                tris.Add(i); tris.Add(i + 3); tris.Add(i + 2);
            }

            var mesh = new Mesh { name = "Floor_Unified" };
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
