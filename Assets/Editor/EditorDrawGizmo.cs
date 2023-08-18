using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor;
using UnityEngine;

public class EditorDrawGizmo
{
    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected)]
    private static void DrawPoint(DrawGizmoObj obj, GizmoType gizmoType)
    {
        Gizmos.color = UnityEngine.Color.green;
        Gizmos.DrawWireSphere(obj.transform.position, obj.transform.localScale.x);
    }
}
