using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FileSave
{ 
    // ��������f�B���N�g���������path�ɓ���Ă���
    public static void SaveToPass(string path = "Assets/", string ext = "png")
    {

        if (string.IsNullOrEmpty(path) || System.IO.Path.GetExtension(path) != "." + ext)
        {
            // ��������ۑ��p�X���Ȃ��Ƃ��̓V�[���̃f�B���N�g�����Ƃ��Ă����肷��i�p�r����j
            if (string.IsNullOrEmpty(path))
            {
                path = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path;
                if (!string.IsNullOrEmpty(path))
                {
                    path = System.IO.Path.GetDirectoryName(path);
                }
            }
            if (string.IsNullOrEmpty(path))
            {
                path = "Assets";
            }
        }

        // �f�B���N�g�����Ȃ���΍��
        else if (System.IO.Directory.Exists(path) == false)
        {
            System.IO.Directory.CreateDirectory(path);
        }

        // �t�@�C���ۑ��p�l����\��
        var fileName = "name." + ext;
        fileName = System.IO.Path.GetFileNameWithoutExtension(AssetDatabase.GenerateUniqueAssetPath(System.IO.Path.Combine(path, fileName)));
        path = EditorUtility.SaveFilePanelInProject("Save Some Asset", fileName, ext, "", path);

        if (!string.IsNullOrEmpty(path))
        {
            // �ۑ�����
            Debug.Log(path);
        }
    }
}
