using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DualContouring : MonoBehaviour
{

    MeshRenderer meshRenderer = null;
    MeshFilter meshFilter = null;

    private void Awake()
    {

        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();

        IDensityFunction densityFunction = new TerrainFunction();
        IVertexPositionSolver surfaceNetSolver = new SurfaceNetSolver();
        DualContourMesh(densityFunction, surfaceNetSolver, 0, 100, 0, 100, 0, 100);
    }


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
            return Vector3.one*1000;

        // TODO calculate normals

        return vertexSolver.Solve(new Vector3(x, y, z), signChanges, new List<Vector3>());
    }


    private void DualContourMesh(IDensityFunction densityFunction, IVertexPositionSolver vertexSolver, int xmin, int xmax, int ymin, int ymax, int zmin, int zmax)
    {

        List<Vector3> vertexList = new List<Vector3>();
        Dictionary<Vector3Int, int> vertexIndices = new Dictionary<Vector3Int, int>();

        for (int x = xmin; x <= xmax; x++)
            for (int y = ymin; y <= ymax; y++)
                for (int z = zmin; z <= zmax; z++)
                {

                    Vector3 vertex = FindBestVertex(densityFunction, vertexSolver, x, y, z);
                    if (vertex != Vector3.one * 1000)
                    {
                        vertexList.Add(vertex);
                        vertexIndices[new Vector3Int(x, y, z)] = vertexList.Count - 1;
                    }
                }

        List<int> tris = new List<int>();

        for (int x = xmin; x <= xmax; x++)
            for (int y = ymin; y <= ymax; y++)
                for (int z = zmin; z <= zmax; z++)
                {

                    if (x > xmin && y > ymin)
                    {

                        bool solid1 = densityFunction.Density(x, y, z + 0) > 0;
                        bool solid2 = densityFunction.Density(x, y, z + 1) > 0;

                        if (solid1 != solid2)
                        {

                            Quad face = new Quad(vertexIndices[new Vector3Int(x - 1, y - 1, z)],
                                                 vertexIndices[new Vector3Int(x - 0, y - 1, z)],
                                                 vertexIndices[new Vector3Int(x - 0, y - 0, z)],
                                                 vertexIndices[new Vector3Int(x - 1, y - 0, z)]);

                            if (solid2)
                                face.Swap();

                            int[] quadIndexes = face.TriangleIndices;

                            foreach (int i in quadIndexes)
                                tris.Add(i);
                        }
                    }

                    if (x > xmin && z > zmin)
                    {

                        bool solid1 = densityFunction.Density(x, y + 0, z) > 0;
                        bool solid2 = densityFunction.Density(x, y + 1, z) > 0;

                        if (solid1 != solid2)
                        {

                            Quad face = new Quad(vertexIndices[new Vector3Int(x - 1, y, z - 1)],
                                                 vertexIndices[new Vector3Int(x - 0, y, z - 1)],
                                                 vertexIndices[new Vector3Int(x - 0, y, z - 0)],
                                                 vertexIndices[new Vector3Int(x - 1, y, z - 0)]);

                            if (solid1)
                                face.Swap();

                            int[] quadIndexes = face.TriangleIndices;

                            foreach (int i in quadIndexes)
                                tris.Add(i);
                        }
                    }

                    if (y > ymin && z > zmin)
                    {

                        bool solid1 = densityFunction.Density(x + 0, y, z) > 0;
                        bool solid2 = densityFunction.Density(x + 1, y, z) > 0;

                        if (solid1 != solid2)
                        {

                            Quad face = new Quad(vertexIndices[new Vector3Int(x, y - 1, z - 1)],
                                                 vertexIndices[new Vector3Int(x, y - 0, z - 1)],
                                                 vertexIndices[new Vector3Int(x, y - 0, z - 0)],
                                                 vertexIndices[new Vector3Int(x, y - 1, z - 0)]);

                            if (solid2)
                                face.Swap();

                            int[] quadIndexes = face.TriangleIndices;

                            foreach (int i in quadIndexes)
                                tris.Add(i);
                        }
                    }
                }

        MeshData meshData = new MeshData(vertexList.ToArray(), tris.ToArray());
        Mesh mesh = meshData.CreateMesh();

        meshFilter.mesh = mesh;
        print("done");
    }


    private float Adapt(float v0, float v1)
    {

        return (0 - v0) / (v1 - v0);
    }
}


public struct Quad
{

    int v1, v2, v3, v4;

    public Quad(int v1, int v2, int v3, int v4)
    {

        this.v1 = v1;
        this.v2 = v2;
        this.v3 = v3;
        this.v4 = v4;
    }

    public void Swap()
    {

        Quad temp = new Quad(this.v1, this.v2, this.v3, this.v4);

        v4 = temp.v1;
        v3 = temp.v2;
        v2 = temp.v3;
        v1 = temp.v4;
    }

    public int[] TriangleIndices
    {
        get
        {

            return new int[6] {
                v3, v2, v1,
                v1, v4, v3
            };
        }
    }
}


    
