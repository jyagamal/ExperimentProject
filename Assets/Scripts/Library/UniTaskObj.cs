using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class UniTaskObj : MonoBehaviour
{
    async void Start()
    {
        //A�L�[�������܂ŏ�������Ȃ�
        await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.A));
        Debug.Log("���͂悤");

        //A�L�[�������ꂽ��ŁAS�L�[���������܂ŏ�������Ȃ�
        await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.S));
        Debug.Log("����ɂ���");

        //A�L�[��S�L�[�������ꂽ��ŁAD�L�[�������܂ŏ�������Ȃ�
        await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.D));
        Debug.Log("����΂��");
    }

    //�ڕW�̎���
    private float m_targetTime = 3.0f;

}
