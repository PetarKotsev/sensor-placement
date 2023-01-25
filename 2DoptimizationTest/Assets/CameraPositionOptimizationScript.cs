using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraPositionOptimizationScript : MonoBehaviour
{
    public GameObject cameraObject;
    public GameObject houseObject;
    public Text areaCoveredTextField;
    private GameObject[] newCameraArray;
    private float[] maxAngles = { 0,0,0,0,0};
    private float[] maxAreas = { 0,0,0,0,0};

    void Start()
    {
        Debug.Log(houseObject.GetComponent<SpriteRenderer>().bounds.center);
        Debug.Log(houseObject.GetComponent<SpriteRenderer>().bounds.extents);

        Bounds r = houseObject.GetComponent<SpriteRenderer>().bounds;

        float areaSum = 0;
        float area = 0;

        newCameraArray = new GameObject[5];

        for (int i = 0; i < 5; i++)
        {
            var x = Random.Range(r.center.x - r.extents.x, r.center.x + r.extents.x);
            var y = Random.Range(r.center.y - r.extents.y, r.center.y + r.extents.y);
            var rot = Random.Range(-180f, 180f);

            newCameraArray[i] = Instantiate(cameraObject, new Vector3(x, y, -4f), Quaternion.Euler(0, 0, 0));
            newCameraArray[i].GetComponent<ReyCasterScript>().angle = rot;

            area = newCameraArray[i].GetComponent<ReyCasterScript>().area;
            areaSum += area;
        }
    }

    // Update is called once per frame
    void Update()
    {

        float areaSum = 0;
        for (int j = 0; j < 5; j++)
        {
            areaSum += newCameraArray[j].GetComponent<ReyCasterScript>().area;

            newCameraArray[j].GetComponent<ReyCasterScript>().angle += 30*Time.deltaTime;
        }
        areaCoveredTextField.text = "Area covered: " + areaSum;


    }

    public void OnDrawGizmosSelected()
    {
        var r = houseObject.GetComponent<SpriteRenderer>();
        if (r == null)
            return;
        var bounds = r.bounds;
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(bounds.center, bounds.extents * 2);
    }

    public float Optimize()
    {
        float areaSum = 0;
        float area = 0;
        for (int j = 0; j < 180; j++)
        {
            for (int i = 0; i < 5; i++)
            {
                area = newCameraArray[i].GetComponent<ReyCasterScript>().area;
                areaSum += area;
                if (area > maxAreas[i])
                {
                    maxAreas[i] = area;
                    maxAngles[i] = newCameraArray[i].GetComponent<ReyCasterScript>().angle;
                }
                newCameraArray[i].GetComponent<ReyCasterScript>().angle += 10;
            }
            areaCoveredTextField.text = "Area covered: " + areaSum;
        }

        return areaSum;
    }
}
