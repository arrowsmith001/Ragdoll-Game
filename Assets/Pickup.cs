using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Pickup : MonoBehaviour
{
    public Transform PICKUP_MARKER;

    // Start is called before the first frame update
    public Pickupable pickupTarget;
    public bool pickingUp = false;

    void Update () {
        if (Input.GetKeyDown(KeyCode.Space)) {
            
            if(pickupTarget == null && pickupable != null)
            {
                pickupTarget = pickupable;

                pickupTarget.GetComponent<Rigidbody>().isKinematic = true;
                pickupTarget.GetComponent<Rigidbody> ().drag = GetComponent<Rigidbody> ().mass;

                pickingUp = true;
            }else if(pickupTarget != null){

                pickupTarget.GetComponent<Rigidbody>().isKinematic = false;
                pickupTarget.GetComponent<Rigidbody> ().drag = 0;

                pickupTarget = null;
                pickingUp = false;
            }
        }

        if(pickupTarget != null){

            // Calculate target axis
            Dictionary<String, Vector3> map = new Dictionary<String, Vector3>();
            map.Add("up", pickupTarget.transform.up);
            map.Add("down", -map["up"]);
            map.Add("right", pickupTarget.transform.right);
            map.Add("left", -map["right"]);
            map.Add("forward", pickupTarget.transform.forward);
            map.Add("back", -map["forward"]);
        
            Vector3 towards = (PICKUP_MARKER.transform.position - pickupTarget.transform.position).normalized;

            float least = float.MaxValue;
            String leastK = "";
            foreach(String k in map.Keys){
                Vector3 vec = map[k];
                float d = Vector3.Angle(towards, vec);
                if(d < least) leastK = k;

               // print(k + " " + d);
            }
            Vector3 tAxis = map[leastK];

            if(pickingUp)
            {
                // Move and turn slowly
                pickupTarget.GetComponent<Rigidbody>()
                    .MovePosition(pickupTarget.transform.position + towards * Time.deltaTime * moveSpeed);

                // Quaternion q = 
                //     Quaternion.FromToRotation(pickupTarget.transform.rotation.eulerAngles, 
                //         Vector3.Slerp(pickupTarget.transform.rotation.eulerAngles, PICKUP_MARKER.transform.rotation.eulerAngles, Time.deltaTime * rotSpeed));
                // pickupTarget.GetComponent<Rigidbody>().MoveRotation(q.normalized);

                if(Vector3.Distance(PICKUP_MARKER.transform.position, pickupTarget.transform.position) < 0.1
                  // && Vector3.Distance(pickupTarget.transform.rotation.eulerAngles, PICKUP_MARKER.transform.rotation.eulerAngles) < 0.1
                    ){
                    pickingUp = false;
                }else
                {
                    print(Vector3.Distance(PICKUP_MARKER.transform.position, pickupTarget.transform.position) 
                    + " " + Vector3.Distance(pickupTarget.transform.rotation.eulerAngles, PICKUP_MARKER.transform.rotation.eulerAngles));
                }
            }
            else
            {
                Quaternion q = 
                    Quaternion.FromToRotation(pickupTarget.transform.rotation.eulerAngles, PICKUP_MARKER.transform.rotation.eulerAngles);

                // Move and turn instantly
                pickupTarget.GetComponent<Rigidbody>().MovePosition(PICKUP_MARKER.position);
                pickupTarget.GetComponent<Rigidbody>().MoveRotation(PICKUP_MARKER.transform.rotation);
            }
 


        }

        
            
            //transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * rotSpeed);

            //GetComponent<Rigidbody> ().drag = GetComponent<Rigidbody> ().mass;
            //GetComponent<Rigidbody> ().MovePosition (pickupTarget.transform.position );

            //GetComponent<Rigidbody> ().MoveRotation (q.normalized);


    }

    public float moveSpeed = 5;
    public float rotSpeed = 5;

    Pickupable pickupable;
    public void PickupDetected(Pickupable pickupable){
        this.pickupable = pickupable;
    }

    private IEnumerator PickUp()
    {
        // Grab handles
        GameObject handL = Global.instance.playerHandL;
        GameObject handR = Global.instance.playerHandR;

        pickupTarget.GetComponent<Rigidbody>().isKinematic = true;
        handL.GetComponent<Rigidbody>().isKinematic = true;
        handR.GetComponent<Rigidbody>().isKinematic = true;

        pickupTarget.GetComponent<Rigidbody>().drag = pickupTarget.GetComponent<Rigidbody>().mass;
        handL.GetComponent<Rigidbody>().drag = handL.GetComponent<Rigidbody>().mass;
        handR.GetComponent<Rigidbody>().drag = handR.GetComponent<Rigidbody>().mass;


        float dL1, dL2, dR1, dR2;
        dL1 = Vector3.Distance(handL.transform.position, pickupTarget.handle1.position);
        dL2 = Vector3.Distance(handL.transform.position, pickupTarget.handle2.position);
        dR1 = Vector3.Distance(handR.transform.position, pickupTarget.handle1.position);
        dR2 = Vector3.Distance(handR.transform.position, pickupTarget.handle2.position);
        // TODO

        float d1 = Vector3.Distance(pickupTarget.handle1.transform.position, handL.transform.position);
        float d2 = Vector3.Distance(pickupTarget.handle2.transform.position, handR.transform.position);
            print("d1 " + d1 + " d2" + d2);
        
        while(d1 > 0.1 || d2 > 0.1){
            print("d1 " + d1 + " d2 " + d2);
           // handL.GetComponent<Rigidbody> ().MovePosition (handL.transform.position + (pickupTarget.handle1.transform.position - handL.transform.position).normalized * Time.deltaTime * handsMoveSpeed);
           // handR.GetComponent<Rigidbody> ().MovePosition (handR.transform.position + (pickupTarget.handle2.transform.position - handR.transform.position).normalized * Time.deltaTime * handsMoveSpeed);

            handL.GetComponent<Rigidbody> ().MovePosition (pickupTarget.handle1.transform.position);
            handR.GetComponent<Rigidbody> ().MovePosition (pickupTarget.handle2.transform.position);

            d1 = Vector3.Distance(pickupTarget.handle1.transform.position, handL.transform.position);
            d2 = Vector3.Distance(pickupTarget.handle2.transform.position, handR.transform.position);

            yield return new WaitForEndOfFrame();
        }
        

    }
}
