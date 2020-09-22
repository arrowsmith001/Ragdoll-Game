using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PuppetBeam : MonoBehaviour
{ 
    public const float IDEAL_BEAM_HEIGHT = 1.85f;
    public const float IDEAL_WALK_SPEED = 2.5f;
    
    /// <summary>
    /// Strength of spring joints that will attach to player. 
    /// </summary>
    public int springStrength = 1000;

    /// <summary>
    /// Break force of spring joints once they are fully attached.
    /// </summary>
    public int weakBreakForce = 1000;

    /// <summary>
    /// Break force of spring joints when re-positioning and therefore must be virtually unbreakable.
    /// </summary>
    private int strongBreakForce = 1000000;

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
    public float beamHeight = 1.85f;

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
    private bool rotationEnabled = true;

    /// <summary>
    /// Cut off point for tilting towards feetBetween position.
    /// </summary>
    public float TILT_TOL = 5;

    private void Awake() {
        // beamLow.GetComponent<SpringJoint>().breakForce = weakBreakForce;
        // beamHigh.GetComponent<SpringJoint>().breakForce = weakBreakForce;

        StartCoroutine(Initialise());
    }


    IEnumerator Initialise(){

        DisableMovement();

        DetachSprings();
        AttachSprings(true);

        while(Time.frameCount < 10)
        {
            yield return new WaitForEndOfFrame();
        }

       // DetachSprings();
        WeakenSprings();
        EnableMovement();
    }

    private void DetachSprings()
    {
        // Destroy(beamHigh.GetComponent<SpringJoint>());
        // Destroy(beamLow.GetComponent<SpringJoint>());

        foreach(GameObject beam in OrbitingBeams)
        {
            Destroy(beam.GetComponent<SpringJoint>());
        }
    }
    private void AttachSprings(bool makeStrong)
    {
        print("CALLED: " + makeStrong);

        // // Attach springs
        // SpringJoint highJoint = beamHigh.GetComponent<SpringJoint>();
        // SpringJoint lowJoint = beamLow.GetComponent<SpringJoint>();

        foreach(GameObject beam in OrbitingBeams)
        {
            SpringJoint joint = beam.GetComponent<SpringJoint>();
            if(joint != null) Destroy(joint);

            joint = beam.AddComponent<SpringJoint>();
            ConfigureSpring(joint, playerLowerRB, springStrength, makeStrong ? strongBreakForce : weakBreakForce);
            
        }

        // if(highJoint != null) Destroy(highJoint);
        // if(lowJoint != null) Destroy(lowJoint);
        
        // highJoint = beamHigh.AddComponent<SpringJoint>();
        // ConfigureSpring(highJoint, playerUpperRB, springStrength, makeStrong ? strongBreakForce : weakBreakForce);

        // lowJoint = beamLow.AddComponent<SpringJoint>();
        // ConfigureSpring(lowJoint, playerLowerRB, springStrength, makeStrong ? strongBreakForce : weakBreakForce);
    }
    
    private void WeakenSprings()
    {
        foreach(GameObject beam in OrbitingBeams)
        {
            SpringJoint joint = beam.GetComponent<SpringJoint>();
            joint.breakForce = weakBreakForce;

            print("WEAKENED: " + joint.breakForce);
            
        }
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
        DisableRotation();

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

        // // Attach prop joint
        SpringJoint propJoint = prop.AddComponent<SpringJoint>();
        propJoint.spring = 5000;
        propJoint.breakForce = strongBreakForce;
        propJoint.connectedBody = playerUpperRB;
    
        
        // Move beam back to standing position
        StartCoroutine(MoveBeamsToPosition());
    }

    private void DisableRotation()
    {
        rotationEnabled = false;
    }
    private void EnableRotation()
    {
        rotationEnabled = true;
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

            Destroy(prop.GetComponent<SpringJoint>());

            fixHeight = true;
            caringAboutJoints = true;

            EnableMovement();
            EnableRotation();
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

    
    public GameObject[] OrbitingBeams;


    // Update is called once per frame
    void Update()
    {
        // Camera forward
        Vector3 forward = Camera.main.transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        Vector3 right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;
        
        // Input
        Vector3 direction = new Vector3();
        if(!Input.GetKey(KeyCode.LeftControl)) direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")); 

        if (direction != Vector3.zero)
        {
            Vector3 rightMovement = Vector3.Normalize(right * Input.GetAxis("Horizontal"))*walkSpeed*Time.deltaTime;
            Vector3 upMovement = Vector3.Normalize(forward * Input.GetAxis("Vertical")) * walkSpeed * Time.deltaTime;
        
            // Look rotation
            Vector3 heading = Vector3.Normalize(rightMovement + upMovement);
            if (heading != Vector3.zero && rotationEnabled)
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
                playerUpperRB.AddForce(heading * diveForce * Mathf.Pow(playerLowerRB.velocity.magnitude, 2), ForceMode.Impulse);
                print("playerMag: "+playerLowerRB.velocity.magnitude);

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
        else
        {
            // Center between feet
            if(movementEnabled)
            {
                Vector3 feetBetweenPos = feetBetween.position;
                feetBetweenPos.y = beamHeight;
                float mag = (transform.position - feetBetweenPos).magnitude;
//                print("mag "+mag);
                if(mag > TILT_TOL) transform.position = Vector3.Slerp(transform.position, feetBetweenPos, Time.deltaTime  * standMovementSpeed);
            }
        }

        

        // Check if any springs are detached from model
        // bool springsAttached = 
        //     beamHigh.GetComponent<SpringJoint>() != null
        //     && beamLow.GetComponent<SpringJoint>() != null;

        bool springsAttached = true;
        foreach(GameObject beam in OrbitingBeams)
        {
            if(beam.GetComponent<SpringJoint>() == null) {
                springsAttached = false;
                continue;
            }
        }

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

        CalculateDistanceVariance();

    }

    private void CalculateDistanceVariance()
    {
        List<double> list = new List<double>();

        foreach(GameObject beam in OrbitingBeams) { 
                double mag = (beam.transform.position - playerLowerRB.position).magnitude; 
                list.Add(mag);
            }

        distVar = Math.Pow(CalculateStandardDeviation(list), 2);
    }
    private double CalculateStandardDeviation(IEnumerable<double> values)
    {   
        double standardDeviation = 0;

        if (values.Any()) 
        {      
            // Compute the average.     
            double avg = values.Average();

            // Perform the Sum of (value-avg)_2_2.      
            double sum = values.Sum(d => Math.Pow(d - avg, 2));

            // Put it all together.      
            standardDeviation = Math.Sqrt((sum) / (values.Count()-1));   
        }  

        return standardDeviation;
    }

    public GameObject prop;

    public double distVar;
}
