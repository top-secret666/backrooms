#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Backrooms.Gameplay;

namespace Backrooms.EditorTools
{
    /// <summary>
    /// Adds Wall_Hell_* wing INSIDE existing Level0_Corridor. Never deletes user Hell walls.
    /// Menu: Backrooms → Add / Refresh Hell Wing (Wall_Hell_*)
    /// </summary>
    public static class Level0HellWingBuilder
    {
        private const string CorridorName = "Level0_Corridor";
        private const float Width = 4f;
        private const float Height = 3.2f;
        private const float Thickness = 0.45f;
        private const float Length = 12f; // west wing length
        // Main corridor west cap is at x=-12; Hell continues to x=-24
        private const float HellCenterX = -18f;
        private const float HellWestX = -24f;
        private const float HalfW = Width * 0.5f;

        [MenuItem("Backrooms/Add or Refresh Hell Wing (Wall_Hell_*)")]
        public static void BuildHellWing()
        {
            var corridor = GameObject.Find(CorridorName);
            if (corridor == null)
            {
                EditorUtility.DisplayDialog(
                    "Backrooms",
                    "Нет Level0_Corridor. Сначала: Backrooms → Build Kane-Style T-Corridor (Cancel если уже есть кастом).",
                    "OK");
                return;
            }

            // Remove ONLY previous auto Hell wing pieces (exact names), never random user stuff
            string[] autoNames =
            {
                "Wall_Hell_N", "Wall_Hell_S", "Wall_Hell_E", "Wall_Hell_W",
                "Floor_Hell", "Ceiling_Hell", "Hell_Lights"
            };
            foreach (string n in autoNames)
            {
                var t = corridor.transform.Find(n);
                if (t != null)
                    Undo.DestroyObjectImmediate(t.gameObject);
            }

            Material wallMat = LoadMat("Assets/_Project/Art/Materials/Mat_Wallpaper_Office.mat");
            Material floorMat = LoadMat("Assets/_Project/Art/Materials/Mat_Carpet_Office.mat");
            Material ceilingMat = LoadMat("Assets/_Project/Art/Materials/Mat_Ceiling_Office.mat");

            float wallY = Height * 0.5f;

            // Open west end of main hall so you can walk into Hell
            var mainWest = corridor.transform.Find("Wall_Main_W");
            if (mainWest != null)
            {
                Undo.RecordObject(mainWest.gameObject, "Open path to Hell");
                mainWest.gameObject.SetActive(false);
            }

            MakeWall(corridor.transform, "Wall_Hell_S",
                new Vector3(HellCenterX, wallY, -HalfW),
                new Vector3(Length, Height, Thickness), wallMat);
            MakeWall(corridor.transform, "Wall_Hell_N",
                new Vector3(HellCenterX, wallY, HalfW),
                new Vector3(Length, Height, Thickness), wallMat);
            MakeWall(corridor.transform, "Wall_Hell_W",
                new Vector3(HellWestX, wallY, 0f),
                new Vector3(Thickness, Height, Width), wallMat);
            // East = opening into main (no solid full wall). Two stubs optional — skip Wall_Hell_E solid.
            // User asked for Wall_Hell_E: thin marker pillars at the junction (not blocking).
            MakeWall(corridor.transform, "Wall_Hell_E",
                new Vector3(-12f, wallY, 0f),
                new Vector3(Thickness, Height, Thickness), wallMat);

            MakeFloor(corridor.transform, "Floor_Hell",
                new Vector3(HellCenterX, 0f, 0f),
                new Vector3(Length / 10f, 1f, Width / 10f), floorMat);
            MakeCeiling(corridor.transform, "Ceiling_Hell",
                new Vector3(HellCenterX, Height, 0f),
                new Vector3(Length / 10f, 1f, Width / 10f), ceilingMat);

            var lights = new GameObject("Hell_Lights");
            Undo.RegisterCreatedObjectUndo(lights, "Hell_Lights");
            lights.transform.SetParent(corridor.transform, false);
            for (float x = HellWestX + 2f; x < -12f; x += 4f)
                MakeFluorescent(lights.transform, new Vector3(x, 3.05f, 0f));

            // Hide mistaken separate root if present
            foreach (string bad in new[] { "HELL", "Level0_Corridor (3)", "Level0_Corridor_Hell" })
            {
                var go = GameObject.Find(bad);
                if (go != null)
                {
                    Undo.RecordObject(go, "Hide duplicate root");
                    go.SetActive(false);
                }
            }

            Selection.activeGameObject = corridor.transform.Find("Wall_Hell_N")?.gameObject;
            Debug.Log("[Backrooms] Hell wing ready under Level0_Corridor: Wall_Hell_N/S/E/W, Floor_Hell, Ceiling_Hell. Ctrl+S!");
            EditorUtility.DisplayDialog(
                "Hell wing",
                "Создано внутри Level0_Corridor:\nWall_Hell_N/S/E/W, Floor_Hell, Ceiling_Hell, Hell_Lights.\n\nСРАЗУ нажми Ctrl+S.",
                "OK");
        }

