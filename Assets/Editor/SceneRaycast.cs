using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

[CustomEditor(typeof(SceneRaycast))]
public class SceneRaycast : Editor
{
    private void OnSceneGUI()
    {

        //�}�E�X�̈ʒu����Ray���΂�
        //GUI��Ray���΂����́A�֐����ς��

        //Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit hit;

        //Ray�ɂȂɂ�������Ȃ������珈�����Ȃ�

        if(Event.current.type == EventType.MouseDown && Event.current.button == 0)

        if (!EditorRaycastHelper.RaycastAgainstScene(out hit))
            return;

        Debug.Log("��������");//hit.transform.name);


    }
}
