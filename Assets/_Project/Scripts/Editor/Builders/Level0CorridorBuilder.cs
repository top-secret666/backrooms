#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Backrooms.Gameplay;

namespace Backrooms.EditorTools
{
    /// <summary>
    /// Меню: Backrooms → Build Kane-Style T-Corridor.
    /// Собирает T-образный коридор Level 0 (ширина ~4м, высота ~3.2м) + сетка люминесцентных ламп.
    /// Старую геометрию с тегом "Greybox" не трогает — создаёт корневой объект Level0_Corridor.
    /// </summary>
    public static class Level0CorridorBuilder
    {
        private const string RootName = "Level0_Corridor";
        private const float CorridorWidth = 4f;
        private const float WallHeight = 3.2f;
        private const float WallThickness = 0.45f;
        private const float MainLength = 24f;
        private const float SpurLength = 12f;
        private const float CeilingY = 3.2f;
        private const float LightY = 3.05f;

        // User sections — NEVER destroy these (manual geometry).
        private static readonly string[] ProtectedRoots =
        {
            "HELL",
            "Hell",
            "Level0_Corridor_Hell",
            "Level0_Corridor (3)",
        };

        [MenuItem("Backrooms/Build Kane-Style T-Corridor")]
        public static void Build()
        {
            // Safety: if a corridor already exists, do NOT wipe the scene.
            // Manual pieces (HELL / duplicates) are never destroyed.
            var existing = GameObject.Find(RootName);
            if (existing != null)
            {
                bool rebuild = EditorUtility.DisplayDialog(
                    "Backrooms — corridor already exists",
                    "Level0_Corridor already in the scene.\n\n" +
                    "Rebuild will DELETE only 'Level0_Corridor' and recreate it.\n" +
                    "Objects named HELL / custom duplicates are kept.\n\n" +
                    "Save the scene (Ctrl+S) before rebuilding.",
                    "Rebuild Level0_Corridor only",
                    "Cancel");
                if (!rebuild)
                    return;

                Object.DestroyImmediate(existing);
            }

            foreach (string protectedName in ProtectedRoots)
            {
                // Just log — never destroy
                if (GameObject.Find(protectedName) != null)
                    Debug.Log($"[Backrooms] Protected user section kept: {protectedName}");
            }

            HideLegacyGreybox();

            var root = new GameObject(RootName);

            Material wallMat = LoadMat("Assets/_Project/Art/Materials/Mat_Wallpaper_Office.mat");
            Material floorMat = LoadMat("Assets/_Project/Art/Materials/Mat_Carpet_Office.mat");
            Material ceilingMat = LoadMat("Assets/_Project/Art/Materials/Mat_Ceiling_Office.mat");

            // --- Main hall along X (center at origin, runs east-west) ---
            float halfW = CorridorWidth * 0.5f;
            float halfMain = MainLength * 0.5f;
            float wallY = WallHeight * 0.5f;

            // Long north/south walls of main hall (with gap for north spur at x≈0)
            float spurHalf = CorridorWidth * 0.5f;
            float sideLen = (MainLength - CorridorWidth) * 0.5f;

            // South wall (solid)
            MakeWall(root.transform, "Wall_Main_S",
                new Vector3(0f, wallY, -halfW),
                new Vector3(MainLength, WallHeight, WallThickness),
                wallMat);

            // North wall — two segments with opening for spur
            MakeWall(root.transform, "Wall_Main_N_West",
                new Vector3(-(spurHalf + sideLen * 0.5f), wallY, halfW),
                new Vector3(sideLen, WallHeight, WallThickness),
                wallMat);
            MakeWall(root.transform, "Wall_Main_N_East",
                new Vector3(spurHalf + sideLen * 0.5f, wallY, halfW),
                new Vector3(sideLen, WallHeight, WallThickness),
                wallMat);

            // End caps of main hall
            MakeWall(root.transform, "Wall_Main_W",
                new Vector3(-halfMain, wallY, 0f),
                new Vector3(WallThickness, WallHeight, CorridorWidth),
                wallMat);
            MakeWall(root.transform, "Wall_Main_E",
                new Vector3(halfMain, wallY, 0f),
                new Vector3(WallThickness, WallHeight, CorridorWidth),
                wallMat);

            // --- North spur along +Z ---
            float spurCenterZ = halfW + SpurLength * 0.5f;
            MakeWall(root.transform, "Wall_Spur_W",
                new Vector3(-halfW, wallY, spurCenterZ),
                new Vector3(WallThickness, WallHeight, SpurLength),
                wallMat);
            MakeWall(root.transform, "Wall_Spur_E",
                new Vector3(halfW, wallY, spurCenterZ),
                new Vector3(WallThickness, WallHeight, SpurLength),
                wallMat);
            MakeWall(root.transform, "Wall_Spur_N",
                new Vector3(0f, wallY, halfW + SpurLength),
                new Vector3(CorridorWidth, WallHeight, WallThickness),
                wallMat);

            // Floors (Plane is 10x10 by default → scale = size/10)
            MakeFloor(root.transform, "Floor_Main",
                new Vector3(0f, 0f, 0f),
                new Vector3(MainLength / 10f, 1f, CorridorWidth / 10f),
                floorMat);
            MakeFloor(root.transform, "Floor_Spur",
                new Vector3(0f, 0f, spurCenterZ),
                new Vector3(CorridorWidth / 10f, 1f, SpurLength / 10f),
                floorMat);

            // Ceilings — flipped Plane
            MakeCeiling(root.transform, "Ceiling_Main",
                new Vector3(0f, CeilingY, 0f),
                new Vector3(MainLength / 10f, 1f, CorridorWidth / 10f),
                ceilingMat);
            MakeCeiling(root.transform, "Ceiling_Spur",
                new Vector3(0f, CeilingY, spurCenterZ),
                new Vector3(CorridorWidth / 10f, 1f, SpurLength / 10f),
                ceilingMat);

            // Fluorescent light grid
            var lights = new GameObject("Lights");
            lights.transform.SetParent(root.transform, false);
            PlaceLightsAlongX(lights.transform, MainLength, z: 0f);
            PlaceLightsAlongZ(lights.transform, SpurLength, startZ: halfW, x: 0f);

            // Kane = only fluorescents — turn off scene Directional Lights
            foreach (var light in Object.FindObjectsByType<Light>(FindObjectsSortMode.None))
            {
                if (light.type == LightType.Directional)
                    light.enabled = false;
            }

            // Place player at corridor start if present
            var player = GameObject.Find("Player");
            if (player != null)
            {
                Undo.RecordObject(player.transform, "Move Player to corridor");
                player.transform.position = new Vector3(0f, 1f, 0f);
                player.transform.rotation = Quaternion.identity;
            }

            Selection.activeGameObject = root;
            Undo.RegisterCreatedObjectUndo(root, "Build Kane-Style T-Corridor");
            Debug.Log("[Backrooms] Kane T-corridor ready. Play and walk the hall + north spur. Esc unlocks cursor; click Game to relock.");
        }

