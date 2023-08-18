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

//起動時&コンパイル時にコンストラクタを呼び出す
[InitializeOnLoad]
public class StageEditorWindow : EditorWindow
{
    /******************************************************************************
    * 
    *                                    public
    * 
    *******************************************************************************/

    /*-----------------------------------クラス------------------------------------*
    /*------------------------------------定数------------------------------------*/
    /*------------------------------------変数------------------------------------*/
    /*----------------------------------Accesser----------------------------------*/
    /*-----------------------------------Getter-----------------------------------*/
    /*-----------------------------------Setter-----------------------------------*/
    /*------------------------------------関数------------------------------------*/


    /*******************************************************************************
    * 
    *                                    private
    * 
    ********************************************************************************

    /*-----------------------------------クラス------------------------------------*/
    /*------------------------------------定数------------------------------------*/
    /*----------------------------SerializeField変数------------------------------*/
    /*------------------------------------変数------------------------------------*/

    //初回起動時かどうか
    static private bool m_isStartUp = true;

    //Prefabの親オブジェクト
    private static GameObject m_parentObj = null;

    //PrefabTable
    private PrefabDataTable m_prefabDataTable = null;

    //選択中のPrefabのアイコン
    private Texture2D m_nowSelectPrefabIcon;
    //選択中のPrefab
    private static GameObject m_targetPrefab = null;

    //エディターに存在するGizmoの親オブジェクト
    static private GameObject m_editorGizmos = null; 
    //Rayの着地点にGizmoを表示するオブジェクト
    static private GameObject m_rayPointGizmoObj = null;
    //ランダム生成の範囲を表示するオブジェクト
    static private GameObject m_RandRadGizmoObj = null;

    //ボタンの大きさ
    private float m_buttonSize = 60;
    //ボタンの間隔
    private int m_padding = 5;

    //ランダム生成にするか
    private static bool m_isRandom = false;
    //ランダム生成される範囲の半径
    private static float m_randRad = 1.0f;

    /*------------------------------------関数------------------------------------*/

    /*---------------------------------------------------------------------------------
    *	
    *   コンストラクタ
    *	 
    -----------------------------------------------------------------------------------*/
    static StageEditorWindow()
    {
        SceneView.duringSceneGui += CreatePrefab;
    }

    private void OnEnable()
    {
        //親ギズモがあるか調べる
        if (GameObject.Find("EditorGizmos") == null)
        {
            //親ギズモを生成する
            GameObject editorGizmos = AssetDatabase.LoadAssetAtPath<GameObject>
                ("Assets/Editor/EditorGizmos.prefab");

            m_editorGizmos = (GameObject)PrefabUtility.InstantiatePrefab(editorGizmos);
        }
        else
        {
            //親ギズモオブジェクトを読み込む
            m_editorGizmos = GameObject.Find("EditorGizmos");
        }

        //親ギズモから子ギズモを取得する
        m_rayPointGizmoObj = m_editorGizmos.transform.GetChild(0).gameObject;
        m_RandRadGizmoObj = m_editorGizmos.transform.GetChild(1).gameObject;

    }

    [MenuItem("StageEditor/EditorWindow/EditorWindow", false, 1)]
    private static void ShowWindow()
    {
        StageEditorWindow stageEditorWindow = EditorWindow.GetWindow<StageEditorWindow>();
    }

