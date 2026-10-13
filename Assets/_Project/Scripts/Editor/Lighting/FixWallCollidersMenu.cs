#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Backrooms.EditorTools
{
    /// <summary>
    /// Меню: Backrooms → Fix Wall Colliders (for CharacterController).
    /// Переводит Wall_* на scale=1 + BoxCollider.size, визуал — дочерний Cube.
    /// </summary>
    public static class FixWallCollidersMenu
    {
        [MenuItem("Backrooms/Fix Wall Colliders (for CharacterController)")]
        public static void Fix()
        {
            int fixedCount = 0;
            foreach (var box in Object.FindObjectsByType<BoxCollider>(FindObjectsSortMode.None))
            {
                var go = box.gameObject;
                if (!go.name.StartsWith("Wall_"))
                    continue;
                if (go.transform.Find("Visual") != null)
                    continue;

                Vector3 size = go.transform.localScale;
                if (size == Vector3.one && box.size != Vector3.one)
                    continue; // уже в правильном формате

                Undo.RecordObject(go.transform, "Fix wall collider");
                Undo.RecordObject(box, "Fix wall collider");

                // Визуал
                var visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Undo.RegisterCreatedObjectUndo(visual, "Wall visual");
                visual.name = "Visual";
                visual.transform.SetParent(go.transform, false);
                visual.transform.localPosition = Vector3.zero;
                visual.transform.localRotation = Quaternion.identity;
                visual.transform.localScale = size;
                Object.DestroyImmediate(visual.GetComponent<Collider>());

                var oldRend = go.GetComponent<MeshRenderer>();
                var oldFilter = go.GetComponent<MeshFilter>();
                if (oldRend != null)
                {
                    var vr = visual.GetComponent<MeshRenderer>();
                    if (vr != null && oldRend.sharedMaterial != null)
                        vr.sharedMaterial = oldRend.sharedMaterial;
                    Undo.DestroyObjectImmediate(oldRend);
                }
                if (oldFilter != null)
                    Undo.DestroyObjectImmediate(oldFilter);

                go.transform.localScale = Vector3.one;
                box.size = size;
                box.center = Vector3.zero;
                fixedCount++;
            }

            EditorUtility.DisplayDialog(
                "Wall colliders",
                $"Исправлено стен: {fixedCount}.\nСразу Ctrl+S и Play — боковые стены должны держать.",
                "OK");
        }
    }
}
#endif
