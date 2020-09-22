using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyPosition : MonoBehaviour
{
    public Transform toCopy;
    
    public bool copyX = false;
    public bool copyY = false;
    public bool copyZ = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(toCopy != null)
        {
            Vector3 copyPos = toCopy.transform.position;
            Vector3 pos = transform.position;
            if(copyX) pos.x = copyPos.x;
            if(copyY) pos.y = copyPos.y;
            if(copyZ) pos.z = copyPos.z;

            transform.position = pos;
        }
    }
}
