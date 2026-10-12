#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
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
        private const float WallThickness = 0.2f;
        private const float MainLength = 24f;
        private const float SpurLength = 12f;
        private const float CeilingY = 3.2f;
        private const float LightY = 3.05f;

        [MenuItem("Backrooms/Build Kane-Style T-Corridor")]
        public static void Build()
        {
            var old = GameObject.Find(RootName);
            if (old != null)
                Object.DestroyImmediate(old);

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

            Selection.activeGameObject = root;
            Undo.RegisterCreatedObjectUndo(root, "Build Kane-Style T-Corridor");
            Debug.Log("[Backrooms] T-corridor built. Hide/disable old Plane/Cubes/Ceiling if they overlap. Move Player to (0, 1, 0).");
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
            light.range = 8f;
            light.intensity = 2.0f;
            light.color = new Color(1f, 0.92f, 0.65f); // warm fluorescent
            light.shadows = LightShadows.Soft;

            var flicker = lightGo.AddComponent<FlickeringLight>();
            // Defaults from script are fine; seeds differ per instance via Awake
        }

        private static void MakeWall(Transform parent, string name, Vector3 pos, Vector3 scale, Material mat)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.position = pos;
            go.transform.localScale = scale;
            ApplyMat(go, mat);
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
