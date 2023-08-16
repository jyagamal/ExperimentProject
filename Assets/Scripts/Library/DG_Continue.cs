using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_Continue : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.transform.DOScale(m_maxScale, m_aniTime).SetLoops(-1, LoopType.Yoyo);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [SerializeField]
    private float m_aniTime = 1.0f;

    [SerializeField]
    private Vector3 m_maxScale;
}
