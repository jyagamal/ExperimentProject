using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FileSave
{ 
    // 推奨するディレクトリがあればpathに入れておく
    public static void SaveToPass(string path = "Assets/", string ext = "png")
    {

        if (string.IsNullOrEmpty(path) || System.IO.Path.GetExtension(path) != "." + ext)
        {
            // 推奨する保存パスがないときはシーンのディレクトリをとってきたりする（用途次第）
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

        // ディレクトリがなければ作る
        else if (System.IO.Directory.Exists(path) == false)
        {
            System.IO.Directory.CreateDirectory(path);
        }

        // ファイル保存パネルを表示
        var fileName = "name." + ext;
        fileName = System.IO.Path.GetFileNameWithoutExtension(AssetDatabase.GenerateUniqueAssetPath(System.IO.Path.Combine(path, fileName)));
        path = EditorUtility.SaveFilePanelInProject("Save Some Asset", fileName, ext, "", path);

        if (!string.IsNullOrEmpty(path))
        {
            // 保存処理
            Debug.Log(path);
        }
    }
}
