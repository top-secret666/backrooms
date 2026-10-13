#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Backrooms.EditorTools
{
    /// <summary>
    /// Меню: Backrooms → Fix Wall UVs (no stretch).
    /// Чинит растянутые обои на Wall_*/Visual.
    /// </summary>
    public static class FixWallUVsMenu
    {
        private const float MetersPerTile = 2.5f;

        [MenuItem("Backrooms/Fix Wall UVs (no stretch)")]
        public static void Fix()
        {
            int fixedCount = 0;

            foreach (var box in Object.FindObjectsByType<BoxCollider>(FindObjectsSortMode.None))
            {
                if (!box.gameObject.name.StartsWith("Wall_"))
                    continue;

                Transform visual = box.transform.Find("Visual");
                if (visual == null)
                    continue;

                // Размер в метрах: либо BoxCollider.size (scale=1), либо scale Visual
                Vector3 size = box.size;
                if (visual.localScale != Vector3.one)
                    size = visual.localScale;

                Undo.RecordObject(visual, "Fix wall UV");
                var filter = visual.GetComponent<MeshFilter>();
                if (filter != null)
                    Undo.RecordObject(filter, "Fix wall UV");

                WallMeshUtility.ApplyUvMeshToVisual(visual, size, MetersPerTile);

                // Коллайдер на корне уже в метрах — выровняем size если Visual был источником
                Undo.RecordObject(box, "Fix wall UV");
                box.size = size;
                box.transform.localScale = Vector3.one;

                fixedCount++;
                EditorUtility.SetDirty(visual.gameObject);
                EditorUtility.SetDirty(box.gameObject);
            }

            // Wallpaper tiling = 1,1 — UV уже в метрах на меше
            var wallMat = AssetDatabase.LoadAssetAtPath<Material>(
                "Assets/_Project/Art/Materials/Mat_Wallpaper_Office.mat");
            if (wallMat != null)
            {
                wallMat.SetTextureScale("_BaseMap", Vector2.one);
                wallMat.SetTextureScale("_MainTex", Vector2.one);
                EditorUtility.SetDirty(wallMat);
            }

            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog(
                "Wall UVs",
                $"Исправлено стен: {fixedCount}.\nОбои больше не растягиваются по длинным кубам.\nCtrl+S и Play.",
                "OK");
        }
    }
}
#endif
