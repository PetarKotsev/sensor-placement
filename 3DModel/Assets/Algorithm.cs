using System;
using System.Collections.Generic;
using UnityEngine;


public abstract class Algorithm: MonoBehaviour
{
    // for position step
    public int updateCounter = 0;
    public int numOfIterations;


    public abstract void OptimizationStep();

    public abstract void Start();

    public abstract void Update();
}
