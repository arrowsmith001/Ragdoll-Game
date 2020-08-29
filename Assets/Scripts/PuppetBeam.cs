using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuppetBeam : MonoBehaviour
{ 
    /// <summary>
    /// Strength of spring joints that will attach to player. 
    /// </summary>
    public int springStrength = 5000;

    /// <summary>
    /// Break force of spring joints once they are fully attached.
    /// </summary>
    public int weakBreakForce = 1000;

    /// <summary>
    /// Break force of spring joints when re-positioning and therefore must be virtually unbreakable.
    /// </summary>
    private int strongBreakForce = 10000000;

    /// <summary>
    /// The part of the puppetry attached to the player's waist.
    /// </summary>
    public GameObject beamLow;

    /// <summary>
    /// The part of the puppetry attached to the player's head.
    /// </summary>
    public GameObject beamHigh;

    /// <summary>
    /// The player's waist's rigibody.
    /// </summary>
    public Rigidbody playerLowerRB;

    /// <summary>
    /// The player's head's rigibody.
    /// </summary>
    public Rigidbody playerUpperRB;

    /// <summary>
    /// Indicator transform that is positioned directly between the player's feet.
    /// </summary>
    public Transform feetBetween;

    /// <summary>
    /// Number of seconds between spring joint detachment and re-attachment routine.
    /// </summary>
    public int postFallDelay = 3;

    /// <summary>
    /// Reference to PlayerAnimator script found on IK.
    /// </summary>
    public PlayerAnimator playerAnim;

    /// <summary>
    /// Base walk speed multipler.
    /// </summary>
    public float walkSpeed = 0.05f;

    /// <summary>
    /// Base run speed multipler.
    /// </summary>
    public float runSpeed = 2;

    /// <summary>
    /// Base speed at which beam rotates in direction of movement axis.
    /// </summary>
    public float rotSpeed = 5;

    /// <summary>
    /// If true, will enter joint-break routine if joints are disturbed. If false, the beam is most likely reattaching joints.
    /// </summary>
    bool caringAboutJoints = true;

    /// <summary>
    /// The base height from the ground the beam should be elevated at.
    /// </summary>
    public float beamHeight = 2.25f;

    /// <summary>
    /// If true, the beam will elevate itself at beamHeight. If false, the beam is most likely reattaching joints.
    /// </summary>
    public bool fixHeight = true;

    /// <summary>
    /// Force at which player can dive forward.
    /// </summary>
    public float diveForce = 20;

    /// <summary>
    /// Speed at which the beam rotates to set the player upright after reattaching joints.
    /// </summary>
    public float beamReattachMovementSpeed = 2;

    /// <summary>
    /// Speed at which the beam returns to beamHeight after reattaching joints.
    /// </summary>
    public float standMovementSpeed = 0.2f;

    /// <summary>
    /// If true, beam will move in direction of movement axis.
    /// </summary>
    private bool movementEnabled = true;

    private void DetachSprings()
    {
        Destroy(beamHigh.GetComponent<SpringJoint>());
        Destroy(beamLow.GetComponent<SpringJoint>());
    }
    private void AttachSprings(bool makeStrong)
    {
        // Attach springs
        SpringJoint highJoint = beamHigh.GetComponent<SpringJoint>();
        SpringJoint lowJoint = beamLow.GetComponent<SpringJoint>();

        if(highJoint != null) Destroy(highJoint);
        if(lowJoint != null) Destroy(lowJoint);
        
        highJoint = beamHigh.AddComponent<SpringJoint>();
        ConfigureSpring(highJoint, playerUpperRB, springStrength, makeStrong ? strongBreakForce : weakBreakForce);

        lowJoint = beamLow.AddComponent<SpringJoint>();
        ConfigureSpring(lowJoint, playerLowerRB, springStrength, makeStrong ? strongBreakForce : weakBreakForce);
    }
    private void ConfigureSpring(SpringJoint joint, Rigidbody target, int springStrength, int breakForce)
    {
        joint.spring = springStrength;
        joint.connectedBody = target;
        joint.breakForce = breakForce;
    }
    private IEnumerator OnSpringBreak(int delay)
    {
        DisableMovement();
        DisableCopyRotation();

        yield return new WaitForSeconds(delay);

        fixHeight = false;

        // Puppetry will now follow spine1
        Vector3 thisPos = transform.position;
        Vector3 lowerTargetPos = playerLowerRB.gameObject.transform.position;
        transform.position = lowerTargetPos;

        // Rotate by direction vector from spine1 to spine3
        Vector3 higherTargetPos = playerUpperRB.gameObject.transform.position;
        Vector3 lowerToUpper = higherTargetPos - lowerTargetPos;
        // Quaternion rot = Quaternion.LookRotation(lowerToUpper);
        // rot.x += spinalRotationOffset.x;
        // rot.y += spinalRotationOffset.y;
        // rot.z += spinalRotationOffset.z;
        // transform.rotation = rot;
        
        Quaternion q = Quaternion.FromToRotation(transform.up, lowerToUpper) * transform.rotation;
        transform.rotation = q;

        // Attach STRONG springs
        AttachSprings(true);
        
        // Move beam back to standing position
        StartCoroutine(MoveBeamsToPosition());
    }
    private IEnumerator MoveBeamsToPosition()
    {
        EnableCopyRotation();

        float rotMag = Vector3.Angle(transform.rotation.eulerAngles, Vector3.up);
        //Vector3 startRot = transform.rotation.eulerAngles;
        //Vector3 targetRot = transform.up;

        playerAnim.Curl();
        // while(rotMag > 0.1 || posMag > 0.1)
        // {
        while(rotMag > 0.1)
        {
            Quaternion q = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;
            rotMag = Quaternion.Angle(transform.rotation, q);
            transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * beamReattachMovementSpeed);
            yield return new WaitForEndOfFrame();
        }

        Vector3 feetBetweenPos = feetBetween.position;
        feetBetweenPos.y = beamHeight;
        float posMag = (feetBetweenPos - transform.position).magnitude;

         playerAnim.Idle();

            while(posMag > 0.1)
            {
                Vector3 pos = transform.position;
                //pos += (feetBetweenPos - pos) * Time.deltaTime * standMovementSpeed;
                //transform.position = pos;
                transform.position = Vector3.Slerp(transform.position, feetBetweenPos, Time.deltaTime  * standMovementSpeed);

                posMag = (feetBetweenPos - transform.position).magnitude;
                yield return new WaitForEndOfFrame();
            }
        //}


            DetachSprings();
            AttachSprings(false);

            fixHeight = true;
            caringAboutJoints = true;

            EnableMovement();
    }
    float GetMovementFactorFromWalkAnim()
    {
        AnimatorStateInfo animationState = playerAnim.anim.GetCurrentAnimatorStateInfo(0);
        AnimatorClipInfo[] myAnimatorClip = playerAnim.anim.GetCurrentAnimatorClipInfo(0);
        float t = myAnimatorClip[0].clip.length * animationState.normalizedTime;
        t = (t % 2) / 2; // 0 <= t < 1
        //transform.LookAt(movement);
        float pi = Mathf.PI;
        float m = Mathf.Abs(Mathf.Sin(2*pi*t));
        return m;
    }
    float GetMovementFactorFromRunAnim()
    {
        // AnimatorStateInfo animationState = anim.GetCurrentAnimatorStateInfo(0);
        // AnimatorClipInfo[] myAnimatorClip = anim.GetCurrentAnimatorClipInfo(0);
        // float t = myAnimatorClip[0].clip.length * animationState.normalizedTime;
        // t = (t % 1) / 1; // 0 <= t < 1
        // float pi = Mathf.PI;
        // float m = Mathf.Abs(Mathf.Sin(2*pi*t));
        // return m;
        
        return 1;
    }

    public void DisableMovement(){movementEnabled = false;}
    public void EnableMovement(){ movementEnabled = true;}
    public void DisableCopyRotation(){
        playerLowerRB.gameObject.GetComponent<CopyRotation>().enabled = false;}
    public void EnableCopyRotation(){
        playerLowerRB.gameObject.GetComponent<CopyRotation>().enabled = true;}

    

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey)
        {
            // Camera forward
            Vector3 forward = Camera.main.transform.forward;
            forward.y = 0;
            forward = Vector3.Normalize(forward);
            Vector3 right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;
            
            // Input
            Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")); 

            if (direction != Vector3.zero)
            {
                Vector3 rightMovement = Vector3.Normalize(right * Input.GetAxis("Horizontal"))*walkSpeed*Time.deltaTime;
                Vector3 upMovement = Vector3.Normalize(forward * Input.GetAxis("Vertical")) * walkSpeed * Time.deltaTime;
            
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

                if(Input.GetKeyDown(KeyCode.Space))
                {
                    print("Space pressed");
                    // Dive
                    DetachSprings();
                    //playerRigidbody.AddExplosionForce(diveForce);
                    playerUpperRB.AddForce(heading*diveForce, ForceMode.Impulse);

                }
                else
                {
                    // Movement
                    bool isRunning = Input.GetKey(KeyCode.LeftShift);
                    //bool isRunning = false;
                    float m = isRunning ? GetMovementFactorFromRunAnim() : GetMovementFactorFromWalkAnim();

                    if(movementEnabled) transform.position += heading * (isRunning ? runSpeed : walkSpeed) * Time.deltaTime * m;
                   // print("MOVEMENT MAG: "+(heading * (isRunning ? runSpeed : walkSpeed) * Time.deltaTime * m).magnitude);
                }

       
            }

        }

        // Check if any springs are detached from model
        bool springsAttached = 
            beamHigh.GetComponent<SpringJoint>() != null
            && beamLow.GetComponent<SpringJoint>() != null;

        print(springsAttached);
        if(caringAboutJoints && !springsAttached)
        {
            StopCoroutine("OnSpringBreak");
            StopCoroutine("MoveBeamsToPosition");

            // Destroy all connections just to be safe
            DetachSprings();
            StartCoroutine(OnSpringBreak(postFallDelay));

            caringAboutJoints = false;
        }

        if(fixHeight)
        {
            Vector3 pos = transform.position;
            pos.y = beamHeight;
            transform.position = pos;
        }
    }

}