    /*---------------------------------------------------------------------------------
    *	
    *	ウィンドウに表示されるもの
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

        //選択中のPrefabのアイコンを表示
        EditorGUILayout.LabelField("選択中のPrefab");
        EditorGUILayout.LabelField(new GUIContent(m_nowSelectPrefabIcon),
            GUILayout.Height(64), GUILayout.Width(64));

        //選択を解除
        if (GUILayout.Button("選択解除"))
        {
            m_targetPrefab = null;
            m_nowSelectPrefabIcon = null;
        }

        EditorGUILayout.Space(20);

        //ランダム生成系をグループでまとめる
        using (EditorGUILayout.ToggleGroupScope randomGuiGroup = new EditorGUILayout.ToggleGroupScope("ランダム生成", m_isRandom))
        {
            m_isRandom = randomGuiGroup.enabled;

            //ランダム生成が有効ならランダム生成範囲を表示するGizmoを表示する
            if (m_RandRadGizmoObj.activeSelf != m_isRandom)
                m_RandRadGizmoObj.SetActive(m_isRandom);

            //ランダム生成する半径
            m_randRad = EditorGUILayout.Slider("半径", m_randRad, 0.0f, 100.0f);

            //ギズモの半径とインスペクターで設定した値を同期させる
            m_RandRadGizmoObj.transform.localScale = new Vector3
            (
                m_randRad,
                m_RandRadGizmoObj.transform.localScale.y,
                m_RandRadGizmoObj.transform.localScale.z
             );
        }

        int count = m_prefabDataTable.dataList.Count;

        Vector2 sceneSize = new Vector2(300, 500);

        foreach (int i in Enumerable.Range(0, count))
        {
            var data = m_prefabDataTable.dataList[i];

            //ボタンのRect
            Rect rect = new Rect
            (
                sceneSize.x / 2 - m_buttonSize * count / 2 + m_buttonSize * i + m_padding * i,
                sceneSize.y - m_buttonSize * 1.6f,
                m_buttonSize,
                m_buttonSize
            );

            //クリックされたものを選択対象のPrefabにする
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

        //Rayの当たり判定
        if (IsCreatePrefab(out hit, out hitObj))
        {
            //Gizmoを表示する
            if (!m_editorGizmos.transform.GetChild(0).gameObject.activeSelf)
            {
                for (int i = 0; i < m_editorGizmos.transform.childCount; i++)
                {
                    m_editorGizmos.transform.GetChild(i).gameObject.SetActive(true);
                }
            }

            //ギズモの座標をPrefabの生成地点にする
            Vector3 rayPoint = PrefabCreatePosition2HitObj
                (in hit, m_rayPointGizmoObj, hitObj);

            m_rayPointGizmoObj.transform.position = rayPoint;
            m_RandRadGizmoObj.transform.position = rayPoint;

            //Prefabが選択されている上で左クリックするまで、
            //これ以上処理しない
            if (m_targetPrefab == null 
            || Event.current.type != EventType.MouseDown
            || Event.current.button != 0)
                return;

            //Prefabを生成する
            GameObject prefab = (GameObject)PrefabUtility.InstantiatePrefab(m_targetPrefab);
            
            //座標をRayが当たった位置を基準に算出する
            if (prefab == null)
                return;
            prefab.transform.position = PrefabCreatePosition2HitObj(in hit, prefab, hitObj);

            //ランダムな値を足す
            if (m_isRandom)
                prefab.transform.position += RandomPos();

            //親オブジェクトを設定する
            if (m_parentObj != null)
                prefab.transform.SetParent(m_parentObj.transform);

            Selection.activeObject = prefab;

            //Prefabを出すだけだとシーンが保存されない可能性があるため
            //Undoに追加する
            Undo.RegisterCreatedObjectUndo(prefab, "CreatePrefab");
        }
        else
        {
            if (m_editorGizmos.transform.GetChild(0).gameObject.activeSelf)
            {
                //Rayが当たっていない時は、Gizmoを非表示にする
                for (int i = 0; i < m_editorGizmos.transform.childCount; i++)
                {
                    m_editorGizmos.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
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
    *	内容　 : プレハブを生成する位置を算出する
    *	引数　 : Rayの当たった結果、prefab、生成したいプレハブ
    *	戻り値 : 生成する位置
    *	 
    -----------------------------------------------------------------------------------*/
    private static Vector3 PrefabCreatePosition2HitObj(in RaycastHit hit, GameObject obj, GameObject hitObj)
    {
        if (hitObj == null)
            return Vector3.zero;

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
                createPos.x += hitObj.transform.lossyScale.x / 2 +
                    obj.transform.localScale.x / 2;
            else
                createPos.x -= hitObj.transform.lossyScale.x / 2 +
                    obj.transform.localScale.x / 2;
        }
        //y軸にRayが当たった場合
        else if (comparitionHitDir.y >= comparitionHitDir.x
                 && comparitionHitDir.y >= comparitionHitDir.z)
        {
            //+ or -
            if (hitDir.y > 0)
                createPos.y += hitObj.transform.lossyScale.y / 2 +
                    obj.transform.localScale.y / 2;
            else
                createPos.y -= hitObj.transform.lossyScale.y / 2 +
                    obj.transform.localScale.y / 2;
        }
        //z軸にRayが当たった場合
        else
        {
            //+ or -
            if (hitDir.z > 0)
                createPos.z += hitObj.transform.lossyScale.z / 2 +
                    obj.transform.localScale.z / 2;
            else
                createPos.z -= hitObj.transform.lossyScale.z / 2 +
                    obj.transform.localScale.z / 2;
        }

        return createPos;
    }

    private static Vector3 RandomPos()
    {
        //ランダム生成じゃなければ、これ以上処理しない
        if (!m_isRandom)
            return Vector3.zero;

        float halfPi = Mathf.PI / 2;

        //ランダムにずらす方向
        float randRad = Random.Range(-halfPi, halfPi);
        //ランダムにずらす位置の割合
        float randDis = Random.Range(-halfPi, halfPi);

        //座標をずらす
        return new Vector3
        (
            Mathf.Cos(randRad),
            0.0f,
            Mathf.Sin(randRad)
         ) * m_randRad * randDis;
    }

}
