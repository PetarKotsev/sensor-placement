using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReyCasterScript3D : MonoBehaviour
{
    // all angles are in degrees
    public float angle;
    public float fieldOfView = 70;
    public int numOfRays = 10;
    public float depthOfView = 10;
    private LineRenderer lr;
    public float area;

    private Vector3 lastVectorEndPoint;
    private Vector3[] endPoints;
    private float startAngle;
    public Vector3[] directions;

    private int layerMask = 1 << 11;

    void Start()
    {
        endPoints = new Vector3[numOfRays];
    }


    void Update()
    {
        area = updateArea();
    }

    public float updateArea ()
    {
        directions = new Vector3[numOfRays];
        startAngle = angle - fieldOfView / 2;
        lastVectorEndPoint = new Vector3(0, 0, 0);
        area = 0;


        float currAngle;
        for (int i = 0; i < numOfRays; i++)
        {
            currAngle = (startAngle + i * fieldOfView / (numOfRays - 1 )) * Mathf.Deg2Rad;
            directions[i] = new Vector3(Mathf.Cos(currAngle), Mathf.Sin(currAngle), 0);

            RaycastHit hit;

            if (Physics.Raycast(transform.position, directions[i], out hit, Mathf.Infinity, layerMask))
            {
                endPoints[i] = hit.point;
                if ((transform.position - endPoints[i]).magnitude > depthOfView)
                {
                    endPoints[i] = (-transform.position + endPoints[i]).normalized * depthOfView + transform.position;
                }
                if (lastVectorEndPoint.magnitude > 0)
                {
                    Vector3 v1 = transform.position - endPoints[i];
                    v1.z = 0;
                    Vector3 v2 = transform.position - lastVectorEndPoint;
                    v2.z = 0;
                    Vector3 V = Vector3.Cross(v1, v2);
                    area += V.magnitude / 2.0f;
                }
                //Debug.Log("> " + endPoints[i].x + " " + endPoints[i].y + " " + endPoints[i].z);
                Debug.DrawLine(transform.position, endPoints[i]);
                lastVectorEndPoint = endPoints[i];
            }
        }

        return area;
    }

    public void printAreas ()
    {
        for (int i = 1; i < endPoints.Length; i++)
        {
            Vector3 v1 = transform.position - (Vector3)endPoints[i];
            v1.z = 0;
            Vector3 v2 = transform.position - (Vector3)endPoints[i - 1];
            v2.z = 0;
            Vector3 V = Vector3.Cross(v1,v2);
            Debug.Log("Area[" + i + "} =" + V.magnitude / 2.0f + "end1: " + v1 + " end2: " + v2);
        }

    }
}
