using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewCasterScript : MonoBehaviour
{
    // all angles are in degrees
    public float angle;
    public float fieldOfView = 70;
    public int numOfRays = 10;
    public float depthOfView = 10;
    public float area;

    private Vector2 lastVectorEndPoint;
    public Vector2[] endPoints;
    private float startAngle;
    Vector2[] directions;

    private Mesh mesh;
    public Vector3[] vertices;
    public Vector2[] vertices2D;
    private Vector2[] uv;
    private int[] triangles;
    private int numberOfTriangles;

    private bool isMeshDrawn = false;

    private int layerMask = 1 << 7;

    void Start()
    {
        endPoints = new Vector2[numOfRays];

        // There is a mathematical formula that proves
        // that the number of tiangles that can be formed 
        // between n points is n-2
        numberOfTriangles = (numOfRays+1) - 2;

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        vertices = new Vector3[numOfRays+1];
        vertices2D = new Vector2[numOfRays+1];
        uv = new Vector2[numOfRays+1];
        triangles = new int[numberOfTriangles * 3];

    }


    void Update()
    {
        //area = updateArea();
    }

    public float updateArea ()
    {
        if (isMeshDrawn)
        {
            return area;
        }
        directions = new Vector2[numOfRays];
        startAngle = angle - fieldOfView / 2;
        lastVectorEndPoint = new Vector2(0, 0);
        area = 0;

        float currAngle;
        for (int i = 0; i < numOfRays; i++)
        {
            currAngle = (startAngle - i * fieldOfView / numOfRays) * Mathf.Deg2Rad;
            directions[i] = new Vector2(Mathf.Cos(currAngle), Mathf.Sin(currAngle));
            directions[i] = directions[i].normalized;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, directions[i], Mathf.Infinity, layerMask);
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
                //Debug.Log("pos: (x:" + transform.position.x + ", y:" + transform.position.y + ", z:" + transform.position.z + ")");
                //Debug.Log("endPoint:[" + i + "] (x:" + endPoints[i].x + ", y:" + endPoints[i].y + ")");
                //Debug.Log("diff :[" + i + "] (x:" + (endPoints[i].x - transform.position.x) + ", y:" + (endPoints[i].y - transform.position.y) + ")");

                lastVectorEndPoint = endPoints[i];
            }
        }
        return area;
    }

    [ContextMenu("Draw mesh")]
    public void drawMesh()
    {
        vertices[0] = Vector3.zero;
        vertices[0].z = transform.position.z;

        vertices2D[0] = Vector2.zero;
        uv[0] = Vector2.zero;
        for (int i = 1; i < numOfRays + 1; i++)
        {
            vertices[i] = ((Vector3) (endPoints[i - 1] - (Vector2) transform.position)) * 3.75f;
            vertices[i].z = transform.position.z;

            vertices2D[i] = (endPoints[i - 1] - (Vector2)transform.position) * 3.75f;
            uv[i] = endPoints[i - 1];
        }

        for (int i = 0, j = 1; i < numberOfTriangles * 3; i += 3, j++)
        {
            triangles[i] = 0;
            triangles[i + 1] = j;
            triangles[i + 2] = j + 1;
        }

        mesh.vertices = (Vector3[]) vertices.Clone();
        mesh.uv = (Vector2[]) uv.Clone();
        mesh.triangles = (int[]) triangles.Clone();

        GetComponent<PolygonCollider2D>().points = (Vector2[]) vertices2D.Clone();

        isMeshDrawn = true;
    }

    public void removeMesh ()
    {
        mesh.Clear();
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
