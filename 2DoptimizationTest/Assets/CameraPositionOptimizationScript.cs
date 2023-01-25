using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPositionOptimizationScript : MonoBehaviour
{
    public GameObject cameraObject;
    public GameObject houseObject;

    void Start()
    {
        Debug.Log(houseObject.transform.GetComponent<SpriteRenderer>().bounds);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
