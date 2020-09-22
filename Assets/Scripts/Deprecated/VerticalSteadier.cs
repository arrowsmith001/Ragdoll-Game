using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalSteadier : MonoBehaviour
{
    float tol = 0.01f;
    void Awake() {
        GetComponent<Rigidbody>().constraints = 
            RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
    }

    // Update is called once per frame
    void Update()
    {
        float mag = GetComponent<Rigidbody>().velocity.magnitude;
        if(mag < tol && Time.frameCount > 1)
        {
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX;
            this.GetComponent<VerticalSteadier>().enabled = false;
        }
    
    }
}
