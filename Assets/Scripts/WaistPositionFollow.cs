using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaistPositionFollow : MonoBehaviour
{
    public float offset = 0.0f;

    public Transform hipsLeftTrans;
    public Transform hipsRightTrans;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;

        Vector3 hipsLeftPos = hipsLeftTrans.position;
        Vector3 hipsRightPos = hipsRightTrans.position;
        Vector3 centerPos = hipsLeftPos + ((hipsRightPos - hipsLeftPos) / 2);

        transform.position = new Vector3(pos.x, centerPos.y + offset, pos.z);
    }
}
