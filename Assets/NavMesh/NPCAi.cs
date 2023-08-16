using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCAi : MonoBehaviour
{
    [SerializeField]
    private GameObject m_targetObject;

    NavMeshAgent m_navMeshAgent;

    [SerializeField]
    private KeyCode m_stopKeyCode, m_restartKeyCode;

    // Start is called before the first frame update
    void Start()
    {
        m_navMeshAgent = this.GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        //�i�r���b�V���������o���Ă���Ȃ�
        if(m_navMeshAgent.pathStatus != NavMeshPathStatus.PathInvalid)
        {
            //�ړI�n��ݒ肷��(�d�����ߏo���邾��Update�ɓ���Ȃ�����)
            m_navMeshAgent.SetDestination(m_targetObject.transform.position);
        }

        if (Input.GetKeyDown(m_stopKeyCode))
            m_navMeshAgent.isStopped = true;

        if (Input.GetKeyDown(m_restartKeyCode))
            m_navMeshAgent.isStopped = false;
    }
}
