using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraPositionOptimizationScript2 : Algorithm
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
    public Vector3[] currPositions;
    private int angleCounter = 0;
    private int angleDelta = 10;
    private int numOfAngles = 36;

    // best results
    public float[] maxAngles;
    public Vector3[] maxPositions;
    public float maxAreaSum = 0;
    public float[] maxAreas;

    private int cameraCounter = 0;

    public override void Start()
    {
        Debug.Assert(cameraObject == null, "Camera object not provided");
        Debug.Assert(cameraObject.GetComponent<ViewCasterScript>() == null, "Given camera object is not of type ViewCasterScript");

        // for position step
        updateCounter = 0;
        numOfIterations = 90;
    
        newCameraArray = new GameObject[numberOfCameras];
        currPositions = new Vector3[numberOfCameras];
        maxPositions = new Vector3[numberOfCameras];

        currMaxAngles = new float[numberOfCameras];
        currMaxAreas = new float[numberOfCameras];
        maxAngles = new float[numberOfCameras];
        r = houseObject.GetComponent<SpriteRenderer>().bounds;

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
        Debug.Log("cameras created");
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
        else
        {
            areaCoveredTextField.text = "Area covered: " + evalueateCameraArrayCoverage(maxPositions, maxAngles);
            drawMeshes();
        }
    }

    public void RepositionAndCheckAngles()
    {
        if (cameraCounter < numberOfCameras)
        {
            // angle optimization 
            if (angleCounter < numOfAngles)
            {
                float newAngle = newCameraArray[cameraCounter].GetComponent<CameraSensor>().angle + angleDelta;
                PositionCamera(newCameraArray[cameraCounter], newCameraArray[cameraCounter].transform.position, newAngle);
                // calculate area
                float area = newCameraArray[cameraCounter].GetComponent<CameraSensor>().updateArea();

                if (area > currMaxAreas[cameraCounter])
                {
                    currMaxAreas[cameraCounter] = area;
                    currMaxAngles[cameraCounter] = newAngle;
                }
                angleCounter++;
            } 
            else
            {
                Debug.Log("camera[" + cameraCounter + "] - full spin");
                // make new camera and start anew
                angleCounter = 0;

                // position camera at max angle
                PositionCamera(newCameraArray[cameraCounter], currPositions[cameraCounter], currMaxAngles[cameraCounter]);

                // draw mesh at max angle
                //newCameraArray[cameraCounter].GetComponent<CameraSensor>().drawMesh();
                areaCoveredTextField.text = "Area covered: " + evalueateCameraArrayCoverage();

                // draw mesh
                newCameraArray[cameraCounter].GetComponent<ViewCasterScript>().drawMesh();

                // increase camera counter
                cameraCounter++;
            }
        }
        
        // evaluation and repositioning
        else
        {
            float currMaxArea = evalueateCameraArrayCoverage();

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

                // destroy old objects
                Destroy(newCameraArray[i]);

                // make new camera
                newCameraArray[i] = makeCamera();

                // randomly position camera
                currPositions[i] = randomlyPositionCamera(newCameraArray[i], r);
            }

            updateCounter++;
            cameraCounter = 0;
        }
    }

    public Vector3 randomlyPositionCamera(GameObject camera, Bounds r)
    {
        var x = Random.Range(r.center.x - r.extents.x, r.center.x + r.extents.x);
        var y = Random.Range(r.center.y - r.extents.y, r.center.y + r.extents.y);
        var rot = 0; // Random.Range(-180f, 180f);

        Vector3 position = new Vector3(x, y, -4f);

        camera.transform.position = position;
        camera.GetComponent<CameraSensor>().angle = rot;

        return position;
    }

    [ContextMenu("Reposition all cameras")]
    public void randomlyPositionAllCameras()
    {
        for (int i = 0; i < numberOfCameras; i++)
        {
            randomlyPositionCamera(newCameraArray[i], r);
        }
    }

    public void PositionCamera(GameObject camera, Vector3 position, float angle)
    {
        camera.transform.position = position;
        camera.GetComponent<CameraSensor>().angle = angle;
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


    public void drawMeshes()
    {
        for (int j = 0; j < numberOfCameras; j++)
        {
            newCameraArray[j].GetComponent<ViewCasterScript>().drawMesh();
        }
    }
    public void removeMeshes ()
    {
        for (int j = 0; j < numberOfCameras; j++)
        {
            newCameraArray[j].GetComponent<ViewCasterScript>().removeMesh();
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
            //newCameraArray[j].GetComponentInParent<CameraSensor>().printAreas();
        }
    }
}
