using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Target : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //���N���b�N���ꂽ��
        if(Input.GetMouseButtonDown(0))
        {
            GameObject clickGameObject = null;

            //ray�����
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();

            //���������ɓ���������
            if (Physics.Raycast(ray, out hit))
            {
                GameObject hitObject = hit.collider.gameObject;

                //���������I�u�W�F�N�g��\������
                Debug.Log(hitObject.transform.name);

                //�u���b�N�Ȃ炱��ȏ㏈�����Ȃ�
                if (hitObject.transform.CompareTag("Block"))
                    return;

                //�N���b�N�n�_�Ɏ��g�̍��W���ړ�����
                this.transform.position = new Vector3
                    (
                        hit.point.x,
                        this.transform.position.y,
                        hit.point.z
                    );
            }
        }
    }
}
