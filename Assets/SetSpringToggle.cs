using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSpringToggle : MonoBehaviour
{
    public bool legsOn = true;
    public bool armsOn = true;

    public bool toggleLegs = false;
    public bool toggleArms = false;
    public bool allOff = false;
    public bool allOn = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    enum ArmsOrLegs{Arms,Legs,Both}

    // Update is called once per frame
    void Update()
    {
        if(allOff || allOn || toggleArms || toggleLegs){
            if(allOff){
                SetInChildren(this.gameObject, ArmsOrLegs.Both, false);
                armsOn = false;
                legsOn = false;
            }else if(allOn){
                SetInChildren(this.gameObject, ArmsOrLegs.Both, true);
                armsOn = true;
                legsOn = true;
            }else if(toggleArms){
                SetInChildren(this.gameObject, ArmsOrLegs.Arms, !armsOn);
                armsOn = !armsOn;
            }else if(toggleLegs){
                SetInChildren(this.gameObject, ArmsOrLegs.Both, !legsOn);
                legsOn = !legsOn;
            }
        }
        
        toggleLegs = false;
        toggleArms = false;
        allOff = false;
        allOn = false;
    }

    private void SetInChildren(GameObject go, ArmsOrLegs part, bool on)
    {
        if(go.GetComponent<HingeJoint>() != null){
            switch(part){
                case ArmsOrLegs.Both:
                    go.GetComponent<HingeJoint>().useSpring = on;
                break;
                case ArmsOrLegs.Arms:
                    if(go.tag == "Arm") go.GetComponent<HingeJoint>().useSpring = on;
                break;
                case ArmsOrLegs.Legs:
                    if(go.tag == "Leg") go.GetComponent<HingeJoint>().useSpring = on;
                break;
            }
        }

        for(int i = 0; i < go.transform.childCount; i++){
            SetInChildren(go.transform.GetChild(i).gameObject, part, on);
        }
    }
}
