
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

    private Stack<int> propagationStack_;
    
    public WaveFunction(int widthInCubes, int heightInCubes, int depthInCubes)
    {
        width_ = widthInCubes;
        height_ = heightInCubes;
        depth_ = depthInCubes;
        totalNumCubes_ = widthInCubes * heightInCubes * depthInCubes;
        
        cubes_ = new Cube[totalNumCubes_];

        propagationStack_ = new Stack<int>();
    }

    public void Update()
    {
        while (!IsCollapsed())
        {
            Iterate();
        }
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
        UpdateCubes();

        int cubeIndex = GetMinimumEntropyCubeIndex();
        CollapseAt(cubeIndex);
        Propagate(cubeIndex);
    }

    private void UpdateCubes()
    {
        foreach (Cube cube in cubes_)
        {
            cube.Update();
        }
    }

    // Get index of the cube with the lowest possible number of potential combinations (lowest entropy).
    private int GetMinimumEntropyCubeIndex()
    {
        int lowestEntropyIndex = -1;
        int lowestEntropy = Int32.MaxValue;
        
        for (int i = 0; i < totalNumCubes_; ++i)
        {
            Cube cube = cubes_[i];
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
        cubes_[cubeIndex].Collapse();
    }

    private void Propagate(int cubeIndex)
    {
        // Kick-off propagation.
        propagationStack_.Push(cubeIndex);

        while (propagationStack_.Count > 0)
        {
            int index = propagationStack_.Peek();
            Cube cube = cubes_[index];
            
            
        }

        propagationStack_.Clear();
    }

    private Cube GetCube(int x, int y, int z)
    {
        return cubes_[x + z * width_ + y * width_ * depth_];
    }
    
}
