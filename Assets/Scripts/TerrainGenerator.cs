
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    // Dimensions specified in cubes.
    [Range(2, 80)] public int width;
    [Range(2, 80)] public int height;
    [Range(2, 80)] public int depth;

    public GameObject camera;
    
    // Allow for a buffer of 'air' nodes to generate terrain without gaps.
    private int width_;
    private int height_;
    private int depth_;
    
    private int totalNumNodes_;
    private int totalNumCubes_;
    
    // Marching cube data.
    private Vector3[] meshVertices_;
    private int numMeshVertices_;

    private WaveFunction waveFunction_;
    private int[] heightMap_;
    
    private GameObject chunk_;

    private void Start()
    {
        // Initialize data.
        chunk_ = new GameObject
        {
            transform =
            {
                parent = transform
            },
            name = "Terrain Chunk"
        };

        chunk_.AddComponent<MeshFilter>();
        chunk_.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Diffuse"));
        
        // Allow for a buffer of air on either side of the generated mesh.
        width_ = width + 2;
        height_ = height + 2;
        depth_ = depth + 2;

        totalNumNodes_ = width_ * height_ * depth_;
        totalNumCubes_ = (width_ - 1) * (height_ - 1) * (depth_ - 1);

        camera.GetComponent<Transform>().LookAt(new Vector3(width_ / 2, height_ / 2 - 3, depth_ / 2));
        
        Generate();
    }

    private void Update()
    {
        chunk_.transform.RotateAround(new Vector3(width / 2, height / 2, depth / 2), new Vector3(0, 1, 0), 15.0f * Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        Vector3Int offset = new Vector3Int(1, 1, 1);
        foreach (Cube cube in waveFunction_.GetCubes())
        {
            // Using diagram from Cube.cs for vertex locations.
            Vector3 location = cube.GetLocation() + offset;
            Vertex[] corners = cube.GetCorners();
            
            // Bottom row.
            DrawSphere(location + new Vector3(0.0f, 0.0f, 0.0f), corners[0]);
            DrawSphere(location + new Vector3(1.0f, 0.0f, 0.0f), corners[1]);
            DrawSphere(location + new Vector3(0.0f, 0.0f, 1.0f), corners[2]);
            DrawSphere(location + new Vector3(1.0f, 0.0f, 1.0f), corners[3]);
            
            // Top row.
            DrawSphere(location + new Vector3(0.0f, 1.0f, 0.0f), corners[4]);
            DrawSphere(location + new Vector3(1.0f, 1.0f, 0.0f), corners[5]);
            DrawSphere(location + new Vector3(0.0f, 1.0f, 1.0f), corners[6]);
            DrawSphere(location + new Vector3(1.0f, 1.0f, 1.0f), corners[7]);
        }
    }

    private void DrawSphere(Vector3 location, Vertex corner)
    {
        bool belowTerrain = corner.GetValue() == Vertex.BelowTerrain;
        Gizmos.color = belowTerrain ? Color.black : Color.white;
        Gizmos.DrawSphere(location, 0.1f);
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(6, 20, 500, 20), "Constraints being applied:");
        GUI.Label(new Rect(6, 35, 500, 20), "1.  Pseudo-Random Height Generation");
        GUI.Label(new Rect(6, 50, 500, 20), "2.  Fixed Initial Conditions (for floor and statue)");
        GUI.Label(new Rect(6, 65, 500, 20), "3.  No Overhangs (over statue)");
        
        GUI.Label(new Rect(6, 90, 500, 20), "Roland Munguia, Seva Netrebchenko");
        
        if (GUILayout.Button("Regenerate"))
        {
            Generate();
        }
    }
    
    // Function is called before any collapsing of the function happens.
    // Responsible for setting ALL initial conditions for the algorithm to work with.
    void InitialConditions()
    {
        // Floor needs to be filled.
        for (int x = 0; x < waveFunction_.GetWidth(); ++x)
        {
            for (int z = 0; z < waveFunction_.GetDepth(); ++z)
            {
                Cube cube = waveFunction_.GetCube(x, 0, z);
                Vertex[] corners = cube.GetCorners();

                for (int i = 0; i < 8; ++i)
                {
                    corners[i].SetValueFinal(Vertex.BelowTerrain);
                }
            }
        }

        int offset = 2;
        int xStart = (width - 1) / 2 - offset;
        int xEnd = (width - 1) / 2 + offset;
        
        int zStart = (depth - 1) / 2 - offset;
        int zEnd = (depth - 1) / 2 + offset;
        
        
        for (int x = xStart - 1; x <= xEnd + 1; ++x)
        {
            for (int y = 1; y < waveFunction_.GetHeight(); ++y)
            {
                for (int z = zStart - 1; z <= zEnd + 1; ++z)
                {
                    Cube cube = waveFunction_.GetCube(x, y, z);
                    Vertex[] corners = cube.GetCorners();
    
                    for (int i = 0; i < 8; ++i)
                    {
                        corners[i].SetValueFinal(Vertex.AboveTerrain);
                    }
                }
            }
        }
        
        
        for (int x = xStart; x <= xEnd; ++x)
        {
            for (int y = 1; y < 3; ++y)
            {
                for (int z = zStart; z <= zEnd; ++z)
                {
                    Cube cube = waveFunction_.GetCube(x, y, z);
                    Vertex[] corners = cube.GetCorners();
    
                    for (int i = 0; i < 8; ++i)
                    {
                        corners[i].SetValueFinal(Vertex.BelowTerrain);
                    }
                }
            }
        }
        
        for (int y = 0; y < 5; ++y)
        {
            Cube cube = waveFunction_.GetCube((width - 1) / 2, y, (depth - 1) / 2);
            Vertex[] corners = cube.GetCorners();

            for (int i = 0; i < 8; ++i)
            {
                corners[i].SetValueFinal(Vertex.BelowTerrain);
            }
        }
        
        for (int x = xStart; x < xEnd; ++x)
        {
            for (int y = 0; y < waveFunction_.GetHeight(); ++y)
            {
                for (int z = zStart; z < zEnd; ++z)
                {
                    Cube cube = waveFunction_.GetCube(x, y, z);
                    cube.SetConstraint(new NoOverhang());
                }
            }
        }
    }

    private void GenerateHeightMap()
    {
        // Initialize all nodes as air nodes.
        for (int x = 0; x < width_; ++x)
        {
            for (int y = 0; y < height_; ++y)
            {
                for (int z = 0; z < depth_; ++z)
                {
                    int index = GetIndex(x, y, z);
                    heightMap_[index] = Vertex.AboveTerrain;
                }
            }
        }
        
        // Copy over heights generated by wave function collapse algorithm.
        Vector3Int offset = new Vector3Int(1, 1, 1); // Offset into each axis (to avoid buffer 'air' nodes.
        foreach (Cube cube in waveFunction_.GetCubes())
        {
            Vector3Int location = cube.GetLocation() + offset;
            Vertex[] corners = cube.GetCorners();

            // Marching cubes algorithm expects a specific data layout.
            heightMap_[GetIndex(location.x + 0, location.y + 0, location.z + 0)] = corners[0].GetValue();
            heightMap_[GetIndex(location.x + 1, location.y + 0, location.z + 0)] = corners[1].GetValue();
            heightMap_[GetIndex(location.x + 1, location.y + 1, location.z + 0)] = corners[5].GetValue();
            heightMap_[GetIndex(location.x + 0, location.y + 1, location.z + 0)] = corners[4].GetValue();
            heightMap_[GetIndex(location.x + 0, location.y + 0, location.z + 1)] = corners[2].GetValue();
            heightMap_[GetIndex(location.x + 1, location.y + 0, location.z + 1)] = corners[3].GetValue();
            heightMap_[GetIndex(location.x + 1, location.y + 1, location.z + 1)] = corners[7].GetValue();
            heightMap_[GetIndex(location.x + 0, location.y + 1, location.z + 1)] = corners[6].GetValue();
        }
    }
    
    private void GenerateMesh()
    {
        int[] cubeCornerValues = new int[8];
        
        // Marching cubes over the entire height map.
        for (int x = 0; x < width_ - 1; ++x)
        {
            for (int y = 0; y < height_ - 1; ++y)
            {
                for (int z = 0; z < depth_ - 1; ++z)
                {
                    // Get 'cube' worth of height map data.
                    Vector3Int location = new Vector3Int(x, y, z);
                    cubeCornerValues[0] = heightMap_[GetIndex(x + 0, y + 0, z + 0)];
                    cubeCornerValues[1] = heightMap_[GetIndex(x + 1, y + 0, z + 0)];
                    cubeCornerValues[2] = heightMap_[GetIndex(x + 1, y + 1, z + 0)];
                    cubeCornerValues[3] = heightMap_[GetIndex(x + 0, y + 1, z + 0)];
                    cubeCornerValues[4] = heightMap_[GetIndex(x + 0, y + 0, z + 1)];
                    cubeCornerValues[5] = heightMap_[GetIndex(x + 1, y + 0, z + 1)];
                    cubeCornerValues[6] = heightMap_[GetIndex(x + 1, y + 1, z + 1)];
                    cubeCornerValues[7] = heightMap_[GetIndex(x + 0, y + 1, z + 1)];
                    
                    int configuration = GetCubeConfiguration(cubeCornerValues);
                    if (configuration == 0 || configuration == 255)
                    {
                        continue;
                    }

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
                    
                            Vector3 edgeVertex1 = location + corner1;
                            Vector3 edgeVertex2 = location + corner2;

                            meshVertices_[numMeshVertices_++] = (edgeVertex1 + edgeVertex2) / 2.0f;
                            ++edgeIndex;
                        }
                    }
                }
            }
        }
    }

    private int GetCubeConfiguration(int[] heightValues)
    {
        int configuration = 0;

        for (int i = 0; i < 8; ++i) {
            if (heightValues[i] == Vertex.AboveTerrain) {
                configuration |= 1 << i;
            }
        }
    
        return configuration;
    }
    
    private void ConstructMesh()
    {
        // Initialize mesh triangle indices (no indexing).
        Vector3[] vertices = new Vector3[numMeshVertices_];
        int[] triangles = new int[numMeshVertices_];

        for (int i = 0; i < numMeshVertices_; ++i)
        {
            vertices[i] = meshVertices_[i];
            triangles[i] = i;
        }

        // Create mesh.
        Mesh mesh = new Mesh
        {
            vertices = vertices,
            triangles = triangles
        };
        mesh.RecalculateNormals();

        chunk_.GetComponent<MeshFilter>().mesh = mesh;
        
        // Debug.Log("Constructed mesh with " + numMeshVertices_ + " vertices");
    }
    
    private int GetIndex(int x, int y, int z)
    {
        if ((x < 0 || x >= width_) || (y < 0 || y >= height_) || (z < 0 || z >= depth_))
        {
            // Invalid index.
            return -1;
        }
        
        return x + z * width_ + y * width_ * depth_;
    }

    private void Generate()
    {
        // A marching cube configuration can have up to 15 triangles.
        meshVertices_ = new Vector3[totalNumCubes_ * 15];
        heightMap_ = new int[totalNumNodes_];
        numMeshVertices_ = 0;

        // Generate internal mesh.
        waveFunction_ = new WaveFunction(width - 1, height - 1, depth - 1);

        HeightGenerator.Refresh();
        
        InitialConditions();
        waveFunction_.Run();

        GenerateHeightMap();
        GenerateMesh();
        ConstructMesh();
    }
    
}
