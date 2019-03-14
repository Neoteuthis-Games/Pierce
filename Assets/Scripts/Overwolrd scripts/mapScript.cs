using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mapScript : MonoBehaviour
{
    float mapSpeed = 0.3f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Debug.Log("Drag");
            float lr = mapSpeed * Input.GetAxis("Mouse X");
            float ud = mapSpeed * Input.GetAxis("Mouse Y");
            transform.Translate(lr, ud, 0);
        }
        else
        {
            Debug.Log("Drop");
        }
    }
}