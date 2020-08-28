using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRoot : MonoBehaviour
{
   public PlayerAnimator animator; // nothing done

    // Start is called before the first frame update
    void Start()
    {
        
    }


    public void CollisionDetected(Collision collision, PassCollisionToParent passCollisionToParent)
    {
       // print("Collision detected in " + passCollisionToParent.gameObject.name);
        //_controller.OnCollision(collision, passCollisionToParent);
    }

    public Vector3 vel;
    public float speedTol = 10;

    void Update()
    {
        vel = GetComponent<Rigidbody>().velocity;
        // if(vel.sqrMagnitude > speedTol)
        // {
        //     animator.currentAnimation = "walk";
        // }else
        // {
        //     animator.currentAnimation = "idle";
        // }
    }
}
