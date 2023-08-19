using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor;
using UnityEngine;

public class DrawSphereGizmo
{
    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected)]
    private static void DrawSphere(DrawSphereGizmoObj obj, GizmoType gizmoType)
    {
        Gizmos.color = UnityEngine.Color.green;
        Gizmos.DrawWireSphere(obj.transform.position, obj.transform.localScale.x);
    }
}
