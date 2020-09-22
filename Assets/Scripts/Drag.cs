using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drag : MonoBehaviour
{
    public GameObject dragIK;
    public bool drag = false;
    void Update () {
        if (drag) {
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Rigidbody> ().drag = GetComponent<Rigidbody> ().mass;
            GetComponent<Rigidbody> ().MovePosition (dragIK.transform.position);

        } else {
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Rigidbody> ().drag = 0;
        }
    }
}
