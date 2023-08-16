using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_Square : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.Animation();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Animation()
    {
        //

        Tween[] animations =
        {
            this.transform
            .DOMove(m_lt.transform.position, m_aniTime)
            .SetEase(Ease.OutQuart),

            this.transform.DORotate(Vector3.forward * 540, m_aniTime),

            this.transform
            .DOMove(m_rt.transform.position, m_aniTime)
            .SetEase(Ease.OutQuart),
            this.transform.DORotate(Vector3.forward * 0, m_aniTime),

            this.transform
            .DOMove(m_rb.transform.position, m_aniTime)
            .SetEase(Ease.OutQuart),

            this.transform.DORotate(Vector3.forward * 540, m_aniTime),

            this.transform
            .DOMove(m_lb.transform.position, m_aniTime)
            .SetEase(Ease.OutQuart),

            this.transform.DORotate(Vector3.forward * 0, m_aniTime),
        };

        Sequence sequence = DOTween.Sequence();

        if(m_startId == aniId.Lt2Rt)
        {
            sequence
                .Append(animations[2]).Join(animations[3])
                .Append(animations[4]).Join(animations[5])
                .Append(animations[6]).Join(animations[7])
                .Append(animations[0]).Join(animations[1]);
        }
        else
        {
            sequence
                .Append(animations[6]).Join(animations[7])
                .Append(animations[0]).Join(animations[1])
                .Append(animations[2]).Join(animations[3])
                .Append(animations[4]).Join(animations[5]);
        }

        sequence.SetLoops(-1);
    }

    private enum aniId
    {
        Lt2Rt = 0,  //左上から右上
        Rt2Rb,      //右上から右下
        Rb2Lb,      //右下から左下
        Lb2Rt       //左下から左上
    }

    [SerializeField]
    private GameObject m_lt, m_rt, m_rb, m_lb;

    [SerializeField]
    private float m_aniTime = 2.0f;

    [SerializeField]
    private aniId m_startId;
}
