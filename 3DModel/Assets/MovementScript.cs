using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour
{

    public float translationStepSize = 5;
    public float angleStepSize = 30;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            gameObject.transform.position += Vector3.left * translationStepSize * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            gameObject.transform.position += Vector3.right * translationStepSize * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            gameObject.transform.position += Vector3.up * translationStepSize * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            gameObject.transform.position += Vector3.down * translationStepSize * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.N))
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(Vector3.forward), angleStepSize * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.M))
        {
            gameObject.transform.eulerAngles -= Vector3.forward * angleStepSize;
        }
    }
}
