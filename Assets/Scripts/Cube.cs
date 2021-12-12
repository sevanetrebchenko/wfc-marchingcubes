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
    private readonly Vector3Int location_;
    private readonly Vertex[] corners_;

    private int entropy_;
    private Random random_;
    
    public Cube(Vector3Int location)
    {
        location_ = location;
        corners_ = new Vertex[8];

        for (int i = 0; i < 8; ++i)
        {
            corners_[i] = new Vertex();
        }
        
        entropy_ = IntPow(2, 8); // 2 ^ 8 total possible combinations.
        random_ = new Random();
    }

    // Get the total number of combinations this cube has with the given corner configuration.
    public void Update()
    {
        int numUninitialized = 0;

        foreach (Vertex corner in corners_)
        {
            if (!corner.IsCollapsed())
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
        return entropy_ == 1; // 2 ^ 0 is 1.
    }

    public int GetEntropy()
    {
        return entropy_;
    }
    
    public Vertex[] GetCorners()
    {
        return corners_;
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
            if (!corner.IsCollapsed())
            {
                corner.SetValue(HeightGenerator.GetRandomHeight(location_.x, location_.y, location_.z));
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
    
}
