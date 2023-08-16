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
        //�V�[���r���[�ɓƎ���UI��ǉ�����
        SceneView.duringSceneGui += OnGui;
    }

    private static void OnGui(SceneView sceneView)
    {
        Handles.BeginGUI();

        //ScriptableObject��ǂݍ���
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

            //��ʉ����A�����A������Ȃ��R���g���[������Rect
            Rect rect = new Rect
            (
                sceneSize.x / 2 - buttonSize * count / 2 + buttonSize * i + padding * i,
                sceneSize.y - buttonSize * 1.6f,
                buttonSize,
                buttonSize
            );

            if (GUI.Button(rect, data.icon.texture))
            {
                //Prefab�𐶐�����
                GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(data.prefab);

                Selection.activeObject = go;

                //Prefab���o���������ƃV�[�����X�V���ꂸ�A�ۑ�����Ȃ��\�������邽�߁A
                //Undo�ɒǉ�����
                Undo.RegisterCreatedObjectUndo(go, "CreatePrefab");
            }

        }
    }

    //�����Ώۂ�prefab
    private static PrefabDataTable m_prefabDataTable;

}
