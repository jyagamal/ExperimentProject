using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using Unity.VisualScripting;
using System.Linq;
using Codice.Client.BaseCommands.WkStatus.Printers;
using Unity.Burst.CompilerServices;
using static UnityEditor.EditorGUILayout;
using static UnityEditor.PlayerSettings;


public class StageEditorWindow : EditorWindow
{
    /******************************************************************************
    * 
    *                                    public
    * 
    *******************************************************************************/

    /*-----------------------------------�N���X------------------------------------*
    /*------------------------------------�萔------------------------------------*/
    /*------------------------------------�ϐ�------------------------------------*/
    /*----------------------------------Accesser----------------------------------*/
    /*-----------------------------------Getter-----------------------------------*/
    /*-----------------------------------Setter-----------------------------------*/
    /*------------------------------------�֐�------------------------------------*/

    /*---------------------------------------------------------------------------------
    *	
    *	�R���X�g���N�^
    *	 
    -----------------------------------------------------------------------------------*/
    static StageEditorWindow()
    {
        SceneView.duringSceneGui += CreatePrefab;
    }


    /*******************************************************************************
    * 
    *                                    private
    * 
    ********************************************************************************

    /*-----------------------------------�N���X------------------------------------*/
    /*------------------------------------�萔------------------------------------*/
    /*----------------------------SerializeField�ϐ�------------------------------*/
    /*------------------------------------�ϐ�------------------------------------*/

    //Prefabの親オブジェクト
    private static GameObject m_parentObj = null;

    //PrefabTable
    private PrefabDataTable m_prefabDataTable = null;

    //選択中のPrefabのアイコン
    private Texture2D m_nowSelectPrefabIcon;
    //選択中のPrefab
    private static GameObject m_targetPrefab = null;

    //ボタンの大きさ
    private float m_buttonSize = 60;
    //ボタンの間隔
    private int m_padding = 5;

    //�����_�������̐܂��݂��J���Ă��邩
    private bool m_isOpenRandomToggle = false;
    //ランダム生成にするか
    private static bool m_isRandom = false;
    //ランダム生成される範囲の半径
    private static float m_randRad = 1.0f;

    /*------------------------------------�֐�------------------------------------*/

    [MenuItem("StageEditor/EditorWindow/EditorWindow", false, 1)]
    private static void ShowWindow()
    {
        StageEditorWindow stageEditorWindow = EditorWindow.GetWindow<StageEditorWindow>();
    }

    /*---------------------------------------------------------------------------------
    *	
    *	�g���E�B���h�E�ɕ\����������
    *	 
    -----------------------------------------------------------------------------------*/
    private void OnGUI()
    {
        //ScriptableObjectを読み込む
        this.LoadPrefab();

        //Prefabの親オブジェクトを設定する
        m_parentObj = (GameObject)EditorGUILayout.ObjectField
            ("Prefabの親オブジェクト", m_parentObj, typeof(GameObject), true);

        m_buttonSize = EditorGUILayout.FloatField("ボタンの大きさ", m_buttonSize);
        m_padding = EditorGUILayout.IntField("ボタンの間隔", m_padding);

        EditorGUILayout.LabelField("選択中のPrefab");
        //�I�𒆂̃A�C�R����\��
        EditorGUILayout.LabelField(new GUIContent(m_nowSelectPrefabIcon),
            GUILayout.Height(64), GUILayout.Width(64));

        //�I�������{�^��
        if (GUILayout.Button("選択解除"))
        {
            m_targetPrefab = null;
            m_nowSelectPrefabIcon = null;
        }

        EditorGUILayout.Space(20);

        //�����_�������Ɋւ���܂��݂�\������
        //m_isOpenRandomToggle = EditorGUILayout.BeginFoldoutHeaderGroup(m_isOpenRandomToggle, "�����_������");

        using (EditorGUILayout.ToggleGroupScope randomGuiGroup = new EditorGUILayout.ToggleGroupScope("ランダム生成", m_isRandom))
        {
            m_isRandom = randomGuiGroup.enabled;

            //�����_�������̔��a�̒l
            m_randRad = EditorGUILayout.Slider("半径", m_randRad, 0.0f, 100.0f);
        }

        //�܂��݂��I������
        //EditorGUILayout.EndFoldoutHeaderGroup();

        int count = m_prefabDataTable.dataList.Count;

        Vector2 sceneSize = new Vector2(300, 500);

        foreach (int i in Enumerable.Range(0, count))
        {
            var data = m_prefabDataTable.dataList[i];

            //��ʉ����A�����A������Ȃ��R���g���[������Rect
            Rect rect = new Rect
            (
                sceneSize.x / 2 - m_buttonSize * count / 2 + m_buttonSize * i + m_padding * i,
                sceneSize.y - m_buttonSize * 1.6f,
                m_buttonSize,
                m_buttonSize
            );

            //�N���b�N���ꂽ�A�C�R����Prefab�𐶐�����Ώۂ�Prefab�ɂ���
            if (GUI.Button(rect, data.icon.texture))
            {
                m_targetPrefab = data.prefab;
                m_nowSelectPrefabIcon = data.icon.texture;
            }
        }
    }

