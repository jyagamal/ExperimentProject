using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

[CustomEditor(typeof(SceneRaycast))]
public class SceneRaycast : Editor
{
    private void OnSceneGUI()
    {

        //マウスの位置からRayを飛ばす
        //GUIでRayを飛ばす時は、関数が変わる

        //Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit hit;

        //Rayになにも当たらなかったら処理しない

        if(Event.current.type == EventType.MouseDown && Event.current.button == 0)

        if (!EditorRaycastHelper.RaycastAgainstScene(out hit))
            return;

        Debug.Log("当たった");//hit.transform.name);


    }
}
