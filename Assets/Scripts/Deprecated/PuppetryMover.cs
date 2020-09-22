using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuppetryMover : MonoBehaviour
{
    public GameObject paths;
    public float speed = 1;
    public float rotSpeed = 3;

    public Drag footLDrag;
    public Drag footRDrag;
    public Animator IKAnim;

    public List<GameObject> joints;
    public Rigidbody lowerTarget;
    public Rigidbody upperTarget;
    public Transform lowerCube;
    public Transform upperCube;

    Dictionary<String, Transform> pathDic = new Dictionary<string, Transform>();

    void Awake()
    {
        try{
            for(int i = 0; i < paths.transform.childCount; i++){

                GameObject go = paths.transform.GetChild(i).gameObject;
                pathDic.Add(go.name, go.transform);
             }
        }catch{}
    }

    private void Start() {
        
       // StartCoroutine(FootCycle("Right"));
    }

    bool movementEnabled = true;
    bool gettingUp = false;

    // Update is called once per frame
    void Update()
    {
        if(movementEnabled) Move();

        if(!gettingUp) AssessJoints();

    }

    void AssessJoints(){
        bool hingesAttached = true;
        foreach(GameObject go in joints){
            if(go.GetComponent<Joint>() == null){
                hingesAttached = false;
            }
        }

        if(!hingesAttached){
            foreach(GameObject go in joints){
                Destroy(go.GetComponent<Joint>());
            }

            StartCoroutine(GetUp());
        }
    }

    float unbreakableBreakforce = 100000000;
    public float normalBreakforce = 3000;
    public float springStrength = 300;
    public float getUpRotateSpeed = 0.1f;
    public float getUpStandSpeed = 0.1f;

    IEnumerator GetUp(){
        movementEnabled = false;
        gettingUp = true;

        while(lowerTarget.velocity.magnitude > 0.1){

             transform.position = lowerTarget.gameObject.transform.position;
            transform.rotation = Quaternion.LookRotation(lowerTarget.gameObject.transform.forward,
            upperTarget.gameObject.transform.position - lowerTarget.gameObject.transform.position);

            yield return new WaitForEndOfFrame();
        }

        // Attach unbreakable joints
        HingeJoint hJoint = lowerCube.gameObject.AddComponent<HingeJoint>();
        hJoint.connectedBody = lowerTarget;
        hJoint.breakForce = unbreakableBreakforce;

        SpringJoint sJoint = upperCube.gameObject.AddComponent<SpringJoint>();
        sJoint.connectedBody = upperTarget;
        sJoint.spring = springStrength;
        sJoint.breakForce = unbreakableBreakforce;

        Vector3 fwd = transform.forward;
        fwd.y = 0;
        fwd.Normalize();
        Quaternion targetRotUpright = Quaternion.LookRotation(fwd, Vector3.up);
        Quaternion targetRotDown = Quaternion.LookRotation(Vector3.down + fwd);

        // Upright
        while(Vector3.Distance(transform.rotation.eulerAngles, targetRotUpright.eulerAngles) > 0.1){
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotUpright, Time.deltaTime * getUpRotateSpeed);
            yield return new WaitForEndOfFrame();
        }

        // Down
        while(Vector3.Distance(transform.rotation.eulerAngles, targetRotDown.eulerAngles) > 0.1){
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotDown, Time.deltaTime * getUpRotateSpeed * 2);
            yield return new WaitForEndOfFrame();
        }

        // Upright and up
        while(Mathf.Abs(transform.position.y - height) > 0.1 
            || Vector3.Distance(transform.rotation.eulerAngles, targetRotUpright.eulerAngles) > 0.1){
            
            Vector3 targetPos = transform.position;
            targetPos.y = height;
            transform.position = Vector3.Slerp(transform.position, targetPos, Time.deltaTime * getUpStandSpeed); 

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotUpright, Time.deltaTime * getUpRotateSpeed);

            yield return new WaitForEndOfFrame();
        }

        hJoint.breakForce = normalBreakforce;
        sJoint.breakForce = normalBreakforce;

        movementEnabled = true;
        gettingUp = false;
    }

    public float walkSpeed = 5;
    public float runSpeed = 5;
    float GetMovementFactorFromWalkAnim()
    {
        AnimatorStateInfo animationState = IKAnim.GetCurrentAnimatorStateInfo(0);
        AnimatorClipInfo[] myAnimatorClip = IKAnim.GetCurrentAnimatorClipInfo(0);
        float t = myAnimatorClip[0].clip.length * animationState.normalizedTime;
        t = (t % 2) / 2; // 0 <= t < 1
        //transform.LookAt(movement);
        float pi = Mathf.PI;
        float m = Mathf.Abs(Mathf.Sin(2*pi*t));
        return m;
    }

    public float bobFactor = 2;
    public float height = 1.8f;
    public float lurch = 0.7f;

    void Move(){
           // Camera forward
        Vector3 forward = Camera.main.transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        Vector3 right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;

        // Walk cycle
        float m = GetMovementFactorFromWalkAnim();
        print(m);
        
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

            // Base movement
            transform.position += heading * speed 
                * ((1 - lurch) + lurch*m) 
                * (Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed) * Time.deltaTime;

            /// Height adjustment
            Vector3 pos = transform.position;
            pos.y = height + bobFactor* Mathf.Pow(m, 2);
            transform.position = pos;
        }
    }
}
