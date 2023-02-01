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
    public float[] maxAngles = { 0,0,0,0,0};
    public float[] maxAreas = { 0,0,0,0,0};
    public int updateCounter = 0;
    private float maxAreaSum = 0;
    private int angleCounter = 0;

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
        if (updateCounter < 180)
        {
            float[] currMaxAngles = { 0, 0, 0, 0, 0 };
            float[] currMaxAreas = { 0, 0, 0, 0, 0 };

            if (angleCounter < 36)
            {
                float areaSum = 0;
                for (int j = 0; j < 5; j++)
                {
                    float newAngle = newCameraArray[j].GetComponent<ReyCasterScript>().angle + 10;
                    newCameraArray[j].GetComponent<ReyCasterScript>().angle = newAngle;
                    float area = newCameraArray[j].GetComponent<ReyCasterScript>().area;
                    areaSum += area;

                    if (area > maxAreas[j])
                    {
                        currMaxAreas[j] = area;
                        currMaxAngles[j] = newAngle;
                    }
                }

                areaCoveredTextField.text = "Area covered: " + areaSum;
                angleCounter++;
            }
            else
            {
                angleCounter = 0;

                float currMaxArea = 0;
                for (int j = 0; j < 5; j++)
                {
                    newCameraArray[j].GetComponent<ReyCasterScript>().angle = maxAngles[j];
                    currMaxArea += newCameraArray[j].GetComponent<ReyCasterScript>().area;
                }
                areaCoveredTextField.text = "Area covered: " + currMaxArea;

                if (currMaxArea > maxAreaSum)
                {
                    maxAreaSum = currMaxArea;
                    maxAreas = currMaxAreas;
                    maxAngles = currMaxAngles;

                }

                Bounds r = houseObject.GetComponent<SpriteRenderer>().bounds;

                for (int i = 0; i < 5; i++)
                {

                    var x = Random.Range(r.center.x - r.extents.x, r.center.x + r.extents.x);
                    var y = Random.Range(r.center.y - r.extents.y, r.center.y + r.extents.y);
                    var rot = Random.Range(-180f, 180f);


                    newCameraArray[i].transform.position =  new Vector3(x, y, -4f);
                    newCameraArray[i].GetComponent<ReyCasterScript>().angle = rot;
                }

                updateCounter++;
            }
        }
        else
        {
            for (int j = 0; j < 5; j++)
            {
                newCameraArray[j].GetComponent<ReyCasterScript>().angle = maxAngles[j];
            }
            areaCoveredTextField.text = "Area covered: " + maxAreaSum;
        }
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
        int j = 0;
        int i = 0;
        area = newCameraArray[i].GetComponent<ReyCasterScript>().area;
        areaSum += area;
        if (area > maxAreas[i])
        {
            maxAreas[i] = area;
            maxAngles[i] = newCameraArray[i].GetComponent<ReyCasterScript>().angle;
        }
        newCameraArray[i].GetComponent<ReyCasterScript>().angle += 10;

        i++;
        j++;

        return areaSum;
    }
}
