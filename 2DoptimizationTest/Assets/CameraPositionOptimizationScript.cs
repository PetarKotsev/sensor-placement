using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraPositionOptimizationScript : MonoBehaviour
{
    public GameObject cameraObject;
    public GameObject houseObject;
    public Text areaCoveredTextField;
    private Bounds r;

    public int numberOfCameras = 5;

    private GameObject[] newCameraArray;

    // for angle step
    private float[] currMaxAngles = { 0, 0, 0, 0, 0 };
    private float[] currMaxAreas = { 0, 0, 0, 0, 0 };
    private Vector3[] currPositions;
    private int angleCounter = 0;
    private int angleDelta = 10;
    private int numOfAngles = 36;

    // for position step
    public int updateCounter = 0;
    public int numOfIterations = 90;

    // best results
    public float[] maxAngles = { 0, 0, 0, 0, 0 };
    public Vector3[] maxPositions;
    public float maxAreaSum = 0;
    public float[] maxAreas;



    void Start()
    {
        newCameraArray = new GameObject[numberOfCameras];
        currPositions = new Vector3[numberOfCameras];
        maxPositions = new Vector3[numberOfCameras];
        r = houseObject.GetComponent<SpriteRenderer>().bounds;


        for (int i = 0; i < numberOfCameras; i++)
        {
            // make new camera
            newCameraArray[i] = makeCamera();
            // randomly position camera
            currPositions[i] = randomlyPositionCamera(newCameraArray[i], r);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Optimization
        OptimizationStep();
    }

    public void OptimizationStep()
    {
        if (updateCounter < numOfIterations)
        {
            RepositionAndCheckAngles();
        }
        //if (updateCounter == numOfIterations)
        else
        {
            areaCoveredTextField.text = "Area covered: " + evalueateCameraArrayCoverage(maxPositions, maxAngles);
            //updateCounter++;
        }
    }

    public void RepositionAndCheckAngles ()
    {
        // angle optimization step
        if (angleCounter < numOfAngles)
        {
            float areaSum = 0;
            for (int j = 0; j < numberOfCameras; j++)
            {

                float newAngle = newCameraArray[j].GetComponent<ReyCasterScript>().angle + angleDelta;
                PositionCamera(newCameraArray[j], newCameraArray[j].transform.position, newAngle);
                // calculate area
                float area = newCameraArray[j].GetComponent<ReyCasterScript>().area;
                areaSum += area;

                if (area > currMaxAreas[j])
                {
                    currMaxAreas[j] = area;
                    currMaxAngles[j] = newAngle;
                    maxPositions[j] = newCameraArray[j].transform.position;
                }
            }

            areaCoveredTextField.text = "Area covered: " + areaSum;
            angleCounter++;
        }
        // evaluation and repositioning
        else
        {
            angleCounter = 0;

            float currMaxArea = evalueateCameraArrayCoverage(currPositions, currMaxAngles);

            areaCoveredTextField.text = "Area covered: " + currMaxArea;

            if (currMaxArea > maxAreaSum)
            {
                maxAreaSum = currMaxArea;
                maxAngles = (float[])currMaxAngles.Clone();
                maxPositions = (Vector3[])currPositions.Clone();
                maxAreas = (float[])currMaxAreas.Clone();

                Debug.Log("Updated: " + maxAreaSum);
                for (int i = 0; i < numberOfCameras; i++) Debug.Log("Area[" + i + "]: ang(" + maxAngles[i]+ "), pos(" + maxPositions[i].x + ", " + maxPositions[i].y + ", " + maxPositions[i].x + ") " + "->" + maxAreas[i]);
            }

            for (int i = 0; i < numberOfCameras; i++)
            {
                currPositions[i] = randomlyPositionCamera(newCameraArray[i], r);
            }

            updateCounter++;
        }
    }

    public Vector3 randomlyPositionCamera(GameObject camera, Bounds r)
    {
        var x = Random.Range(r.center.x - r.extents.x, r.center.x + r.extents.x);
        var y = Random.Range(r.center.y - r.extents.y, r.center.y + r.extents.y);
        var rot = 0; // Random.Range(-180f, 180f);

        Vector3 position = new Vector3(x, y, -4f);

        camera.transform.position = position;
        camera.GetComponent<ReyCasterScript>().angle = rot;

        return position;
    }

    public void PositionCamera (GameObject camera, Vector3 position, float angle)
    {
        camera.transform.position = position;
        camera.GetComponent<ReyCasterScript>().angle = angle;
    }

    public GameObject makeCamera()
    {
        return Instantiate(cameraObject, new Vector3(0, 0, -4f), Quaternion.Euler(0, 0, 0));
    }

    public float evalueateCameraArrayCoverage()
    {
        float areaSum = 0;
        
        for (int i = 0; i < numberOfCameras; i++)
        {
            areaSum += newCameraArray[i].GetComponent<ReyCasterScript>().area;
        }

        return areaSum;
    }

    public float evalueateCameraArrayCoverage(Vector3[] positions, float[] angles)
    {
        float areaSum = 0;

        for (int j = 0; j < numberOfCameras; j++)
        {
            PositionCamera(newCameraArray[j],positions[j], angles[j]);
            areaSum += newCameraArray[j].GetComponent<ReyCasterScript>().area;
        }

        return areaSum;
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
}
