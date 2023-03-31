using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraPositionOptimizationScript3D : Algorithm
{
    public GameObject cameraObject;
    public GameObject houseObject;
    public Text areaCoveredTextField;
    private Bounds r;

    public int numberOfCameras = 1;

    private GameObject[] newCameraArray;

    // for angle step
    private float[] currMaxAngles;
    private float[] currMaxAreas;
    private Vector3[] currPositions;
    private int angleCounter = 0;
    private int angleDelta = 10;
    private int numOfAngles = 36;

    // best results
    public float[] maxAngles;
    public Vector3[] maxPositions;
    public float maxAreaSum = 0;
    public float[] maxAreas;



    public override void Start()
    {
        // for position step
        updateCounter = 0;
        numOfIterations = 90;

        newCameraArray = new GameObject[numberOfCameras];
        currPositions = new Vector3[numberOfCameras];
        maxPositions = new Vector3[numberOfCameras];

        currMaxAngles = new float[numberOfCameras];
        currMaxAreas = new float[numberOfCameras];
        maxAngles = new float[numberOfCameras];
        r = houseObject.GetComponent<Renderer>().bounds;

        for (int i = 0; i < numberOfCameras; i++)
        {
            // Rest angles and areas
            currMaxAngles[i] = 0;
            currMaxAreas[i] = 0;
            maxAngles[i] = 0;
            // make new camera
            newCameraArray[i] = makeCamera();
            // randomly position camera
            currPositions[i] = randomlyPositionCamera(newCameraArray[i], r);
        }
    }

    // Update is called once per frame
    public override void Update()
    {
        // Optimization
        OptimizationStep();
    }

    public override void OptimizationStep()
    {
        if (updateCounter < numOfIterations)
        {
            RepositionAndCheckAngles();

        }
        else if (updateCounter == 0)
        {
            // for debugging perposes
        }
        else
        {
            areaCoveredTextField.text = "Area covered: " + evalueateCameraArrayCoverage(maxPositions, maxAngles);
        }
    }

    public void RepositionAndCheckAngles()
    {
        // angle optimization step
        if (angleCounter < numOfAngles)
        {
            float areaSum = 0;
            for (int j = 0; j < numberOfCameras; j++)
            {

                float newAngle = newCameraArray[j].GetComponent<CameraSensor>().angle + angleDelta;
                PositionCamera(newCameraArray[j], newCameraArray[j].transform.position, newAngle);
                // calculate area
                float area = newCameraArray[j].GetComponent<CameraSensor>().updateArea();
                areaSum += area;

                if (area > currMaxAreas[j])
                {
                    currMaxAreas[j] = area;
                    currMaxAngles[j] = newAngle;
                }
                //Debug.Log("Camera[" + j + "] Angle: " + newAngle + " Area: " + areaSum);
            }

            //areaCoveredTextField.text = "Area covered: " + areaSum;
            angleCounter++;
        }
        // evaluation and repositioning
        else
        {
            angleCounter = 0;

            float currMaxArea = evalueateCameraArrayCoverage(currPositions, currMaxAngles);

            if (currMaxArea > maxAreaSum)
            {
                maxAreaSum = currMaxArea;
                maxAngles = (float[])currMaxAngles.Clone();
                maxPositions = (Vector3[])currPositions.Clone();
                maxAreas = (float[])currMaxAreas.Clone();

                Debug.Log("Updated: " + maxAreaSum);
                for (int i = 0; i < numberOfCameras; i++) Debug.Log("Area[" + i + "]: ang(" + maxAngles[i] + "), pos(" + maxPositions[i].x + ", " + maxPositions[i].y + ", " + maxPositions[i].x + ") " + "->" + maxAreas[i]);
            }

            areaCoveredTextField.text = "Max area: " + maxAreaSum;

            for (int i = 0; i < numberOfCameras; i++)
            {
                // Rest angles and areas
                currMaxAngles[i] = 0;
                currMaxAreas[i] = 0;
                // randomly position camera
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

        Vector3 position = new Vector3(x, y, transform.position.z);

        camera.transform.position = position;
        camera.GetComponent<CameraSensor>().angle = rot;

        return position;
    }

    public void PositionCamera(GameObject camera, Vector3 position, float angle)
    {
        camera.transform.position = position;
        camera.GetComponent<CameraSensor>().angle = angle;
    }

    public GameObject makeCamera()
    {
        return Instantiate(cameraObject, new Vector3(0, 0, transform.position.z), Quaternion.Euler(0, 0, 0));
    }

    public float evalueateCameraArrayCoverage()
    {
        float areaSum = 0;

        for (int i = 0; i < numberOfCameras; i++)
        {
            areaSum += newCameraArray[i].GetComponent<CameraSensor>().updateArea();
        }

        return areaSum;
    }

    public float evalueateCameraArrayCoverage(Vector3[] positions, float[] angles)
    {
        float areaSum = 0;

        for (int j = 0; j < numberOfCameras; j++)
        {
            PositionCamera(newCameraArray[j], positions[j], angles[j]);
            areaSum += newCameraArray[j].GetComponent<CameraSensor>().updateArea();
        }

        return areaSum;
    }

    //public void OnDrawGizmosSelected()
    //{
    //    var r = houseObject.GetComponent<Renderer>();
    //    if (r == null)
    //        return;
    //    var bounds = r.bounds;
    //    Gizmos.matrix = Matrix4x4.identity;
    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawWireCube(bounds.center, bounds.extents * 2);
    //}

    [ContextMenu("Move angle for all")]
    public void moveAngle()
    {
        float areaSum = 0;
        for (int j = 0; j < numberOfCameras; j++)
        {

            float newAngle = newCameraArray[j].GetComponent<CameraSensor>().angle + angleDelta;
            PositionCamera(newCameraArray[j], newCameraArray[j].transform.position, newAngle);
            // calculate area
            float area = newCameraArray[j].GetComponent<CameraSensor>().updateArea();
            areaSum += area;

            if (area > currMaxAreas[j])
            {
                currMaxAreas[j] = area;
                currMaxAngles[j] = newAngle;
            }
            Debug.Log("Camera[" + j + "] Angle: " + newAngle + " Area: " + areaSum);
            //newCameraArray[j].GetComponentInParent<ReyCasterScript>().printAreas();
        }
    }
}
