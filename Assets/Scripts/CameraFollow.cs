using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject target;
    public float followDistance = 50;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 towardTarget = target.transform.position - transform.position;
        Vector3 awayFromTarget = -towardTarget;

        // Rotation
        Quaternion q = Quaternion.FromToRotation(transform.forward, towardTarget) * transform.rotation;
        q = Quaternion.Slerp(transform.rotation, q, Time.deltaTime);
        transform.rotation = q;

        // Position
        Vector3 targetPos = target.transform.position + (awayFromTarget.normalized)*followDistance;
        Vector3 towardsTargetPosition = targetPos - transform.position;
        transform.position = Vector3.Slerp(transform.position, targetPos, Time.deltaTime);

        // Fix tilt // TODO This doesn't really work.
        q = transform.rotation;
        q.z = 0; // TODO Add drunk camera effect
        transform.rotation = q;


    }
}
