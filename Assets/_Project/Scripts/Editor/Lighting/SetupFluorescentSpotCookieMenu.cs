#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace Backrooms.EditorTools
{
    /// <summary>
    /// Чинит лампы: mesh отдельно от Spot (без сплюснутого scale), cookie, цвет.
    /// Меню: Backrooms → FIX Lights
    /// </summary>
    public static class SetupFluorescentSpotCookieMenu
    {
        private const string CookiePath = "Assets/_Project/Art/Textures/cookie_fluorescent_rect.png";
        private const string MatPath = "Assets/_Project/Art/Materials/Mat_Fluorescent.mat";

        [MenuItem("Backrooms/FIX Lights")]
        public static void Setup()
        {
            EnsureCookieImport();
            var cookie = AssetDatabase.LoadAssetAtPath<Texture>(CookiePath);
            if (cookie == null)
            {
                EditorUtility.DisplayDialog("FIX Lights", "Нет cookie_fluorescent_rect.png", "OK");
                return;
            }

            FixPrefab("Assets/_Project/Prefabs/FluorescentPanel.prefab", cookie, 0.85f, 9f);
            FixPrefab("Assets/_Project/Prefabs/FluorescentPanel_Hell.prefab", cookie, 0.7f, 8f);
            FixMaterial();
            DisableFillLightsInOpenScene();

            AssetDatabase.SaveAssets();
            if (!Application.isPlaying)
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

            EditorUtility.DisplayDialog(
                "FIX Lights",
                "Готово.\n1) Ctrl+S\n2) Play\nКруг на потолке должен пропасть / стать прямоугольнее.",
                "OK");
        }

        private static void FixPrefab(string path, Texture cookie, float intensity, float range)
        {
            var root = PrefabUtility.LoadPrefabContents(path);
            try
            {
                // Корневой scale = 1 (Spot не должен наследовать 0.05 по Y)
                root.transform.localScale = Vector3.one;

                // Mesh на отдельном ребёнке Panel
                var meshFilter = root.GetComponent<MeshFilter>();
                if (meshFilter != null)
                {
                    var panel = new GameObject("Panel");
                    panel.transform.SetParent(root.transform, false);
                    panel.transform.localScale = new Vector3(1.2f, 0.05f, 0.6f);

                    var mf = panel.AddComponent<MeshFilter>();
                    mf.sharedMesh = meshFilter.sharedMesh;

                    var oldMr = root.GetComponent<MeshRenderer>();
                    var mr = panel.AddComponent<MeshRenderer>();
                    if (oldMr != null)
                    {
                        mr.sharedMaterials = oldMr.sharedMaterials;
                        mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                        mr.receiveShadows = false;
                        Object.DestroyImmediate(oldMr);
                    }

                    var oldCol = root.GetComponent<BoxCollider>();
                    if (oldCol != null)
                    {
                        var col = panel.AddComponent<BoxCollider>();
                        col.size = oldCol.size;
                        Object.DestroyImmediate(oldCol);
                    }

                    Object.DestroyImmediate(meshFilter);
                }
                else
                {
                    var panelT = root.transform.Find("Panel");
                    if (panelT != null)
                        panelT.localScale = new Vector3(1.2f, 0.05f, 0.6f);
                }

                foreach (var light in root.GetComponentsInChildren<Light>(true))
                {
                    light.transform.SetParent(root.transform, true);
                    light.transform.localPosition = new Vector3(0f, -0.08f, 0f);
                    light.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                    light.transform.localScale = Vector3.one;
                    light.type = LightType.Spot;
                    light.spotAngle = 95f;
                    light.innerSpotAngle = 55f;
                    light.range = range;
                    light.intensity = intensity;
                    light.color = new Color(0.92f, 0.95f, 0.88f);
                    light.cookie = cookie;
                    light.shadows = LightShadows.None;
                    light.gameObject.name = "Spot Light";

                    var add = light.GetComponent<UniversalAdditionalLightData>();
                    if (add == null)
                        add = light.gameObject.AddComponent<UniversalAdditionalLightData>();
                    add.lightCookieSize = new Vector2(1.2f, 0.6f);

                    var flick = light.GetComponent<Backrooms.Gameplay.FlickeringLight>();
                    if (flick != null)
                    {
                        var so = new SerializedObject(flick);
                        so.FindProperty("_baseIntensity").floatValue = intensity;
                        so.FindProperty("_minIntensity").floatValue = intensity * 0.5f;
                        so.FindProperty("_flickerSpeed").floatValue = 0.55f;
                        so.FindProperty("_rareDipChance").floatValue = 0.004f;
                        so.FindProperty("_rareDipDuration").floatValue = 0.18f;
                        so.FindProperty("_rareDipIntensity").floatValue = intensity * 0.15f;
                        so.ApplyModifiedPropertiesWithoutUndo();
                    }
                }

                PrefabUtility.SaveAsPrefabAsset(root, path);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        private static void FixMaterial()
        {
            var fluo = AssetDatabase.LoadAssetAtPath<Material>(MatPath);
            if (fluo == null)
                return;
            fluo.EnableKeyword("_EMISSION");
            fluo.SetColor("_EmissionColor", new Color(0.42f, 0.4f, 0.28f));
            fluo.SetColor("_BaseColor", new Color(0.55f, 0.56f, 0.48f));
            EditorUtility.SetDirty(fluo);
        }

        private static void DisableFillLightsInOpenScene()
        {
            foreach (var t in Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (t.name == "Light_JunctionFill" || t.name == "Light_SpurJunctionFill")
                    t.gameObject.SetActive(false);
            }
        }

        private static void EnsureCookieImport()
        {
            if (!System.IO.File.Exists(CookiePath))
                return;

            AssetDatabase.ImportAsset(CookiePath, ImportAssetOptions.ForceUpdate);
            var importer = AssetImporter.GetAtPath(CookiePath) as TextureImporter;
            if (importer == null)
                return;

            importer.textureType = TextureImporterType.Cookie;
            importer.alphaSource = TextureImporterAlphaSource.FromGrayScale;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.filterMode = FilterMode.Bilinear;
            importer.anisoLevel = 0;
            importer.mipmapEnabled = false;
            importer.sRGBTexture = false;

            var so = new SerializedObject(importer);
            var cookieLight = so.FindProperty("m_CookieLightType") ?? so.FindProperty("cookieLightType");
            if (cookieLight != null)
            {
                cookieLight.intValue = 1; // Spot
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            importer.SaveAndReimport();
        }
    }
}
#endif
