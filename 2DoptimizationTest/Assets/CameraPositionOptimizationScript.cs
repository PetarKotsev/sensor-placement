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
    public int updateCounter = 0;
    public float maxAreaSum = 0;
    private int angleCounter = 0;
    private float[] currMaxAngles = { 0, 0, 0, 0, 0 };
    private float[] currMaxAreas = { 0, 0, 0, 0, 0 };
    public int numOfIterations = 90;
    public int numberOfCameras = 5;
    private Vector3[] maxPositions;

    void Start()
    {
        Debug.Log(houseObject.GetComponent<SpriteRenderer>().bounds.center);
        Debug.Log(houseObject.GetComponent<SpriteRenderer>().bounds.extents);

        Bounds r = houseObject.GetComponent<SpriteRenderer>().bounds;

        float areaSum = 0;
        float area = 0;

        newCameraArray = new GameObject[numberOfCameras];
        maxPositions = new Vector3[numberOfCameras];

        for (int i = 0; i < numberOfCameras; i++)
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
        OptimizationStep();
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


    public void OptimizationStep()
    {
        if (updateCounter < numOfIterations)
        {
            RepositionAndCheckAngles();
        }
        else
        {
            float areaSum = 0;
            for (int j = 0; j < numberOfCameras; j++)
            {
                newCameraArray[j].transform.position = maxPositions[j];
                newCameraArray[j].GetComponent<ReyCasterScript>().angle = maxAngles[j];
                areaSum += newCameraArray[j].GetComponent<ReyCasterScript>().area;
            }
            areaCoveredTextField.text = "Area covered: " + areaSum;
        }
    }

    public void RepositionAndCheckAngles ()
    {

        if (angleCounter < 36)
        {
            float areaSum = 0;
            for (int j = 0; j < numberOfCameras; j++)
            {
                float newAngle = newCameraArray[j].GetComponent<ReyCasterScript>().angle + 10;
                newCameraArray[j].GetComponent<ReyCasterScript>().angle = newAngle;
                float area = newCameraArray[j].GetComponent<ReyCasterScript>().area;
                areaSum += area;

                if (area > currMaxAreas[j])
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
            Vector3[] positions = new Vector3[numberOfCameras];
            for (int j = 0; j < numberOfCameras; j++)
            {
                newCameraArray[j].GetComponent<ReyCasterScript>().angle = currMaxAngles[j];
                currMaxArea += newCameraArray[j].GetComponent<ReyCasterScript>().area;
                positions[j] = newCameraArray[j].transform.position;
            }
            areaCoveredTextField.text = "Area covered: " + currMaxArea;

            if (currMaxArea > maxAreaSum)
            {
                maxAreaSum = currMaxArea;
                maxAngles = (float[])currMaxAngles.Clone();
                maxPositions = (Vector3[])positions.Clone();

                Debug.Log("Updated: " + maxAreaSum);
                for (int i = 0; i < numberOfCameras; i++) Debug.Log("Area[" + i + "]: " + currMaxAreas[i]);
            }

            Bounds r = houseObject.GetComponent<SpriteRenderer>().bounds;

            for (int i = 0; i < numberOfCameras; i++)
            {

                var x = Random.Range(r.center.x - r.extents.x, r.center.x + r.extents.x);
                var y = Random.Range(r.center.y - r.extents.y, r.center.y + r.extents.y);
                var rot = Random.Range(-180f, 180f);


                newCameraArray[i].transform.position = new Vector3(x, y, -4f);
                newCameraArray[i].GetComponent<ReyCasterScript>().angle = rot;
            }

            updateCounter++;
        }
    }
}
