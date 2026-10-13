#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Backrooms.EditorTools
{
    /// <summary>
    /// Меню: Backrooms → Fix Hell/Main Light Seam.
    /// Убирает «ступеньку» света на стыке полов (разные меши + разные лампы).
    /// </summary>
    public static class FixHellJunctionLightMenu
    {
        private const string FillName = "Light_JunctionFill";

        [MenuItem("Backrooms/Fix Hell/Main Light Seam")]
        public static void Fix()
        {
            var corridor = GameObject.Find("Level0_Corridor");
            if (corridor == null)
            {
                EditorUtility.DisplayDialog("Junction light", "Level0_Corridor не найден.", "OK");
                return;
            }

            // Мягкая заливка ровно в проёме Hell ↔ Main (примерно x = -12)
            Transform existing = corridor.transform.Find(FillName);
            GameObject fillGo;
            if (existing != null)
            {
                fillGo = existing.gameObject;
            }
            else
            {
                fillGo = new GameObject(FillName);
                Undo.RegisterCreatedObjectUndo(fillGo, "Junction fill");
                fillGo.transform.SetParent(corridor.transform, false);
            }

            fillGo.transform.localPosition = new Vector3(-12f, 2.6f, 0f);

            var light = fillGo.GetComponent<Light>();
            if (light == null)
                light = Undo.AddComponent<Light>(fillGo);

            light.type = LightType.Point;
            light.color = new Color(1f, 0.93f, 0.72f);
            light.intensity = 1.35f;
            light.range = 16f;
            light.shadows = LightShadows.None;
            light.renderMode = LightRenderMode.ForcePixel;

            // Чуть подтянуть ambient — меньше контраст «луж» на полу
            RenderSettings.ambientIntensity = 1.05f;
            RenderSettings.ambientLight = new Color(0.36f, 0.34f, 0.22f);

            // Если Floor_Hell чуть ниже/выше Main — выровнять Y
            var floorHell = corridor.transform.Find("Floor_Hell");
            var floorMain = corridor.transform.Find("Floor_Main");
            if (floorHell != null && floorMain != null)
            {
                Undo.RecordObject(floorHell, "Align Floor_Hell");
                Vector3 p = floorHell.position;
                p.y = floorMain.position.y;
                floorHell.position = p;
            }

            EditorUtility.SetDirty(fillGo);
            EditorUtility.DisplayDialog(
                "Junction light",
                "Добавлен мягкий Light_JunctionFill на стыке Hell/Main.\nТени у потолочных ламп выкл в префабах.\nCtrl+S → Play.",
                "OK");
        }
    }
}
#endif