        private static void MakeWall(Transform parent, string name, Vector3 pos, Vector3 size, Material mat)
        {
            var go = new GameObject(name);
            Undo.RegisterCreatedObjectUndo(go, name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            go.transform.localScale = Vector3.one;
            var box = go.AddComponent<BoxCollider>();
            box.size = size;

            var visual = new GameObject("Visual");
            visual.name = "Visual";
            Undo.RegisterCreatedObjectUndo(visual, name + " Visual");
            visual.transform.SetParent(go.transform, false);
            visual.AddComponent<MeshFilter>();
            visual.AddComponent<MeshRenderer>();
            WallMeshUtility.ApplyUvMeshToVisual(visual.transform, size, metersPerTile: 2.5f);
            ApplyMat(visual, mat);
        }

        private static void MakeFloor(Transform parent, string name, Vector3 pos, Vector3 scale, Material mat)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Plane);
            go.name = name;
            Undo.RegisterCreatedObjectUndo(go, name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            go.transform.localScale = scale;
            ApplyMat(go, mat);
        }

        private static void MakeCeiling(Transform parent, string name, Vector3 pos, Vector3 scale, Material mat)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Plane);
            go.name = name;
            Undo.RegisterCreatedObjectUndo(go, name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            go.transform.localRotation = Quaternion.Euler(180f, 0f, 0f);
            go.transform.localScale = scale;
            ApplyMat(go, mat);
        }

        private static void MakeFluorescent(Transform parent, Vector3 pos)
        {
            var panel = GameObject.CreatePrimitive(PrimitiveType.Cube);
            panel.name = "FluorescentPanel_Hell";
            Undo.RegisterCreatedObjectUndo(panel, panel.name);
            panel.transform.SetParent(parent, false);
            panel.transform.localPosition = pos;
            panel.transform.localScale = new Vector3(1.2f, 0.05f, 0.6f);

            var renderer = panel.GetComponent<MeshRenderer>();
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader != null)
            {
                var mat = new Material(shader) { color = new Color(1f, 0.95f, 0.75f) };
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", new Color(1.2f, 1.1f, 0.7f));
                renderer.sharedMaterial = mat;
            }

            var lightGo = new GameObject("Point Light");
            lightGo.transform.SetParent(panel.transform, false);
            lightGo.transform.localPosition = new Vector3(0f, -0.15f, 0f);
            var light = lightGo.AddComponent<Light>();
            light.type = LightType.Point;
            light.range = 7.5f;
            light.intensity = 1.9f;
            light.color = new Color(1f, 0.93f, 0.68f);
            light.shadows = LightShadows.Soft;
            lightGo.AddComponent<FlickeringLight>();
        }

        private static void ApplyMat(GameObject go, Material mat)
        {
            if (mat == null) return;
            var r = go.GetComponent<MeshRenderer>();
            if (r != null) r.sharedMaterial = mat;
        }

        private static Material LoadMat(string path) =>
            AssetDatabase.LoadAssetAtPath<Material>(path);
    }
}
#endif
