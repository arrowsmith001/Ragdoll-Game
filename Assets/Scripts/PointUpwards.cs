using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointUpwards : MonoBehaviour
{
    public Transform forwardTransform;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(forwardTransform.forward, Vector3.up);
    }
}
