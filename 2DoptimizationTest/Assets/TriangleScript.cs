using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleScript : MonoBehaviour
{
    public Vector2 originPoint;
    public Vector2[] endPoints;
    public int numberOfPoints;

    private Mesh mesh;
    private Vector3[] vertices;
    private Vector2[] vertices2D;
    private Vector2[] uv;
    private int[] triangles;
    private int numberOfTriangles;

    void Start()
    {
        endPoints = new Vector2[numberOfPoints];
        // There is a mathematical formula that proves
        // that the number of tiangles that can be formed 
        // between n points is n-2
        numberOfTriangles = numberOfPoints - 2;

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        vertices = new Vector3[numberOfPoints];
        vertices2D = new Vector2[numberOfPoints];
        uv = new Vector2[numberOfPoints];
        triangles = new int[numberOfTriangles*3];
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < numberOfPoints; i++)
        {
            vertices[i] = (Vector3)endPoints[i];
            vertices[i].z = -4;

            vertices2D[0] = endPoints[i];
            uv[0] = endPoints[i];
        }

        for (int i = 0, j = 1; i < numberOfTriangles*3; i += 3, j++)
        {
            triangles[i] = 0;
            triangles[i + 1] = j;
            triangles[i + 2] = j + 1;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        GetComponent<PolygonCollider2D>().points = vertices2D;
    }

    [ContextMenu("DebugDisplay")]
    public void debug()
    {
        Debug.Log("x:" + transform.position.x + " y:" + transform.position.y + " z:" + transform.position.z);
    }
}
