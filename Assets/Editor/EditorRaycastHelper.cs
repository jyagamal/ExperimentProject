using System.Reflection;
using Unity.Burst.CompilerServices;
using UnityEditor;
using UnityEngine;

public static class EditorRaycastHelper
{
    private static readonly MethodInfo intersectRayMeshMethod = typeof(HandleUtility).GetMethod("IntersectRayMesh",
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

    private static GameObject lastGameObjectUnderCursor;

    public static bool RaycastAgainstScene(out RaycastHit hit, out GameObject hitObj)
    {
        Ray ray;

        RayCheckHit(out ray, out hitObj);

        // Raycast against scene geometry with colliders
        object raycastResult = HandleUtility.RaySnap(ray);
        if (raycastResult != null && raycastResult is RaycastHit)
        {
            hit = (RaycastHit)raycastResult;
            return true;
        }

        hit = new RaycastHit();
        return false;
    }
    public static bool RaycastAgainstScene(out RaycastHit hit)
    {
        Ray ray;
        GameObject hitObj = null;

        RayCheckHit(out ray, out hitObj);

        // Raycast against scene geometry with colliders
        object raycastResult = HandleUtility.RaySnap(ray);
        if (raycastResult != null && raycastResult is RaycastHit)
        {
            hit = (RaycastHit)raycastResult;
            return true;
        }

        hit = new RaycastHit();
        return false;
    }

    private static void RayCheckHit(out Ray ray, out GameObject hitObj)
    {
        //GUI用のRayを生成
        ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

        // First, try raycasting against scene geometry with or without colliders (it doesn't matter)
        // Credit: https://forum.unity.com/threads/editor-raycast-against-scene-meshes-without-collider-editor-select-object-using-gui-coordinate.485502
        GameObject gameObjectUnderCursor;

        //マウスカーソルの位置によって処理を変えている？
        switch (Event.current.type)
        {
            // HandleUtility.PickGameObject doesn't work with some EventTypes in OnSceneGUI
            case EventType.Layout:
            case EventType.Repaint:
            case EventType.ExecuteCommand:
                gameObjectUnderCursor = lastGameObjectUnderCursor;
                break;
            default:
                //これにRayが当たったオブジェクトが入ってそう？
                gameObjectUnderCursor = HandleUtility.PickGameObject(Event.current.mousePosition, false);
                break;
        }

        if (gameObjectUnderCursor)
        {
            Mesh meshUnderCursor = null;
            if (gameObjectUnderCursor.TryGetComponent(out MeshFilter meshFilter))
                meshUnderCursor = meshFilter.sharedMesh;
            if (!meshUnderCursor &&
                gameObjectUnderCursor.TryGetComponent(out SkinnedMeshRenderer skinnedMeshRenderer))
                meshUnderCursor = skinnedMeshRenderer.sharedMesh;

            //当たったオブジェクトを取得する
            hitObj = gameObjectUnderCursor as GameObject;

            if (meshUnderCursor)
            {
                // Remember this GameObject so that it can be used inside problematic EventTypes, as well
                lastGameObjectUnderCursor = gameObjectUnderCursor;

                object[] rayMeshParameters = new object[]
                    {ray, meshUnderCursor, gameObjectUnderCursor.transform.localToWorldMatrix, null};
                if ((bool)intersectRayMeshMethod.Invoke(null, rayMeshParameters))
                {
                    
                    return;
                }
            }
            else
            {
                lastGameObjectUnderCursor = null;
            }
        }

        hitObj = null;
    }
}
