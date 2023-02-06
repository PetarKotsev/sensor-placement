using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReyCasterScript : MonoBehaviour
{
    // all angles are in degrees
    public float angle;
    public float fieldOfView = 70;
    public int numOfRays = 10;
    public float depthOfView = 10;
    private LineRenderer lr;
    public float area;

    private Vector2 lastVectorEndPoint;
    private Vector2[] endPoints;
    private float startAngle;
    Vector2[] directions;

    void Start()
    {
        endPoints = new Vector2[numOfRays];
    }


    void Update()
    {
        area = updateArea();
    }

    public float updateArea ()
    {
        directions = new Vector2[numOfRays];
        startAngle = angle - fieldOfView / 2;
        lastVectorEndPoint = new Vector2(0, 0);
        area = 0;

        float currAngle;
        for (int i = 0; i < numOfRays; i++)
        {
            currAngle = (startAngle + i * fieldOfView / numOfRays) * Mathf.Deg2Rad;
            directions[i] = new Vector2(Mathf.Cos(currAngle), Mathf.Sin(currAngle));
            directions[i] = directions[i].normalized;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, directions[i]);
            if (hit.collider)
            {
                endPoints[i] = hit.point;
                if (((Vector2)transform.position - endPoints[i]).magnitude > depthOfView)
                {
                    endPoints[i] = (-(Vector2)transform.position + endPoints[i]).normalized * depthOfView + (Vector2)transform.position;
                }
                if (lastVectorEndPoint.magnitude > 0)
                {
                    Vector3 v1 = transform.position - (Vector3)endPoints[i];
                    v1.z = 0;
                    Vector3 v2 = transform.position - (Vector3)lastVectorEndPoint;
                    v2.z = 0;
                    Vector3 V = Vector3.Cross(v1, v2);
                    area += V.magnitude / 2.0f;
                }
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
