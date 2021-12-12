using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Cube
{
    public enum Side
    {
        Front,
        Back,
        Right,
        Left,
        Top,
        Bottom
    }
    
    private Vertex[] corners_;
    private int entropy;
    
    public Cube()
    {
        corners_ = new Vertex[8];
        entropy = IntPow(2, 8); // 2 ^ 8 total possible combinations.
    }

    // Get the total number of combinations this cube has with the given corner configuration.
    public void Update()
    {
        int numUninitialized = 0;

        foreach (Vertex corner in corners_)
        {
            if (!corner.IsInitialized)
            {
                ++numUninitialized;
            }
            
            // Clear flags for new collapse iteration.
            corner.Update();
        }
        
        // Each vertex has two possible states.
        entropy = IntPow(2, numUninitialized);
    }

    public bool IsCollapsed()
    {
        return entropy == 0;
    }

    public int GetEntropy()
    {
        return entropy;
    }

    // Picks a random combination for the uninitialized corners of the cube.
    public void Collapse()
    {
        if (IsCollapsed())
        {
            return;
        }

        // Pick random configuration for all uninitialized vertices.
        for (int i = 0; i < 8; ++i)
        {
            Vertex corner = corners_[i];

            if (!corner.IsInitialized)
            {
                // TODO: constraints?
                corner.Initialize();
            }
        }
    }

    // Collapsed cube has all 8 valid corners, need to propagate the result of its collapse to this cube.
    // Returns whether propagation had an effect on the configuration of this cube.
    //      6           7
    //       +--------+
    //      /        /|
    //     /        / |
    //  4 +--------+ 5|
    //    |        |  |
    //  2 |        |  + 3
    //    |        | /
    //    |        |/
    //    +--------+
    //  0           1
    // Note: 'side' is the position of the collapsed cube relative to this one.
    // i.e. collapsed with FRONT propagates to this cube's BACK face.
    public bool Propagate(Cube collapsed, Side side)
    {
        switch (side)
        {
            case Side.Front:
                break;
            case Side.Back:
                break;
            case Side.Right:
                break;
            case Side.Left:
                break;
            case Side.Top:
                break;
            case Side.Bottom:
                break;
        }
        
        return false;
    }

    // Not safe, does not account for negative numbers.
    private int IntPow(int value, int power)
    {
        int result = 1;
        for (int i = 0; i < power; ++i)
        {
            result *= value;
        }

        return result;
    }
    
}
