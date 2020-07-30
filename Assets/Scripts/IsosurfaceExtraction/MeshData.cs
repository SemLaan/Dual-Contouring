using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MeshData
{

    public Vector3[] vertices;
    public int[] triangles;

    public MeshData(Vector3[] vertices, int[] triangles)
    {

        this.vertices = vertices;
        this.triangles = triangles;
    }

    public Mesh CreateMesh()
    {

        Mesh mesh = new Mesh();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }
}
