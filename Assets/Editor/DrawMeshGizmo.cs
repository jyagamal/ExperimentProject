using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DrawMeshGizmo
{
    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected)]
    private static void DrawMesh(DrawMeshGizmoObj obj, GizmoType gizmoType)
    {
        if (m_drawMesh == null)
            return;

        Gizmos.color = UnityEngine.Color.red;
        Gizmos.DrawWireMesh(m_drawMesh, obj.transform.position,
           obj.transform.rotation, obj.transform.localScale);
    }

    public static Mesh m_drawMesh;
}
