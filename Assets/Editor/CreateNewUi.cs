using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class CreateNewUi
{
    static CreateNewUi()
    {
        //シーンビューに独自のUIを追加する
        SceneView.duringSceneGui += OnGui;
    }

    private static void OnGui(SceneView sceneView)
    {
        Handles.BeginGUI();

        //ScriptableObjectを読み込む
        m_prefabDataTable = AssetDatabase.LoadAssetAtPath<PrefabDataTable>
            ("Assets/Data/PrefabDataTable.asset");

        //ShowButtons(sceneView.position.size);

        Handles.EndGUI();
    }

    private static void ShowButtons(Vector2 sceneSize)
    {
        int count = m_prefabDataTable.dataList.Count;
        int buttonSize = 60;
        int padding = 5;

        foreach (int i in Enumerable.Range(0, count))
        {
            var data = m_prefabDataTable.dataList[i];

            //画面下部、水平、中央寄席をコントロールするRect
            Rect rect = new Rect
            (
                sceneSize.x / 2 - buttonSize * count / 2 + buttonSize * i + padding * i,
                sceneSize.y - buttonSize * 1.6f,
                buttonSize,
                buttonSize
            );

            if (GUI.Button(rect, data.icon.texture))
            {
                //Prefabを生成する
                GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(data.prefab);

                Selection.activeObject = go;

                //Prefabを出すだけだとシーンが更新されず、保存されない可能性があるため、
                //Undoに追加する
                Undo.RegisterCreatedObjectUndo(go, "CreatePrefab");
            }

        }
    }

    //生成対象のprefab
    private static PrefabDataTable m_prefabDataTable;

}
