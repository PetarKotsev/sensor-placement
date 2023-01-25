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

    // Start is called before the first frame update
    void Start()
    {
        lr = gameObject.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2[] directions = new Vector2[numOfRays];
        float startAngle = angle - fieldOfView / 2;

        float currAngle;
        for (int i = 0; i < numOfRays; i++)
        {
            currAngle = (startAngle + i * fieldOfView / numOfRays) * Mathf.Deg2Rad;
            directions[i] = new Vector2(Mathf.Cos(currAngle), Mathf.Sin(currAngle));
            directions[i] = directions[i].normalized;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, directions[i]);
            if (hit.collider)
            {
                Vector2 endPoint = hit.point;
                if (((Vector2)transform.position - endPoint).magnitude > depthOfView)
                {
                    endPoint = (-(Vector2)transform.position + endPoint).normalized*depthOfView + (Vector2)transform.position;
                }
                Debug.DrawLine(transform.position, endPoint);
                //    lr.SetPosition(0, transform.position);
                //    lr.SetPosition(1, hit.point);
            }
        }
    }

    
}
