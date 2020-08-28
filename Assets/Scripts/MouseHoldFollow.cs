using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MouseHoldFollow : MonoBehaviour
{
    CharacterController _controller;
    void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            Vector3 dir = Input.mousePosition - transform.position;
            _controller.Move(dir);
    
        }
    }
}
