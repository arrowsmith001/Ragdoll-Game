using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetweenFeet : MonoBehaviour
{
    public Transform leftFoot;
    public Transform rightFoot;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 positionBetweenFeet = 
            leftFoot.position + ((rightFoot.position - leftFoot.position)/2);
        transform.position = positionBetweenFeet;
    }
}
