using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDensityFunction
{

    float Density(Vector3 position);
    float Density(float x, float y, float z);

    Vector3 Normal(Vector3 position, float d = 0.001f);
}
