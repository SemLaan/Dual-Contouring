using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFunction : IDensityFunction
{
    
    public float Density(Vector3 position)
    {

        float density = 1f;

        if (position.y > 50)
            density = -1;

        float height = Mathf.PerlinNoise(position.x * 0.01f, position.z * 0.01f) * 130;
        height += Mathf.PerlinNoise(position.x * 0.03f, position.z * 0.03f) * 10;
        height += Mathf.PerlinNoise(position.x * 0.09f, position.z * 0.09f) * 5;
        height += Mathf.PerlinNoise(position.x * 0.018f, position.z * 0.018f) * 2.5f;
        height += Mathf.PerlinNoise(position.x * 0.036f, position.z * 0.036f) * 1.25f;

        if (height < 55)
        {

            height *= 0.05f;
            height += 52;

        }

        density = (int)((position.y - height) * 50);

        return density;
    }

    public float Density(float x, float y, float z)
    {

        return Density(new Vector3(x, y, z));
    }


    public Vector3 Normal(Vector3 position, float d = 0.001f)
    {

        Vector3 normal = new Vector3((Density(position.x + d, position.y, position.z) - Density(position.x - d, position.y, position.z)) / 2 / d,
                                     (Density(position.x, position.y + d, position.z) - Density(position.x, position.y - d, position.z)) / 2 / d,
                                     (Density(position.x, position.y, position.z + d) - Density(position.x, position.y, position.z - d)) / 2 / d);

        return normal.normalized;
    }
}
