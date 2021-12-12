using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex
{
    public const int Uninitialized = -2;
    public const int AboveTerrain = 1;
    public const int BelowTerrain = -1;

    private int value_;

    public Vertex()
    {
        value_ = Uninitialized;
    }

    public int GetValue()
    {
        return value_;
    }

    public void SetValue(int value)
    {
        value_ = value;
    }
    
    public bool IsCollapsed()
    {
        return value_ != Uninitialized;
    }

}