    private void LoadPrefab()
    {
        //ScriptableObjectを読み込む
        m_prefabDataTable ??= AssetDatabase.LoadAssetAtPath<PrefabDataTable>
            ("Assets/Data/PrefabDataTable.asset");
    }

    private static void CreatePrefab(SceneView sceneView)
    {
        RaycastHit hit;
        GameObject hitObj = null;

        //Ray���΂��Ĕ��肷��
        if (IsCreatePrefab(out hit, out hitObj))
        {
            //EditorDrawGizmo.DrawPoint(hit.point, GizmoType.NotInSelectionHierarchy);

            if (m_targetPrefab == null 
            || Event.current.type != EventType.MouseDown
            || Event.current.button != 0)
                return;

            //Prefabを生成する
            GameObject prefab = (GameObject)PrefabUtility.InstantiatePrefab(m_targetPrefab);
            //座標をRayが当たった位置を基準に算出する
            prefab.transform.position = PrefabCreatePosition2HitObj(in hit, prefab, hitObj);

            //親オブジェクトを設定する
            if (m_parentObj != null)
                prefab.transform.SetParent(m_parentObj.transform);

            Selection.activeObject = prefab;

            //Prefab���o���������ƃV�[�����X�V���ꂸ�A�ۑ�����Ȃ��\�������邽�߁A
            //Undo�ɒǉ�����
            Undo.RegisterCreatedObjectUndo(prefab, "CreatePrefab");
        }
    }

    /*---------------------------------------------------------------------------------
    *	
    *	���e�@ : Prefab�������o���邩���肷��
    *	�����@ : Ray�̔��茋��
    *	�߂�l : true:�����o���� false:�o���Ȃ� 
    *	 
    -----------------------------------------------------------------------------------*/
    private static bool IsCreatePrefab(out RaycastHit hit, out GameObject hitObj)
    {
        if (EditorRaycastHelper.RaycastAgainstScene(out hit, out hitObj))
            return true;

        return false;
    }

    /*---------------------------------------------------------------------------------
    *	
    *	���e�@ : Ray�̓��������������Z�o���A�����ʒu���Z�o����
    *	�����@ : Ray�̓����������A��������v���n�u
    *	�߂�l : Prefab�̐����ʒu
    *	 
    -----------------------------------------------------------------------------------*/
    private static Vector3 PrefabCreatePosition2HitObj(in RaycastHit hit, GameObject prefab, GameObject hitObj)
    {
        Vector3 hitDir = hit.point - hitObj.transform.position;
        hitDir.Normalize();

        //比較用に正規化する
        Vector3 comparitionHitDir = new Vector3
        (
            Mathf.Abs(hitDir.x),
            Mathf.Abs(hitDir.y),
            Mathf.Abs(hitDir.z)
        );

        //ここから下もう少しスッキリさせたい
        Vector3 createPos = hitObj.transform.position;

        //x軸にRayが当たった場合
        if (comparitionHitDir.x >= comparitionHitDir.y 
            && comparitionHitDir.x >= comparitionHitDir.z)
        {
            //+ or -
            if (hitDir.x > 0)
                createPos.x += hitObj.transform.localScale.x / 2 +
                    prefab.transform.localScale.x / 2;
            else
                createPos.x -= hitObj.transform.localScale.x / 2 +
                    prefab.transform.localScale.x / 2;
        }
        //y軸にRayが当たった場合
        else if (comparitionHitDir.y >= comparitionHitDir.x
                 && comparitionHitDir.y >= comparitionHitDir.z)
        {
            //+ or -
            if (hitDir.y > 0)
                createPos.y += hitObj.transform.localScale.y / 2 +
                    prefab.transform.localScale.y / 2;
            else
                createPos.y -= hitObj.transform.localScale.y / 2 +
                    prefab.transform.localScale.y / 2;
        }
        //z軸にRayが当たった場合
        else
        {
            //+ or -
            if (hitDir.z > 0)
                createPos.z += hitObj.transform.localScale.z / 2 +
                    prefab.transform.localScale.z / 2;
            else
                createPos.z -= hitObj.transform.localScale.z / 2 +
                    prefab.transform.localScale.z / 2;
        }

        //ランダム生成じゃなければ、これ以上処理しない
        if (!m_isRandom)
            return createPos;

        float halfPi = Mathf.PI / 2;

        //ランダムにずらす方向
        float randRad = Random.Range(-halfPi, halfPi);
        //ランダムにずらす位置の割合
        float randDis = Random.Range(-halfPi, halfPi);

        //座標をずらす
        createPos += new Vector3
        (
            Mathf.Cos(randRad),
            0.0f,
            Mathf.Sin(randRad)
         ) * m_randRad * randDis;

        return createPos;
    }


}
