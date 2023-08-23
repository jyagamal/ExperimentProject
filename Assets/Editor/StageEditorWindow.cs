using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using Unity.VisualScripting;
using System.Linq;
using Codice.Client.BaseCommands.WkStatus.Printers;
using static UnityEditor.EditorGUILayout;
using static UnityEditor.PlayerSettings;
using System.IO;
using GluonGui.WorkspaceWindow.Views.WorkspaceExplorer;
using UnityEngine.Rendering;

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
    //Prefabのメッシュを仮で描画するオブジェクト
    static private GameObject m_demoMeshObj = null;
    //ランダム生成の範囲を表示するオブジェクト
    static private GameObject m_RandRadGizmoObj = null;
    

    //ボタンの大きさ
    private float m_buttonSize = 60;
    //ボタンの間隔
    private int m_padding = 5;

    //Rayに当たったオブジェクトの座標を参照するか
    private static bool m_isUseRayHitObj = false;

    //ランダム生成にするか
    private static bool m_isRandom = false;
    //ランダム生成される範囲の半径
    private static float m_randRad = 1.0f;
    //半径の区切り
    private float[] m_randRadRangeArray = new float[] { 1.0f, 10.0f, 50.0f, 100.00f, 500.00f };
    //適応する半径の区切り
    private int m_randRadArrEle = 0;
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
        m_demoMeshObj = m_editorGizmos.transform.GetChild(2).gameObject;

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

        if (GUILayout.Button("全てのプレハブを読み込む"))
        {
            //読み込むPrefabがあるパスを指定させる
            var fullFilePath = EditorUtility.SaveFolderPanel("LoadPrefabFolder", "Assets", "");

            Debug.Log(fullFilePath);

            //ディレクトリが存在しなければ処理しない
            if (!Directory.Exists(fullFilePath))
                return;

            //このままだとフルパスになっているため、
            //Assets/までパスを短縮する
            string relativeFilePath = System.Text.RegularExpressions.
                Regex.Match(fullFilePath, "Assets/.*").Value;

           //データテーブルを消去する
            m_prefabDataTable.Clear();

            //プレハブのアイコン画像を全て破棄する
            //this.DirectoryDelete("Assets/Editor/Data/PreviewIcon");

            //プレハブを読み込む
            var guids = AssetDatabase.FindAssets("t:GameObject", new string[] { relativeFilePath });
            var paths = guids.Select(guid => AssetDatabase.GUIDToAssetPath(guid)).ToArray();
            var list = paths.Select(_ => AssetDatabase.LoadAssetAtPath<GameObject>(_)).ToList();

            if (list.Count == 0)
            {
                Debug.LogWarning("プレハブがありません\n" + "検索パス[" + relativeFilePath + "]");
                return;
            }

            Debug.Log("プレハブを検索:" + list.Count + "つ格納\n" + "検索パス[" + relativeFilePath + "]");

            foreach (GameObject prefab in list)
            {
                //プレハブのプレビュー写真を生成する
                Texture2D prevIcon =  CreatePrefabPreview.RenderPreview(prefab);
                //色を反映する
                prevIcon.Apply();

                string prevIconPath = "/Editor/Data/PreviewIcon/" + prefab.name + ".png";

                //PNG画像を生成する
                //ここだけフルパスじゃないとダメっぽい
                File.WriteAllBytes(Application.dataPath + prevIconPath, prevIcon.EncodeToPNG());

                //プレビュー写真をアセットとしてプロジェクトに読み込む
                //(.metaが生成されることでプロジェクトに反映される)
                AssetDatabase.ImportAsset("Assets" + prevIconPath);

                //アセットとしてのプレビュー写真を読み込む
                var prevIconAssets = AssetDatabase.LoadAssetAtPath
                    ("Assets" +prevIconPath, typeof(Texture2D));

                //データテーブルに追加する
                m_prefabDataTable.AddPrefab(prefab, prevIconAssets as Texture2D);

            }
        }

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
            DrawMeshGizmo.m_drawMesh = null;
        }

        EditorGUILayout.Space(20);

        m_isUseRayHitObj = EditorGUILayout.Toggle("座標補正", m_isUseRayHitObj);

        //ランダム生成系をグループでまとめる
        using (EditorGUILayout.ToggleGroupScope randomGuiGroup = new EditorGUILayout.ToggleGroupScope("ランダム生成", m_isRandom))
        {
            m_isRandom = randomGuiGroup.enabled;

            //ランダム生成が有効なら、ランダム生成範囲を表示するGizmoを表示する
            if (m_RandRadGizmoObj.activeSelf != m_isRandom)
                m_RandRadGizmoObj.SetActive(m_isRandom);

            //ランダム生成する範囲
            float min = m_randRadRangeArray[m_randRadArrEle],
                  max = m_randRadRangeArray[m_randRadArrEle +1] ;

            //100.0f以上がGUI上だと小数点表記が無くなるため調整
            float offset = (m_randRadRangeArray[m_randRadArrEle] >= 100.0f) ?
                0.1f : 1f;

            m_randRad = EditorGUILayout.Slider("半径", m_randRad, min - offset, max);

            //現在の範囲内の最大値まで来たら、最大値を引き上げる
            if (m_randRad >= max && m_randRadArrEle < m_randRadRangeArray.Length - 2)
                    m_randRadArrEle+= 1;
            //現在の範囲内の最小値まできたら、最小値を引き下げる
            else if (m_randRad < min && m_randRadArrEle > 0)
                    m_randRadArrEle-= 1;


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
            if (GUI.Button(rect, data.icon))
            {
                m_targetPrefab = data.prefab;
                m_nowSelectPrefabIcon = data.icon;

                //選択対象のMeshを取得する
                MeshFilter demoMesh;
                m_targetPrefab.TryGetComponent<MeshFilter>(out demoMesh);

                DrawMeshGizmo.m_drawMesh = demoMesh.sharedMesh;

                //Transformを同期する
                m_demoMeshObj.transform.position = m_targetPrefab.transform.position;
                m_demoMeshObj.transform.localScale = m_targetPrefab.transform.localScale;
                m_demoMeshObj.transform.rotation = m_targetPrefab.transform.rotation;
            }
        }
    }

    private void LoadPrefab()
    {
        //ScriptableObjectを読み込む
        m_prefabDataTable ??= AssetDatabase.LoadAssetAtPath<PrefabDataTable>
            ("Assets/Editor/Data/PrefabDataTable.asset");
    }

    private static void CreatePrefab(SceneView sceneView)
    {
        RaycastHit hit;
        GameObject hitObj = null;

        //Rayの当たり判定
        if (EditorRaycastHelper.RaycastAgainstScene(out hit, out hitObj))
        {
            GameObject targetObj = null;

            //Rayの着地点を表示する
            if (m_targetPrefab == null)
            {
                m_rayPointGizmoObj.SetActive(true);
                targetObj ??= m_rayPointGizmoObj;
            }
            //Rayの着地点に選択中のPrefabのメッシュを表示する
            else
            {
                m_demoMeshObj.SetActive(true);
                targetObj ??= m_demoMeshObj;
            }

            //ギズモの座標をPrefabの生成地点にする
            Vector3 rayPoint = PrefabCreatePosition2HitObj(in hit, targetObj, hitObj);

            m_rayPointGizmoObj.transform.position = rayPoint;
            m_RandRadGizmoObj.transform.position = rayPoint;
            m_demoMeshObj.transform.position = rayPoint;

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
            //Rayが当たっていない時は、Gizmoを非表示にする
            if (m_editorGizmos.transform.GetChild(0).gameObject.activeSelf)
            {
                for (int i = 0; i < m_editorGizmos.transform.childCount; i++)
                {
                    m_editorGizmos.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
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
        
        Vector3 hitDirNor = hitDir.normalized;

        //比較用に符号をなくす
        Vector3 comparitionHitDir = new Vector3
        (
            Mathf.Abs(hitDirNor.x),
            Mathf.Abs(hitDirNor.y),
            Mathf.Abs(hitDirNor.z)
        );

        //ここから下もう少しスッキリさせたい
        Vector3 createPos;

        if (m_isUseRayHitObj)
        {
            createPos = hitObj.transform.position;

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
        }
        else
        {
            createPos = hit.point;

            Vector3 scaleOffset = new Vector3
            (
                hitDirNor.x * obj.transform.localScale.x / 2,
                hitDirNor.y * obj.transform.localScale.y / 2,
                hitDirNor.z * obj.transform.localScale.z / 2
            );

            createPos += scaleOffset;
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

    /*---------------------------------------------------------------------------------
    *	
    *	内容　 : 対象のディレクトリの中身を破棄する
    *	引数　 : ディレクトリのパス
    *	戻り値 : なし
    *	 
    -----------------------------------------------------------------------------------*/   
    private void DirectoryDelete(string targetDirectoryPath)
    {
        //ディレクトリが存在しなければ処理しない
        if (!Directory.Exists(targetDirectoryPath))   
            return;

        //ディレクトリ以外の全ファイルを削除
        string[] filePaths = Directory.GetFiles(targetDirectoryPath);
        foreach (string filePath in filePaths)
        {
            File.SetAttributes(filePath, FileAttributes.Normal);
            File.Delete(filePath);
        }

        //ディレクトリの中のディレクトリも再帰的に削除
        string[] directoryPaths = Directory.GetDirectories(targetDirectoryPath);
        foreach (string directoryPath in directoryPaths)
        {
            DirectoryDelete(directoryPath);
        }

        //中が空になったらディレクトリ自身も削除
        Directory.Delete(targetDirectoryPath, false);
    }

}
