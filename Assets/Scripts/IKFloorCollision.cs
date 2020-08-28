using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKFloorCollision : MonoBehaviour
{
    public enum side {L, R, neither};
    public side leftOrRight;
    public Roller roller;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {

        if(collision.collider.tag == "IK Floor")
        {
        //print("OK");
            roller.FootEnterFloor(leftOrRight, collision);
        }
    }
    void OnCollisionExit(Collision collision)
    {

        if(collision.collider.tag == "IK Floor")
        {
        //print("OK");
            roller.FootExitFloor(leftOrRight, collision);
        
        }
    }  

    
    // void OnTriggerEnter(Collider collider)
    // {

    //     if(collider.tag == "IK Floor")
    //     {
    //     print("OK");
    //         roller.FootEnterFloor(leftOrRight, new Collision());
    //     }
    // }
    // void OnTriggerExit(Collider collider)
    // {

    //     if(collider.tag == "IK Floor")
    //     {
    //     print("OK");
    //         roller.FootExitFloor(leftOrRight, new Collision());
        
    //     }
    // }   

    // void OnCollisionStay(Collision collision)
    // {
    //     if(collision.collider.tag == "IK Floor")
    //     {
    //        // print("OK");
    //         roller.FootStayingInFloor(leftOrRight, new Collision());
    //     }
    // }
    
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
