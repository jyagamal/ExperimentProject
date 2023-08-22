using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;


public class CreatePrefabPreview
{
    public static Texture2D RenderPreview(GameObject prefab, RenderTexture renderTexture = null)
    {
        if (!prefab || !EditorUtility.IsPersistent(prefab))
        {
            return null;
        }

        RenderTexture rt;
        if (renderTexture)
        {
            rt = renderTexture;
        }
        else
        {
            rt = new RenderTexture(128, 128, 24, RenderTextureFormat.ARGB32);
            rt.Create();
        }

        //�Ǝ��̃V�[���𐶐�����
        var preview = EditorSceneManager.NewPreviewScene();

        var container = new GameObject("Container");
        //�Q�[���I�u�W�F�N�g��ʂ̃V�[���Ɉړ�������
        EditorSceneManager.MoveGameObjectToScene(container, preview);

        //�v���r���[�p�̃J�����𐶐�����
        var cameraObject = new GameObject("Camera");
        cameraObject.transform.SetParent(container.transform);
        cameraObject.transform.SetPositionAndRotation
        (
            new Vector3(-32f, 45.7f, -32f),
            Quaternion.Euler(45f, 45f, 0f)
        );
        var camera = cameraObject.AddComponent<Camera>();
        camera.cameraType = CameraType.Preview;
        camera.fieldOfView = 1.5f;
        camera.nearClipPlane = 57.6f;
        camera.farClipPlane = 78.3f;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = Color.clear;

        camera.aspect = 1f;
        camera.targetTexture = rt;
        camera.forceIntoRenderTexture = true;
        camera.renderingPath = RenderingPath.Forward;
        //camera.cullingMask = 1 << 30;
        camera.scene = preview;

        //���C�g�𐶐�����
        var lightObject = new GameObject("Camera");
        lightObject.transform.SetParent(container.transform);
        lightObject.transform.SetPositionAndRotation(
            Vector3.zero,
            Quaternion.Euler(60f, 45f, 0f)
        );
        var light = lightObject.AddComponent<Light>();
        light.type = LightType.Directional;
        light.color = Color.white;
        light.shadows = LightShadows.Soft;
        light.lightmapBakeType = LightmapBakeType.Realtime;

        //�Ώۂ̃v���n�u�𐶐�����
        var go = UnityEngine.Object.Instantiate(
            prefab,
            Vector3.zero,
            Quaternion.identity,
            container.transform
        );

        var animator = go.GetComponentInChildren<Animator>();
        if (animator)
        {
            animator.Play("idle", -1, 0.1f);
            animator.Update(0.1f);
            animator.speed = 0f;
        }

        //�J�����̕`����e�N�X�`���[�ɏ�������
        camera.Render();

        var old = RenderTexture.active;
        RenderTexture.active = rt;
        var st = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
        
        //�J�����ŎB�����摜��ǂݍ���
        st.ReadPixels(new Rect(0f, 0f, rt.width, rt.height), 0, 0);
        RenderTexture.active = old;

        camera.targetTexture = null;
        camera.forceIntoRenderTexture = false;
        if (!renderTexture)
        {
            rt.Release();
            UnityEngine.Object.DestroyImmediate(rt);
        }
        //UnityEngine.Object.DestroyImmediate(cameraObject);
        //UnityEngine.Object.DestroyImmediate(go);
        //UnityEngine.Object.DestroyImmediate(prefabContainer);

        //�Ǝ��̃V�[�������
        EditorSceneManager.ClosePreviewScene(preview);

        return st;
    }

    [MenuItem("Assets/Create Preview")]
    public static void CreatePreview()
    {
        //�I�𒆂̃I�u�W�F�N�g���擾����
        var selection = Selection.gameObjects;
        if (selection == null || selection.Length == 0)
        {
            return;
        }

        var items = new List<GameObject>();
        //�I�𒆂̑S�ẴI�u�W�F�N�g���Ǘ�����
        foreach (var go in selection)
        {
            if (!EditorUtility.IsPersistent(go))
            {
                continue;
            }


            items.Add(go);
            //ProcessAsset(go);
        }
        if (items.Count == 0)
        {
            return;
        }

        const int size = 128;
        var rt = new RenderTexture(size, size, 24, RenderTextureFormat.ARGB32);
        rt.Create();
        foreach (var item in items)
        {
            //�v���r���[�𐶐�����
            var preview = RenderPreview(item, rt);
            //�ۑ���̃p�X��ݒ肷��
            var iconPath = "Assets/" + item.name + ".png";
            File.WriteAllBytes(iconPath, preview.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(preview);
            AssetDatabase.ImportAsset(iconPath, ImportAssetOptions.ForceUpdate);
        }
        rt.Release();
        UnityEngine.Object.DestroyImmediate(rt);
    }
}

