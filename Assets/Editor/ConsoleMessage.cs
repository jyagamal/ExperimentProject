using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.IMGUI.Controls;

#if UNITY_EDITOR
using UnityEditor;
#endif 

public class ConsoleMessage : EditorWindow
{
    [MenuItem
    (
        "Test/Editor extention/Sample",     //階層パス
        false,                              //このボタンが使えるか
        1                                   //メニューの並び順(昇順)
    )]
    private static void ShowWindow()
    {
        //このエディター拡張クラスを表示する(戻り値はこのクラスのオブジェクト)
        ConsoleMessage window = GetWindow<ConsoleMessage>();

        window.titleContent = new GUIContent("Sample Window");
    }

    /*---------------------------------------------------------------------------------
    *	
    *	拡張ウィンドウに表示されるもの
    *	 
    -----------------------------------------------------------------------------------*/
    private void OnGUI()
    {
        m_searchField = new SearchField();
        text = m_searchField.OnToolbarGUI(text);

        //引数にはテクスチャやGUIContextでも可
        GUILayout.Label("この文字列を出力するよ");

        //テキストエリアを生成
        text = EditorGUILayout.TextArea(text, GUILayout.Height(100));
        
        //ボタンが押されたら
        if(GUILayout.Button("押せぇぇぇぇぇぇぇぇ!"))
        {
            Debug.Log(text);
        }

        if (GUILayout.Button("オブジェクト生成"))
        {
            GameObject gameObject = testObj;
        }
            
    }

    [SerializeField]
    private GameObject testObj;

    private SearchField m_searchField;

    private string text = "";
}
