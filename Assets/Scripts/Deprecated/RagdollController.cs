using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    CharacterController _controller;

    Dictionary<Transform, Vector3> startPositions = new Dictionary<Transform, Vector3>();

    public GameObject armature;

    void Awake()
    {
        _controller = GetComponent<CharacterController>();

        
    }


    // Update is called once per frame
    void Update()
    {

        Vector3 moveVec = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        _controller.Move(moveVec);
    }
}
