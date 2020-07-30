using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceNetSolver : IVertexPositionSolver
{

    public Vector3 Solve(Vector3 position, List<Vector3> signChanges, List<Vector3> normals)
    {

        Vector3 vertexPosition = Vector3.zero;

        foreach (Vector3 signChange in signChanges)
            vertexPosition += signChange;

        return vertexPosition / signChanges.Count;
    }
}
