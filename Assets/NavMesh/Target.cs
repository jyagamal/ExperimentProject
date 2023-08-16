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
        //左クリックされたら
        if(Input.GetMouseButtonDown(0))
        {
            GameObject clickGameObject = null;

            //rayを作る
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();

            //もし何かに当たったら
            if (Physics.Raycast(ray, out hit))
            {
                GameObject hitObject = hit.collider.gameObject;

                //当たったオブジェクトを表示する
                Debug.Log(hitObject.transform.name);

                //ブロックならこれ以上処理しない
                if (hitObject.transform.CompareTag("Block"))
                    return;

                //クリック地点に自身の座標を移動する
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
