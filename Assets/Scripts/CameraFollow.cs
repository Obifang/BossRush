using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraFollow : MonoBehaviour
{
    public Camera Camera;
    public Transform Following;
    public float XOffset;
    public float YOffset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Following == null) {
            return;
        }
        transform.position = new Vector3(Following.position.x + XOffset, Following.position.y + YOffset, Camera.transform.position.z);
    }
}