        [MenuItem("Backrooms/Hide Legacy Greybox Room")]
        public static void HideLegacyGreybox()
        {
            string[] names =
            {
                "Plane", "Ceiling", "Cube", "Cube (1)", "Cube (2)", "Cube (3)", "Point Light"
            };

            int hidden = 0;
            foreach (string name in names)
            {
                var go = GameObject.Find(name);
                if (go == null)
                    continue;
                // Don't hide lights that live under the corridor
                if (go.transform.root != null && go.transform.root.name == RootName)
                    continue;
                if (go.name == "Point Light" && go.transform.parent != null)
                    continue;

                Undo.RecordObject(go, "Hide legacy greybox");
                go.SetActive(false);
                hidden++;
            }

            Debug.Log($"[Backrooms] Legacy greybox objects hidden: {hidden}");
        }

        private static void PlaceLightsAlongX(Transform parent, float length, float z)
        {
            const float spacing = 4f;
            float half = length * 0.5f;
            for (float x = -half + spacing * 0.5f; x <= half - spacing * 0.25f; x += spacing)
                MakeFluorescent(parent, new Vector3(x, LightY, z));
        }

        private static void PlaceLightsAlongZ(Transform parent, float length, float startZ, float x)
        {
            const float spacing = 4f;
            for (float z = startZ + spacing * 0.5f; z <= startZ + length - spacing * 0.25f; z += spacing)
                MakeFluorescent(parent, new Vector3(x, LightY, z));
        }

