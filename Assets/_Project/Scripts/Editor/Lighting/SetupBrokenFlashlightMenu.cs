#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Backrooms.Gameplay;

namespace Backrooms.EditorTools
{
    /// <summary>
    /// Меню: Backrooms → Setup Broken Flashlight (F)
    /// </summary>
    public static class SetupBrokenFlashlightMenu
    {
        private const string ConfigPath = "Assets/_Project/Configs/FlashlightConfig.asset";

        [MenuItem("Backrooms/Setup Broken Flashlight (F)")]
        public static void Setup()
        {
            var player = GameObject.Find("Player");
            if (player == null)
            {
                EditorUtility.DisplayDialog("Flashlight", "В сцене нет объекта Player.", "OK");
                return;
            }

            var flash = player.GetComponent<BrokenFlashlight>();
            if (flash == null)
                flash = Undo.AddComponent<BrokenFlashlight>(player);

            var cam = player.GetComponentInChildren<Camera>();
            if (cam == null && Camera.main != null)
                cam = Camera.main;

            var config = AssetDatabase.LoadAssetAtPath<FlashlightConfig>(ConfigPath);

            var so = new SerializedObject(flash);
            if (cam != null)
                so.FindProperty("_cameraTransform").objectReferenceValue = cam.transform;
            if (config != null)
                so.FindProperty("_config").objectReferenceValue = config;
            so.ApplyModifiedPropertiesWithoutUndo();

            EditorUtility.SetDirty(player);
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

            EditorUtility.DisplayDialog(
                "Flashlight",
                "Готово: BrokenFlashlight + FlashlightConfig.\n" +
                "В игре: F — вкл/выкл.\nCtrl+S → Play.",
                "OK");
        }
    }
}
#endif
