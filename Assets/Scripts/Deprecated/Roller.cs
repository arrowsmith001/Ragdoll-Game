using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roller : MonoBehaviour
{
    public bool printMessages = false;
    public Transform playerRoot;
    public Transform hipsLeftTrans;
    public Transform hipsRightTrans;

    public Transform leftFoot;
    public Transform rightFoot;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public IKFloorCollision.side actingFoot;
    public bool leftFootIn;
    public bool rightFootIn;

    public float fixedHeight = 6;

    public void FootEnterFloor(IKFloorCollision.side side, Collision collider)
    {
        switch(side)
        {
            case IKFloorCollision.side.L: 
            print(DateTime.Now + ": L enter");
                leftFootIn = true;
            break;
            case IKFloorCollision.side.R: 
            print(DateTime.Now + ": R enter");
                rightFootIn = true;
            break;
        }
       
        
    }
    public void FootExitFloor(IKFloorCollision.side side, Collision collider)
    {
        switch(side)
        {
            case IKFloorCollision.side.L: 
            print(DateTime.Now + ": L exit");
                leftFootIn = false;
            break;
            case IKFloorCollision.side.R: 
            print(DateTime.Now + ": R exit");
                rightFootIn = false;
            break;
        }
    }
    public void FootStayingInFloor(IKFloorCollision.side side, Collision collider)
    {
        switch(side)
        {
            case IKFloorCollision.side.L: 
            print(DateTime.Now + ": L");
                leftFootIn = true;
            break;
            case IKFloorCollision.side.R: 
            print(DateTime.Now + ": R");
                rightFootIn = true;
            break;
        }
    }

    bool DetermineFootControl()
    {
        IKFloorCollision.side changedValue = IKFloorCollision.side.neither;
        if(!leftFootIn && !rightFootIn)
        {
            // stay as neither
        }
        else if(leftFootIn && rightFootIn)
        {
            return false;
        }
        else if(leftFootIn)
        {
            changedValue = IKFloorCollision.side.L;
        }
        else if(rightFootIn)
        {
            changedValue = IKFloorCollision.side.R;
        }

        if(changedValue != actingFoot){
            actingFoot = changedValue;
            return true;
        } 
        else return false;
    }

    public float rotationFactor = 20f;
    public float forceFactor = 1f;
    private Vector3? lastPos;
    private Vector3? currentPos;
    void ApplyRotation(bool changed)
    {
        if(changed) {
            lastPos = null;
        }
        switch(actingFoot)
        {
            case IKFloorCollision.side.L:
            currentPos = leftFoot.position;
            break;
            case IKFloorCollision.side.R:
            currentPos = rightFoot.position;
            break;
            case IKFloorCollision.side.neither:
            currentPos = null;
            break;
        }

        if(currentPos != null && lastPos != null)
        {
            Vector3 diff = currentPos.Value - lastPos.Value;
            diff.y = 0;

            // Rotates cube
            transform.Rotate(new Vector3(0, 0, -diff.z)*rotationFactor);

            // Moves beam
            playerRoot.transform.position +=  playerRoot.transform.forward * diff.magnitude * forceFactor;
           // playerRoot.gameObject.GetComponent<Rigidbody>()
            //    .AddForce(diff * forceFactor, ForceMode.Force);
        }

        lastPos = currentPos;
    }

    // Update is called once per frame
    void Update()
    {
        // Determine which foot is controlling movement direction
        bool changed = DetermineFootControl();
        ApplyRotation(changed);


        Vector3 hipsLeftPos = hipsLeftTrans.position;
        Vector3 hipsRightPos = hipsRightTrans.position;

        // Position of waist/de facto centre of mass
        Vector3 centerPos = hipsLeftPos + ((hipsRightPos - hipsLeftPos) / 2);

        // leftFootIn = false;
        // rightFootIn = false;
    }
}
