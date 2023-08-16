using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor;
using UnityEngine;

public class EditorDrawGizmo
{
    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected)]
    private static void DrawPoint(GameObject obj, GizmoType gizmoType)
    {
        Gizmos.DrawWireSphere(obj.transform.position, 1f);
    }
}
