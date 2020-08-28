using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.ObjectModel;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEngine.AI;
using UnityEngine.Animations;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class PlayerAnimator : MonoBehaviour
{
    public String currentAnimation = BOOL_IDLE;

    Animator anim;
    private const String BOOL_IDLE = "idle";
    private const String BOOL_WALK = "walk";
    private const String BOOL_RUN = "run";
    private const String BOOL_CURL = "curl";

    void Awake()
    {
        this.anim = this.GetComponent<Animator>();
        Animate(anim, "idle");
    }

       void Animate(Animator anim, String animation)
    {
        DisableOtherAnimations(anim, animation);
        anim.SetBool(animation, true);
    }

    void DisableOtherAnimations(Animator anim, String animation)
    {
        foreach (AnimatorControllerParameter param in anim.parameters)
        {
            if(param.name != animation)
            {
                anim.SetBool(param.name, false);
            }
        }
    }

    public float movementTol = 10;

    public void Curl(){
        Animate(anim,"curl");
    }

    public void Idle()
    {
        
        Animate(anim, "idle");
    }

    // Update is called once per frame
    void Update()
    {
    
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(h, 0, v);

        if(movement.magnitude > movementTol)
        {
            if(Input.GetKey(KeyCode.LeftShift))
            {
                Animate(anim,"run");
            }
            else
            {
                Animate(anim, "walk");
            }
        }
        else if(anim.GetBool("curl"))
        {
            // do nothing
        }
        else
        {
            Animate(anim, "idle");
        }
    }
}
