using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering;

public class DoTween : MonoBehaviour
{
    void Start()
    {
        this.transform.position = m_start.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
            this.transform.position = m_start.transform.position;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            //this.transform.DOMove
            //(
            //    m_end.transform.position,
            //    m_timer
            //).SetEase(ease);


            Sequence sequence = DOTween.Sequence();

            //アニメーションを数珠のようにつなげる
            sequence.Append
            (
                this.transform.DOMove
                (
                    m_end1.transform.position,    //目標座標
                    3.0f                      //何秒かけて移動するか
                ).SetEase(Ease.OutBounce)
            )
            .Append
            (
                this.transform.DOMove
                (
                    m_end2.transform.position,    //目標座標
                    2.0f                      //何秒かけて移動するか
                )
            )
            .Join//一つ上のアニメーションと同時に再生する
            (
                this.transform.DORotate
                (
                    Vector3.up * 180,         //目標の角度
                    2.0f                      //何秒かけて回転するか
                 )
            );
        }
    }

    [SerializeField]
    private Ease ease;

    [SerializeField]
    private GameObject m_start;

    [SerializeField]
    private GameObject m_end1;
    [SerializeField]
    private GameObject m_end2;

    [SerializeField]
    private float m_timer = 2.0f;
}
