using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVertexPositionSolver
{

    Vector3 Solve(Vector3 position, List<Vector3> signChanges, List<Vector3> normals);
}
