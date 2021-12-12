
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

    private WaveFunction waveFunction_;

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
        
        totalNumCubes_ = (width - 1) * (height - 1) * (depth - 1);
        
        // A marching cube configuration can have up to 15 triangles.
        meshVertices_ = new Vector3[totalNumCubes_ * 15];
        numMeshVertices_ = 0;

        waveFunction_ = new WaveFunction(width - 1, height - 1, depth - 1);
        
        waveFunction_.Run();
        GenerateMesh();
        ConstructMesh();
    }

    private void GenerateMesh()
    {
        int vertexIndex = 0;
        
        // Marching cubes.
        foreach (Cube cube in waveFunction_.GetCubes())
        {
            int configuration = GetCubeConfiguration(cube.GetCorners());
            if (configuration == 0 || configuration == 255)
            {
                continue;
            }

            Vector3 cubeLocation = cube.GetLocation();
            int edgeIndex = 0;
            bool breakOut = false;
            
            // A configuration has maximum 5 triangles in it.
            for (int i = 0; i < 5; ++i) {
                if (breakOut)
                {
                    break;
                }
                
                // A configuration element (triangle) consists of 3 points.
                for (int j = 0; j < 3; ++j) {
                    int triangleIndex = MarchingCubes.TriangleTable[configuration * 16 + edgeIndex];

                    // Reached the end of this configuration.
                    if (triangleIndex == -1)
                    {
                        breakOut = true;
                        break;
                    }

                    int edgeVertex1Index = triangleIndex * 2 + 0;
                    int edgeVertex2Index = triangleIndex * 2 + 1;

                    int corner1Index = MarchingCubes.EdgeTable[edgeVertex1Index] * 3;
                    int corner2Index = MarchingCubes.EdgeTable[edgeVertex2Index] * 3;
                    
                    Vector3 corner1 = new Vector3(MarchingCubes.CornerTable[corner1Index + 0], MarchingCubes.CornerTable[corner1Index + 1], MarchingCubes.CornerTable[corner1Index + 2]);
                    Vector3 corner2 = new Vector3(MarchingCubes.CornerTable[corner2Index + 0], MarchingCubes.CornerTable[corner2Index + 1], MarchingCubes.CornerTable[corner2Index + 2]);
                    
                    Vector3 edgeVertex1 = cubeLocation + corner1;
                    Vector3 edgeVertex2 = cubeLocation + corner2;

                    Vector3 vertexPosition = (edgeVertex1 + edgeVertex2) / 2.0f;

                    meshVertices_[vertexIndex++] = vertexPosition;
                    ++edgeIndex;
                }
            }
        }

        numMeshVertices_ = vertexIndex;
    }

    private int GetCubeConfiguration(int[] cubeCornerValues)
    {
        int configuration = 0;

        for (int i = 0; i < 8; ++i) {
            if (cubeCornerValues[i] > 0) {
                configuration |= 1 << i;
            }
        }
    
        return configuration;
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
