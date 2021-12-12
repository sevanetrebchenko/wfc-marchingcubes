using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveFunction
{
    private Cube[] cubes_;

    // Dimension of wave function (in cubes).
    private int width_;
    private int height_;
    private int depth_;
    private int totalNumCubes_;

    public WaveFunction(int widthInCubes, int heightInCubes, int depthInCubes)
    {
        width_ = widthInCubes;
        height_ = heightInCubes;
        depth_ = depthInCubes;
        totalNumCubes_ = widthInCubes * heightInCubes * depthInCubes;

        InitializeCubes();
    }

    private void InitializeCubes()
    {
        cubes_ = new Cube[totalNumCubes_];

        for (int x = 0; x < width_; ++x)
        {
            for (int y = 0; y < height_; ++y)
            {
                for (int z = 0; z < depth_; ++z)
                {
                    int index = GetIndex(x, y, z);
                    cubes_[index] = new Cube(new Vector3Int(x, y, z));
                }
            }
        }
    }

    public void Run()
    {
        do
        {
            Iterate();
        } while (!IsCollapsed());
    }

    public Cube[] GetCubes()
    {
        return cubes_;
    }

    private bool IsCollapsed()
    {
        foreach (Cube cube in cubes_)
        {
            if (!cube.IsCollapsed())
            {
                return false;
            }
        }

        return true;
    }

    private void Iterate()
    {
        int cubeIndex = GetMinimumEntropyCubeIndex();
        CollapseAt(cubeIndex);
        Propagate(cubeIndex);
        
        UpdateCubes();
    }

    private void UpdateCubes()
    {
        foreach (Cube cube in cubes_)
        {
            cube.Update();
        }
    }

    // Get index of the cube with the lowest possible number of potential combinations (lowest entropy) that isn't
    // already collapsed.
    private int GetMinimumEntropyCubeIndex()
    {
        int lowestEntropyIndex = -1;
        int lowestEntropy = Int32.MaxValue;

        for (int i = 0; i < totalNumCubes_; ++i)
        {
            Cube cube = cubes_[i];
            
            if (cube.IsCollapsed())
            {
                continue;
            }
            
            int currentEntropy = cube.GetEntropy();
            if (currentEntropy < lowestEntropy)
            {
                lowestEntropyIndex = i;
                lowestEntropy = currentEntropy;
            }
        }

        return lowestEntropyIndex;
    }

    private void CollapseAt(int cubeIndex)
    {
        Debug.Log("Collapsing cube at index: " + cubeIndex);
        cubes_[cubeIndex].Collapse();
    }

    private void Propagate(int cubeIndex)
    {
        Cube collapsed = cubes_[cubeIndex];
        Vertex[] collapsedCorners = collapsed.GetCorners();
        Vector3Int collapsedLocation = collapsed.GetLocation();

        // For each cube being processed.
        Cube cube;
        Vector3Int location;
        Vertex[] corners;
        
        // Propagation affects the 26 cubes around the collapsed cube (not including the collapsed cube).
        // Bottom row.
        // (-1, -1, -1)
        location = collapsedLocation + new Vector3Int(-1, -1, -1);
        cube = GetCube(location);
        if (cube != null)
        {
            corners = cube.GetCorners();
            corners[7] = collapsedCorners[0];
        }

        // (0, -1, -1)
        location = collapsedLocation + new Vector3Int(0, -1, -1);
        cube = GetCube(location);
        if (cube != null)
        {
            corners = cube.GetCorners();
            corners[6] = collapsedCorners[0];
            corners[7] = collapsedCorners[1];
        }

        // (1, -1, -1)
        location = collapsedLocation + new Vector3Int(1, -1, -1);
        cube = GetCube(location);
        if (cube != null)
        {
            corners = cube.GetCorners();
            corners[6] = collapsedCorners[1];
        }


        // (-1, -1, 0)
        location = collapsedLocation + new Vector3Int(-1, -1, 0);
        cube = GetCube(location);
        if (cube != null)
        {
            corners = cube.GetCorners();
            corners[5] = collapsedCorners[0];
            corners[7] = collapsedCorners[2];
        }

        // (0, -1, 0)
        location = collapsedLocation + new Vector3Int(0, -1, 0);
        cube = GetCube(location);
        if (cube != null)
        {
            corners = cube.GetCorners();
            corners[4] = collapsedCorners[0];
            corners[5] = collapsedCorners[1];
            corners[6] = collapsedCorners[2];
            corners[7] = collapsedCorners[3];
        }

        // (1, -1, 0)
        location = collapsedLocation + new Vector3Int(1, -1, 0);
        cube = GetCube(location);
        if (cube != null)
        {
            corners = cube.GetCorners();
            corners[4] = collapsedCorners[1];
            corners[6] = collapsedCorners[3];
        }


        // (-1, -1, 1)
        location = collapsedLocation + new Vector3Int(-1, -1, 1);
        cube = GetCube(location);
        if (cube != null)
        {
            corners = cube.GetCorners();
            corners[5] = collapsedCorners[2];
        }

        // (0, -1, 1)
        location = collapsedLocation + new Vector3Int(0, -1, 1);
        cube = GetCube(location);
        if (cube != null)
        {
            corners = cube.GetCorners();
            corners[4] = collapsedCorners[2];
            corners[5] = collapsedCorners[3];
        }

        // (1, -1, 1)
        location = collapsedLocation + new Vector3Int(1, -1, 1);
        cube = GetCube(location);
        if (cube != null)
        {
            corners = cube.GetCorners();
            corners[4] = collapsedCorners[3];
        }
        
        // Middle row.
        // (-1, 0, -1)
        location = collapsedLocation + new Vector3Int(-1, 0, -1);
        cube = GetCube(location);
        if (cube != null)
        {
            corners = cube.GetCorners();
            corners[3] = collapsedCorners[0];
            corners[7] = collapsedCorners[4];
        }

        // (0, 0, -1)
        location = collapsedLocation + new Vector3Int(0, 0, -1);
        cube = GetCube(location);
        if (cube != null)
        {
            corners = cube.GetCorners();
            corners[2] = collapsedCorners[0];
            corners[3] = collapsedCorners[1];
            corners[6] = collapsedCorners[4];
            corners[7] = collapsedCorners[5];
        }

        // (1, 0, -1)
        location = collapsedLocation + new Vector3Int(1, 0, -1);
        cube = GetCube(location);
        if (cube != null)
        {
            corners = cube.GetCorners();
            corners[2] = collapsedCorners[1];
            corners[6] = collapsedCorners[5];
        }


        // (-1, 0, 0)
        location = collapsedLocation + new Vector3Int(-1, 0, 0);
        cube = GetCube(location);
        if (cube != null)
        {
            corners = cube.GetCorners();
            corners[1] = collapsedCorners[0];
            corners[3] = collapsedCorners[2];
            corners[5] = collapsedCorners[4];
            corners[7] = collapsedCorners[6];
        }

        // (0, 0, 0)
        // No point in propagating to collapsed cube.
        // location = collapsedLocation + new Vector3Int(-1, -1, -1);
        // cube = GetCube(location);
        // if (cube != null)
        // {
        //     corners = cube.GetCorners();
        //     corners[7] = collapsedCorners[0];
        // }

        // (1, 0, 0)
        location = collapsedLocation + new Vector3Int(1, 0, 0);
        cube = GetCube(location);
        if (cube != null)
        {
            corners = cube.GetCorners();
            corners[0] = collapsedCorners[1];
            corners[2] = collapsedCorners[3];
            corners[4] = collapsedCorners[5];
            corners[6] = collapsedCorners[7];
        }


        // (-1, 0, 1)
        location = collapsedLocation + new Vector3Int(-1, 0, 1);
        cube = GetCube(location);
        if (cube != null)
        {
            corners = cube.GetCorners();
            corners[1] = collapsedCorners[2];
            corners[5] = collapsedCorners[6];
        }

        // (0, 0, 1)
        location = collapsedLocation + new Vector3Int(0, 0, 1);
        cube = GetCube(location);
        if (cube != null)
        {
            corners = cube.GetCorners();
            corners[0] = collapsedCorners[2];
            corners[1] = collapsedCorners[3];
            corners[4] = collapsedCorners[6];
            corners[5] = collapsedCorners[7];
        }

        // (1, 0, 1)
        location = collapsedLocation + new Vector3Int(1, 0, 1);
        cube = GetCube(location);
        if (cube != null)
        {
            corners = cube.GetCorners();
            corners[0] = collapsedCorners[3];
            corners[4] = collapsedCorners[7];
        }
        
        // Top row.
        // (-1, 1, -1)
        location = collapsedLocation + new Vector3Int(-1, 1, -1);
        cube = GetCube(location);
        if (cube != null)
        {
            corners = cube.GetCorners();
            corners[3] = collapsedCorners[4];
        }

        // (0, 1, -1)
        location = collapsedLocation + new Vector3Int(0, 1, -1);
        cube = GetCube(location);
        if (cube != null)
        {
            corners = cube.GetCorners();
            corners[2] = collapsedCorners[4];
            corners[3] = collapsedCorners[5];
        }

        // (1, 1, -1)
        location = collapsedLocation + new Vector3Int(1, 1, -1);
        cube = GetCube(location);
        if (cube != null)
        {
            corners = cube.GetCorners();
            corners[2] = collapsedCorners[5];
        }

        
        // (-1, 1, 0)
        location = collapsedLocation + new Vector3Int(-1, 1, 0);
        cube = GetCube(location);
        if (cube != null)
        {
            corners = cube.GetCorners();
            corners[1] = collapsedCorners[4];
            corners[3] = collapsedCorners[6];
        }

        // (0, 1, 0)
        location = collapsedLocation + new Vector3Int(0, 1, 0);
        cube = GetCube(location);
        if (cube != null)
        {
            corners = cube.GetCorners();
            corners[0] = collapsedCorners[4];
            corners[1] = collapsedCorners[5];
            corners[2] = collapsedCorners[6];
            corners[3] = collapsedCorners[7];
        }

        // (1, 1, 0)
        location = collapsedLocation + new Vector3Int(1, 1, 0);
        cube = GetCube(location);
        if (cube != null)
        {
            corners = cube.GetCorners();
            corners[0] = collapsedCorners[5];
            corners[2] = collapsedCorners[7];
        }


        // (-1, 1, 1)
        location = collapsedLocation + new Vector3Int(-1, 1, 1);
        cube = GetCube(location);
        if (cube != null)
        {
            corners = cube.GetCorners();
            corners[1] = collapsedCorners[6];
        }

        // (0, 1, 1)
        location = collapsedLocation + new Vector3Int(0, 1, 1);
        cube = GetCube(location);
        if (cube != null)
        {
            corners = cube.GetCorners();
            corners[0] = collapsedCorners[6];
            corners[1] = collapsedCorners[7];
        }

        // (1, 1, 1)
        location = collapsedLocation + new Vector3Int(1, 1, 1);
        cube = GetCube(location);
        if (cube != null)
        {
            corners = cube.GetCorners();
            corners[0] = collapsedCorners[7];
        }
    }

    private Cube GetCube(Vector3Int location)
    {
        return GetCube(location.x, location.y, location.z);
    }

    private Cube GetCube(int x, int y, int z)
    {
        int index = GetIndex(x, y, z);
        if (index < 0)
        {
            return null;
        }

        return cubes_[index];
    }

    private int GetIndex(Vector3Int location)
    {
        return GetIndex(location.x, location.y, location.z);
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
}