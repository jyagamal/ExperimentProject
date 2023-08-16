using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class UniTaskObj : MonoBehaviour
{
    async void Start()
    {
        //Aキーを押すまで処理されない
        await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.A));
        Debug.Log("おはよう");

        //Aキーが押された上で、Sキーが押されるまで処理されない
        await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.S));
        Debug.Log("こんにちは");

        //AキーとSキーが押された上で、Dキーを押すまで処理されない
        await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.D));
        Debug.Log("こんばんは");
    }

    //目標の時間
    private float m_targetTime = 3.0f;

}
