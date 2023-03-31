using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class SensorObject : MonoBehaviour
{
    public float value;

    public abstract void printAreas();
    public abstract float updateArea();
}
