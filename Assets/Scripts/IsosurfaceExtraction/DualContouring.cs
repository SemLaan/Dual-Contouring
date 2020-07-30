using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class DualContouring : MonoBehaviour
{
    


    private Vector3 FindBestVertex(IDensityFunction densityFunction, IVertexPositionSolver vertexSolver, int x, int y, int z)
    {

        // Evaluate density function at all corners of voxel xyz
        float[,,] cornerDensities = new float[2, 2, 2];

        for (int dx = 0; dx < 2; dx++)
            for (int dy = 0; dy < 2; dy++)
                for (int dz = 0; dz < 2; dz++)
                    cornerDensities[dx, dy, dz] = densityFunction.Density(x+dx, y+dy, z+dz);

        // Find all the sign changes on the edges and save their coordinates
        List<Vector3> signChanges = new List<Vector3>();

        for (int dx = 0; dx < 2; dx++)
            for (int dy = 0; dy < 2; dy++)
                if ((cornerDensities[dx, dy, 0] > 0) != (cornerDensities[dx, dy, 1] > 0))
                    signChanges.Add(new Vector3(x + dx, y + dy, z + Adapt(cornerDensities[dx, dy, 0], cornerDensities[dx, dy, 1])));

        for (int dx = 0; dx < 2; dx++)
            for (int dz = 0; dz < 2; dz++)
                if ((cornerDensities[dx, 0, dz] > 0) != (cornerDensities[dx, 1, dz] > 0))
                    signChanges.Add(new Vector3(x + dx, y + Adapt(cornerDensities[dx, 0, dz], cornerDensities[dx, 1, dz]), z + dz));

        for (int dy = 0; dy < 2; dy++)
            for (int dz = 0; dz < 2; dz++)
                if ((cornerDensities[0, dy, dz] > 0) != (cornerDensities[1, dy, dz] > 0))
                    signChanges.Add(new Vector3(x + Adapt(cornerDensities[0, dy, dz], cornerDensities[1, dy, dz]), y + dy, z + dz));

        if (signChanges.Count <= 1)
            return Vector3.negativeInfinity;

        // TODO calculate normals

        return vertexSolver.Solve(new Vector3(x, y, z), signChanges, new List<Vector3>());
    }


    private void DualContourMesh(IDensityFunction densityFunction, IVertexPositionSolver vertexSolver, int xmin, int xmax, int ymin, int ymax, int zmin, int zmax)
    {

        List<Vector3> vertexList = new List<Vector3>();
        Dictionary<Vector3, int> vertexIndices = new Dictionary<Vector3, int>();

        for (int x = xmin; x <= xmax; x++)
            for (int y = ymin; y <= ymax; y++)
                for (int z = zmin; z <= zmax; z++)
                {

                    Vector3 vertex = FindBestVertex(densityFunction, vertexSolver, x, y, z);
                    if (vertex == Vector3.negativeInfinity)
                        continue;

                    vertexList.Add(vertex);
                    vertexIndices[vertex] = vertexList.Count - 1;
                }

        List<int> tris = new List<int>();

        for (int x = xmin; x <= xmax; x++)
            for (int y = ymin; y <= ymax; y++)
                for (int z = zmin; z <= zmax; z++)
                {


                }
    }


    private float Adapt(float v0, float v1)
    {

        return (0 - v0) / (v1 - v0);
    }
}
