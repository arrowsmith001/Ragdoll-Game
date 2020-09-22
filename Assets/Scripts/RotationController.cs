using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationController : MonoBehaviour
{
    public float rotSpeed = 3;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {           // Camera forward
        Vector3 forward = Camera.main.transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        Vector3 right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;

        // Input
        Vector3 direction = new Vector3();
        if(!Input.GetKey(KeyCode.LeftControl)) direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (direction != Vector3.zero)
        {
            Vector3 rightMovement = Vector3.Normalize(right * Input.GetAxis("Horizontal"));
            Vector3 upMovement = Vector3.Normalize(forward * Input.GetAxis("Vertical"));
        
            // Look rotation
            Vector3 heading = Vector3.Normalize(rightMovement + upMovement);
            if (heading != Vector3.zero)
            {
                Quaternion q = Quaternion.LookRotation(heading);
                Quaternion newRot = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(heading), Time.deltaTime * rotSpeed);
                // newRot.x = 0;
                // newRot.z = 0;
                transform.rotation = newRot;
            }
        }
    }
}
