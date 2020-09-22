using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global : MonoBehaviour
{
    public static Global instance;

    private void Awake() {
        instance = this;
    }

    public GameObject playerHips;
    public GameObject playerHandL;
    public GameObject playerHandR;
}
