
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    // Dimensions specified in cubes.
    [Range(2, 80)] public int width;
    [Range(2, 80)] public int height;
    [Range(2, 80)] public int depth;

    private int totalNumCubes_;
    
    // Marching cube data.
    private Vector3[] meshVertices_;
    private int numMeshVertices_;

    private GameObject chunk_;
    private MeshFilter meshFilter_;
    private MeshRenderer meshRenderer_;
    
    private void Start()
    {
        // Initialize data.
        chunk_ = new GameObject();
        chunk_.transform.parent = transform;
        chunk_.name = "Terrain Chunk";
        
        meshFilter_ = new MeshFilter();
        meshRenderer_ = new MeshRenderer();
        
        totalNumCubes_ = (width + 1) * (height + 1) * (depth + 1);
        
        // A marching cube configuration can have up to 15 triangles.
        meshVertices_ = new Vector3[totalNumCubes_ * 15];
        numMeshVertices_ = 0;
    }

    private void Update()
    {
        
    }

    private void GenerateMesh()
    {
        
    }
    
    private void ConstructMesh()
    {
        // Initialize mesh triangle indices (no indexing).
        int[] triangles = new int[numMeshVertices_];
        for (int i = 0; i < numMeshVertices_; ++i)
        {
            triangles[i] = i;
        }

        // Create mesh.
        Mesh mesh = new Mesh();
        mesh.vertices = meshVertices_;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        meshFilter_.mesh = mesh;
    }

    private void ResetGeneration()
    {
        meshFilter_.mesh.Clear();
    }
}
