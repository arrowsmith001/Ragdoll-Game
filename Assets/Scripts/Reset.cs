using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reset : MonoBehaviour
{
    public bool reset = false;
    Vector3 startPos;
    Quaternion startRot;

    // Start is called before the first frame update
    void Awake()
    {
        startPos = transform.position;
        startRot = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if(reset)
        {
            transform.position = startPos;
            transform.rotation = startRot;
            GetComponent<Rigidbody>().isKinematic = true;
            
            reset = false;
        }
    }
}