        private static void MakeFluorescent(Transform parent, Vector3 pos)
        {
            // Visual panel (thin cube)
            var panel = GameObject.CreatePrimitive(PrimitiveType.Cube);
            panel.name = "FluorescentPanel";
            panel.transform.SetParent(parent, false);
            panel.transform.position = pos;
            panel.transform.localScale = new Vector3(1.2f, 0.05f, 0.6f);

            var renderer = panel.GetComponent<MeshRenderer>();
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader != null)
            {
                var mat = new Material(shader)
                {
                    name = "Mat_Fluorescent_Runtime",
                    color = new Color(1f, 0.95f, 0.75f)
                };
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", new Color(1.2f, 1.1f, 0.7f));
                mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                renderer.sharedMaterial = mat;
            }

            // Point light child
            var lightGo = new GameObject("Point Light");
            lightGo.transform.SetParent(panel.transform, false);
            lightGo.transform.localPosition = new Vector3(0f, -0.15f, 0f);

            var light = lightGo.AddComponent<Light>();
            light.type = LightType.Point;
            light.range = 7.5f;
            light.intensity = 1.9f;
            light.color = new Color(1f, 0.93f, 0.68f); // warm fluorescent (Kane)
            light.shadows = LightShadows.Soft;

            var flicker = lightGo.AddComponent<FlickeringLight>();
            // Editor: push Kane-tuned values into private SerializeFields
            var so = new SerializedObject(flicker);
            so.FindProperty("_baseIntensity").floatValue = 1.9f;
            so.FindProperty("_minIntensity").floatValue = 1.45f;
            so.FindProperty("_flickerSpeed").floatValue = 0.9f;
            so.FindProperty("_rareDipChance").floatValue = 0.004f;
            so.FindProperty("_rareDipDuration").floatValue = 0.1f;
            so.FindProperty("_rareDipIntensity").floatValue = 0.55f;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void MakeWall(Transform parent, string name, Vector3 pos, Vector3 size, Material mat)
        {
            // Корень: scale 1 + BoxCollider.size в мировых метрах
            // (CharacterController плохо дружит с сильно растянутыми кубами).
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.position = pos;
            go.transform.localScale = Vector3.one;
            var box = go.AddComponent<BoxCollider>();
            box.size = size;

            var visual = new GameObject("Visual");
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
            go.transform.SetParent(parent, false);
            go.transform.position = pos;
            go.transform.localScale = scale;
            ApplyMat(go, mat);
        }

        private static void MakeCeiling(Transform parent, string name, Vector3 pos, Vector3 scale, Material mat)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Plane);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.position = pos;
            go.transform.rotation = Quaternion.Euler(180f, 0f, 0f);
            go.transform.localScale = scale;
            ApplyMat(go, mat);
        }

        private static void ApplyMat(GameObject go, Material mat)
        {
            if (mat == null) return;
            var r = go.GetComponent<MeshRenderer>();
            if (r != null) r.sharedMaterial = mat;
        }

        private static Material LoadMat(string path)
        {
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null)
                Debug.LogWarning($"[Backrooms] Material not found: {path}");
            return mat;
        }
    }
}
#endif
