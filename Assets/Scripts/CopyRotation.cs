using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyRotation : MonoBehaviour
{
    public Transform toCopy;
    public float rotSpeed = 5;
    

    // Update is called once per frame
    void Update()
    {
        // Quaternion rot = transform.rotation;
        // transform.rotation 
        // = new Quaternion(rot.x, toCopy.rotation.y, rot.z, rot.w);

        Quaternion q = Quaternion.FromToRotation(transform.forward, toCopy.forward) * transform.rotation;
        q.x = transform.rotation.x;
        q.z = transform.rotation.z;
        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * rotSpeed);

    }
}
