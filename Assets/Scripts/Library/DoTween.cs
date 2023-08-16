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

            //�A�j���[�V�����𐔎�̂悤�ɂȂ���
            sequence.Append
            (
                this.transform.DOMove
                (
                    m_end1.transform.position,    //�ڕW���W
                    3.0f                      //���b�����Ĉړ����邩
                ).SetEase(Ease.OutBounce)
            )
            .Append
            (
                this.transform.DOMove
                (
                    m_end2.transform.position,    //�ڕW���W
                    2.0f                      //���b�����Ĉړ����邩
                )
            )
            .Join//���̃A�j���[�V�����Ɠ����ɍĐ�����
            (
                this.transform.DORotate
                (
                    Vector3.up * 180,         //�ڕW�̊p�x
                    2.0f                      //���b�����ĉ�]���邩
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
