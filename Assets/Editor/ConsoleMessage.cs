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
        "Test/Editor extention/Sample",     //�K�w�p�X
        false,                              //���̃{�^�����g���邩
        1                                   //���j���[�̕��я�(����)
    )]
    private static void ShowWindow()
    {
        //���̃G�f�B�^�[�g���N���X��\������(�߂�l�͂��̃N���X�̃I�u�W�F�N�g)
        ConsoleMessage window = GetWindow<ConsoleMessage>();

        window.titleContent = new GUIContent("Sample Window");
    }

    /*---------------------------------------------------------------------------------
    *	
    *	�g���E�B���h�E�ɕ\����������
    *	 
    -----------------------------------------------------------------------------------*/
    private void OnGUI()
    {
        m_searchField = new SearchField();
        text = m_searchField.OnToolbarGUI(text);

        //�����ɂ̓e�N�X�`����GUIContext�ł���
        GUILayout.Label("���̕�������o�͂����");

        //�e�L�X�g�G���A�𐶐�
        text = EditorGUILayout.TextArea(text, GUILayout.Height(100));
        
        //�{�^���������ꂽ��
        if(GUILayout.Button("��������������������!"))
        {
            Debug.Log(text);
        }

        if (GUILayout.Button("�I�u�W�F�N�g����"))
        {
            GameObject gameObject = testObj;
        }
            
    }

    [SerializeField]
    private GameObject testObj;

    private SearchField m_searchField;

    private string text = "";
}
