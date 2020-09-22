using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puppetry3 : MonoBehaviour
{
    public Animator IKAnim;
    public float rotSpeed;
    public float speed;
    public float lurch;
    public float runSpeed;
    public float walkSpeed;
    public float height;
    public float bobFactor;

    // Start is called before the first frame update
    void Start()
    {
        
    }
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

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    
}
