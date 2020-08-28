using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassCollisionToParent : MonoBehaviour
{
    public PlayerRoot parent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        parent.CollisionDetected(collision, this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
