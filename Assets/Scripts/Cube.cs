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
    private Vertex[] corners_;

    private int entropy_;
    private Random random_;
    private Constraint constraint_;
    
    public Cube(Vector3Int location)
    {
        location_ = location;

        InitializeVertices();

        entropy_ = IntPow(2, 8); // 2 ^ 8 total possible combinations.
        random_ = new Random();

        constraint_ = new RandomNoise();
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

    public bool HasTerrain()
    {
        if (!IsCollapsed())
        {
            return false;
        }

        foreach (Vertex corner1 in corners_)
        {
            foreach (Vertex corner2 in corners_)
            {
                if (corner1.GetValue() != corner2.GetValue())
                {
                    // Difference in terrain values means there exists a surface.
                    return true;
                }
            }
        }
        
        return false;
    }

    public void SetConstraint(Constraint constraint)
    {
        constraint_ = constraint;
    }

    public Constraint GetConstraint()
    {
        return constraint_;
    }

    private void InitializeVertices()
    {
        corners_ = new Vertex[8];
        corners_[0] = new Vertex(0, 0, 0);
        corners_[1] = new Vertex(1, 0, 0);
        corners_[2] = new Vertex(0, 0, 1);
        corners_[3] = new Vertex(1, 0, 1);
        corners_[4] = new Vertex(0, 1, 0);
        corners_[5] = new Vertex(1, 1, 0);
        corners_[6] = new Vertex(0, 1, 1);
        corners_[7] = new Vertex(1, 1, 1);
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
