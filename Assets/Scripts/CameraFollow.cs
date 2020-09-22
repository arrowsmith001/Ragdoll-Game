using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public bool fixHeight;
    public float fixedHeightOffset = 1;
    public float X_TILT = 10;
    public GameObject target;
    public float followDistance = 50;
    public float CAMERA_INPUT_MOVE_SPEED = 50;
    public float CAMERA_INPUT_STEP_SIZE = 10;
    public float CAMERA_MOVE_SPEED = 10;
    public PuppetBeam puppetBeam;

    // Start is called before the first frame update
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        // Camera forward
        Vector3 forward = transform.forward;
        Vector3 up = transform.up;
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        Vector3 right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;            
        
        Vector3 direction = new Vector3();
        if(Input.GetKey(KeyCode.LeftControl)) direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")); 

        Vector3 targetPos = new Vector3();

        // // Respond to input
        // if (direction != Vector3.zero)
        // {
        //     Vector3 rightMovement = Vector3.Normalize(right * Input.GetAxis("Horizontal"))* CAMERA_INPUT_MOVE_SPEED *Time.deltaTime;
        //     Vector3 upMovement = Vector3.Normalize(up * Input.GetAxis("Vertical")) * CAMERA_INPUT_MOVE_SPEED * Time.deltaTime;
        
        //     targetPos = transform.position + CAMERA_INPUT_STEP_SIZE * (rightMovement + upMovement).normalized;
        //     transform.position = Vector3.Slerp(transform.position, targetPos, CAMERA_INPUT_MOVE_SPEED * Time.deltaTime);
        // }


        // Away from player vector
        Vector3 towardTarget = target.transform.position - transform.position;
        Vector3 awayFromTarget = -towardTarget;
        
        // Rotation towards player
        Quaternion q = Quaternion.FromToRotation(transform.forward, towardTarget) * transform.rotation;
        q = Quaternion.Slerp(transform.rotation, q, CAMERA_MOVE_SPEED * Time.deltaTime);
        //q.x = X_TILT;
        q.z = 0; 
        transform.rotation = q;
        // transform.LookAt(target.transform);

        // Correct distance
        targetPos = target.transform.position + (awayFromTarget.normalized)*followDistance;
        Vector3 towardsTargetPosition = targetPos - transform.position;
        transform.position = Vector3.Slerp(transform.position, targetPos, CAMERA_MOVE_SPEED * Time.deltaTime);
    

        if(fixHeight)
        {
           Vector3 pos = transform.position;
           pos.y = puppetBeam.transform.position.y + fixedHeightOffset;
           transform.position = pos;
        }

    }
}
