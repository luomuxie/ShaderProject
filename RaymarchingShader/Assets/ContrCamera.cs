using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContrCamera : MonoBehaviour
{
    // Start is called before the first frame update
    private Transform trans;
    public float speed = 2;
    void Start()
    {
        trans = this.gameObject.transform;
        trans.position = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = trans.position;
        
        if (pos.x < 32)
        {
            pos.x += Time.deltaTime*speed;
        }
        else
        {
            pos.z += Time.deltaTime*speed;

        }
       trans.position = pos;  

    }
}
