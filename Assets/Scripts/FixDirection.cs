using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixDirection : MonoBehaviour
{
    public bool upFixed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;

        if(upFixed)
        {
           //transform.LookAt(transform, transform.forward);
            transform.rotation.SetLookRotation(Vector3.forward);
        }
    }
}
