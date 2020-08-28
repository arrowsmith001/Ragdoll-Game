using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool track = true;

    public float standingHeight = 2;

    public float playerRootFollowSpeed = 0.5f;
    public float controllerMoveSpeed = 1f;

    public PlayerRoot playerRoot;


    void Awake()
    {
        //playerRoot.SetController(this);
    }


    void OnCollisionEnter(Collision collision)
    {

    }

    IEnumerator Ragdoll(int t)
    {
        ToggleKinematics(false);
        track = false;

        yield return new WaitForSeconds(t);

        ToggleKinematics(true);
        track = true;
    }

    internal void OnCollision(Collision collision, PassCollisionToParent passCollisionToParent)
    {
        if(collision.collider.tag != "Floor") StartCoroutine(Ragdoll(3));
    }


    // Update is called once per frame
    void Update()
    {
        // Keep controller at fixed height
        transform.position = new Vector3(transform.position.x, standingHeight, transform.position.z);

        // Respond to player input
        Vector3 moveVec = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        transform.position += moveVec * controllerMoveSpeed * Time.deltaTime;

        if(track)
        {
            // Vector from controller to player root
            Vector3 vecTowards = transform.position - playerRoot.gameObject.transform.position;

            // Move/turn player towards controller
             playerRoot.gameObject.transform.position += vecTowards * playerRootFollowSpeed * Time.deltaTime;
            if(vecTowards != Vector3.zero) 
            {
                Quaternion rotVec = Quaternion.LookRotation(vecTowards);
                 playerRoot.gameObject.transform.rotation = new Quaternion(0, rotVec.y, 0, rotVec.w);
            }

        }


    }

    private void ToggleKinematics(bool on)
    {
            playerRoot.GetComponent<Rigidbody>().isKinematic = on;
    }
}
