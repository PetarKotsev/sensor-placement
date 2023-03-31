using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class SensorObject : MonoBehaviour
{
    public object value;

    public float[] positions;

    public float[] orientations;
    public abstract void printAreas();
    public abstract void updateArea();
}
