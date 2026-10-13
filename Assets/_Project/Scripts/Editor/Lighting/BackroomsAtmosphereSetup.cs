#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace Backrooms.EditorTools
{
    /// <summary>
    /// Меню: Backrooms → Setup Atmosphere (Fog + Volume Bloom/Vignette).
    /// </summary>
    public static class BackroomsAtmosphereSetup
    {
        private const string ProfilePath = "Assets/_Project/Art/PP_Backrooms_Level0.asset";

        [MenuItem("Backrooms/Setup Atmosphere (Fog + Bloom)")]
        public static void Setup()
        {
            // --- Fog / ambient ---
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogColor = new Color(0.32f, 0.33f, 0.24f);
            RenderSettings.fogDensity = 0.075f;
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.3f, 0.31f, 0.26f);
            RenderSettings.ambientIntensity = 1.15f;

            // --- Volume profile ---
            var profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(ProfilePath);
            if (profile == null)
            {
                profile = ScriptableObject.CreateInstance<VolumeProfile>();
                AssetDatabase.CreateAsset(profile, ProfilePath);
            }

            if (!profile.TryGet(out Bloom bloom))
            {
                bloom = profile.Add<Bloom>(true);
            }
            bloom.active = true;
            bloom.threshold.Override(0.85f);
            bloom.intensity.Override(0.35f);
            bloom.scatter.Override(0.6f);
            bloom.tint.Override(new Color(1f, 0.95f, 0.7f));

            if (!profile.TryGet(out Vignette vignette))
            {
                vignette = profile.Add<Vignette>(true);
            }
            vignette.active = true;
            vignette.intensity.Override(0.28f);
            vignette.smoothness.Override(0.4f);
            vignette.color.Override(new Color(0.15f, 0.12f, 0.05f));

            if (!profile.TryGet(out ColorAdjustments color))
            {
                color = profile.Add<ColorAdjustments>(true);
            }
            color.active = true;
            color.contrast.Override(8f);
            color.saturation.Override(-8f);
            color.colorFilter.Override(new Color(1f, 0.96f, 0.82f));

            EditorUtility.SetDirty(profile);

            // --- Global Volume in scene ---
            var existing = GameObject.Find("PP_Backrooms");
            if (existing == null)
            {
                existing = new GameObject("PP_Backrooms");
                Undo.RegisterCreatedObjectUndo(existing, "Create PP_Backrooms");
            }

            var volume = existing.GetComponent<Volume>();
            if (volume == null)
                volume = existing.AddComponent<Volume>();
            volume.isGlobal = true;
            volume.priority = 1f;
            volume.profile = profile;

            // Camera post-processing on
            var cam = Camera.main;
            if (cam != null)
            {
                var data = cam.GetComponent<UniversalAdditionalCameraData>();
                if (data != null)
                {
                    data.renderPostProcessing = true;
                    EditorUtility.SetDirty(data);
                }
                cam.nearClipPlane = 0.05f;
                cam.fieldOfView = 70f;
                EditorUtility.SetDirty(cam);
            }

            // Soften fluorescent emission a bit if mat exists
            var fluo = AssetDatabase.LoadAssetAtPath<Material>(
                "Assets/_Project/Art/Materials/Mat_Fluorescent.mat");
            if (fluo != null)
            {
                fluo.EnableKeyword("_EMISSION");
                // Тусклые «еле живые» панели
                fluo.SetColor("_EmissionColor", new Color(0.42f, 0.4f, 0.28f));
                fluo.SetColor("_BaseColor", new Color(0.55f, 0.56f, 0.48f));
                EditorUtility.SetDirty(fluo);
            }

            var wall = AssetDatabase.LoadAssetAtPath<Material>(
                "Assets/_Project/Art/Materials/Mat_Wallpaper_Office.mat");
            if (wall != null)
            {
                wall.SetTextureScale("_BaseMap", new Vector2(1.25f, 1.1f));
                wall.SetTextureScale("_MainTex", new Vector2(1.25f, 1.1f));
                wall.SetFloat("_BumpScale", 0.7f);
                wall.SetFloat("_Smoothness", 0.12f);
                EditorUtility.SetDirty(wall);
            }

            AssetDatabase.SaveAssets();
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

            EditorUtility.DisplayDialog(
                "Atmosphere",
                "Готово:\n• Fog / ambient\n• PP_Backrooms (Bloom + Vignette)\n• Post Processing на Main Camera\n\nСразу Ctrl+S и Play.",
                "OK");
        }
    }
}
#endif
