using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFunction
{
    
    private float Density(Vector3 position)
    {

        float density = 0f;

        return density;
    }

    private float Density(float x, float y, float z)
    {

        float density = 0f;

        return density;
    }


    private Vector3 Normal(Vector3 position, float d = 0.001f)
    {

        Vector3 normal = new Vector3((Density(position.x + d, position.y, position.z) - Density(position.x - d, position.y, position.z)) / 2 / d,
                                     (Density(position.x, position.y + d, position.z) - Density(position.x, position.y - d, position.z)) / 2 / d,
                                     (Density(position.x, position.y, position.z + d) - Density(position.x, position.y, position.z - d)) / 2 / d);

        return normal.normalized;
    }
}
