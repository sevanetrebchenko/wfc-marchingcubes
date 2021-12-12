using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

// Assuming positive x points to the right, positive y points up, positive z points into the screen.
//      6           7
//       o--------o
//      /        /|
//     /        / |
//  4 o--------o 5|
//    |        |  |
//  2 |        |  o 3
//    |        | /
//    |        |/
//    o--------o
//  0           1
public class Cube
{
    private const int Uninitialized = -2;
    private const int AboveTerrain = 1;
    private const int BelowTerrain = -1;
    
    private readonly Vector3Int location_;
    private readonly int[] corners_;

    public int[] GetCorners()
    {
        return corners_;
    }
    
    private int entropy_;
    
    public Cube(Vector3Int location)
    {
        location_ = location;
        corners_ = new int[8];
        entropy_ = IntPow(2, 8); // 2 ^ 8 total possible combinations.
    }

    // Get the total number of combinations this cube has with the given corner configuration.
    public void Update()
    {
        int numUninitialized = 0;

        foreach (int corner in corners_)
        {
            if (corner == Uninitialized)
            {
                ++numUninitialized;
            }
        }
        
        // Each vertex has two possible states.
        entropy_ = IntPow(2, numUninitialized);
    }

    public Vector3Int GetLocation()
    {
        return location_;
    }
    
    public bool IsCollapsed()
    {
        return entropy_ == 0;
    }

    public int GetEntropy()
    {
        return entropy_;
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
            if (corners_[i] == Uninitialized)
            {
                corners_[i] = RandomDouble(-1.0f, 1.0f) < 0.0f ? BelowTerrain : AboveTerrain;
            }
        }
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
    
    private double RandomDouble(double min, double max)
    {
        Random random = new Random();
        return random.NextDouble() * (max - min) + min;
    }
    
}
