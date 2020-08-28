using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuppetBeam : MonoBehaviour
{
    public bool attachSprings;
    public bool detachSprings;
    public bool moveBeamsToPosition;
    public int springStrength = 5000;
    public int weakBreakForce = 1000;
    private int strongBreakForce = 10000000;
    public GameObject beamLow;
    public GameObject beamHigh;
    public Rigidbody lowSpringTarget;
    public Rigidbody highSpringTarget;
    public Transform feetBetween;
    public int fallDelay = 3;

public Animator anim;
public PlayerAnimator playerAnim;
public float walkSpeed = 0.05f;
public float runSpeed = 2;
public float rotSpeed = 5;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void DetachSprings()
    {
        Destroy(beamHigh.GetComponent<SpringJoint>());
        Destroy(beamLow.GetComponent<SpringJoint>());
    }

    private void AttachSprings(bool strong)
    {
        // Attach springs
        SpringJoint highJoint = beamHigh.GetComponent<SpringJoint>();
        SpringJoint lowJoint = beamLow.GetComponent<SpringJoint>();

        if(highJoint != null) Destroy(highJoint);
        if(lowJoint != null) Destroy(lowJoint);
        
        highJoint = beamHigh.AddComponent<SpringJoint>();
        ConfigureSpring(highJoint, highSpringTarget, springStrength, strong ? strongBreakForce : weakBreakForce);

        lowJoint = beamLow.AddComponent<SpringJoint>();
        ConfigureSpring(lowJoint, lowSpringTarget, springStrength, strong ? strongBreakForce : weakBreakForce);
    }

    private void ConfigureSpring(SpringJoint joint, Rigidbody target, int springStrength, int breakForce)
    {
        joint.spring = springStrength;
        joint.connectedBody = target;
        joint.breakForce = breakForce;
    }

    private IEnumerator OnSpringBreak(int delay)
    {
        yield return new WaitForSeconds(delay);

        fixHeight = false;

        // Puppetry will now follow spine1
        Vector3 thisPos = transform.position;
        Vector3 lowerTargetPos = lowSpringTarget.gameObject.transform.position;
        transform.position = lowerTargetPos;

        // Rotate by direction vector from spine1 to spine3
        Vector3 higherTargetPos = highSpringTarget.gameObject.transform.position;
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

    public float beamReattachMovementSpeed = 2;
    public float standMovementSpeed = 0.2f;
    private IEnumerator MoveBeamsToPosition()
    {
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

            while(posMag > 1)
            {
                Vector3 pos = transform.position;
                pos += (feetBetweenPos - pos) * Time.deltaTime * standMovementSpeed;
                transform.position = pos;

                posMag = (feetBetweenPos - transform.position).magnitude;
            yield return new WaitForEndOfFrame();
            }
        //}


            DetachSprings();
            AttachSprings(false);

            fixHeight = true;
            caringAboutJoints = true;
    }

    float GetMovementFactorFromWalkAnim()
    {
        AnimatorStateInfo animationState = anim.GetCurrentAnimatorStateInfo(0);
        AnimatorClipInfo[] myAnimatorClip = anim.GetCurrentAnimatorClipInfo(0);
        float t = myAnimatorClip[0].clip.length * animationState.normalizedTime;
        t = (t % 2) / 2; // 0 <= t < 1
        //transform.LookAt(movement);
        float pi = Mathf.PI;
        float m = Mathf.Abs(Mathf.Sin(2*pi*t));
        return m;
    }
    float GetMovementFactorFromRunAnim()
    {
        AnimatorStateInfo animationState = anim.GetCurrentAnimatorStateInfo(0);
        AnimatorClipInfo[] myAnimatorClip = anim.GetCurrentAnimatorClipInfo(0);
        float t = myAnimatorClip[0].clip.length * animationState.normalizedTime;
        t = (t % 1) / 1; // 0 <= t < 1
        //transform.LookAt(movement);
        float pi = Mathf.PI;
        float m = Mathf.Abs(Mathf.Sin(2*pi*t));
        return m;
    }

    public Rigidbody playerRigidbody;
    public float diveForce = 20;

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
                    playerRigidbody.AddForce(heading*diveForce, ForceMode.Impulse);

                }
                else
                {
                    // Movement
                    bool isRunning = Input.GetKey(KeyCode.LeftShift);
                    float m = isRunning ? GetMovementFactorFromRunAnim() : GetMovementFactorFromWalkAnim();
                    transform.position += heading * (isRunning ? runSpeed : walkSpeed) * Time.deltaTime * m;
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
            StartCoroutine(OnSpringBreak(fallDelay));

            caringAboutJoints = false;
        }

        if(fixHeight)
        {
            Vector3 pos = transform.position;
            pos.y = beamHeight;
            transform.position = pos;
        }
    }

    bool caringAboutJoints = true;

    public Vector3 spinalRotationOffset = new Vector3(0.5f, 0 ,-0.5f);
    public float beamHeight = 2.25f;
    public bool fixHeight = true;
}
