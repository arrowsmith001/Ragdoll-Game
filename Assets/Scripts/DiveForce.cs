using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiveForce : MonoBehaviour
{
    Vector3 initPos;
    Quaternion initRot;

    // Start is called before the first frame update
    void Start()
    {  
    }

    public float force;
    bool divedState = false;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)){

            if(divedState){
                this.gameObject.transform.position = initPos;
                this.gameObject.transform.rotation = initRot;
                this.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;

            }else
            {
                
                initPos = this.gameObject.transform.position;
                initRot = this.gameObject.transform.rotation;  
                this.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                this.gameObject.GetComponent<Rigidbody>().AddForce(this.gameObject.transform.forward * force, ForceMode.Impulse);
            }

            divedState = !divedState;

        }
    }
}
