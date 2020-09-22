using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PickupDetectedEvent : UnityEvent<Pickupable>{}

public class PickupTriggerDetector : MonoBehaviour
{
    public static PickupTriggerDetector instance;
    public PickupDetectedEvent pickupDetected = new PickupDetectedEvent();

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.GetComponent<Pickupable>() != null){

            pickupDetected.Invoke(other.gameObject.GetComponent<Pickupable>());

        }
    }
    private void OnTriggerExit(Collider other) {
        if(other.gameObject.GetComponent<Pickupable>() != null){

            pickupDetected.Invoke(null);

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
