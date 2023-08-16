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
        //ナビメッシュが準備出来ているなら
        if(m_navMeshAgent.pathStatus != NavMeshPathStatus.PathInvalid)
        {
            //目的地を設定する(重いため出来るだけUpdateに入れないこと)
            m_navMeshAgent.SetDestination(m_targetObject.transform.position);
        }

        if (Input.GetKeyDown(m_stopKeyCode))
            m_navMeshAgent.isStopped = true;

        if (Input.GetKeyDown(m_restartKeyCode))
            m_navMeshAgent.isStopped = false;
    }
}
