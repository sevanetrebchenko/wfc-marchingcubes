using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using Random = System.Random;

public class Vertex
{
    private const int Uninitialized = -2;
    private const int AboveTerrain = 1;
    private const int BelowTerrain = -1;

    private int value_;
    private bool isChanged_;

    public int Value
    {
        get => value_;
        set
        {
            value_ = value;
            isChanged_ = true;
        }
    }

    public bool IsDirty => isChanged_;
    public bool IsInitialized => value_ != Uninitialized;

    public Vertex()
    {
        value_ = Uninitialized;
        isChanged_ = false;
    }

    public void Initialize()
    {
        if (IsInitialized)
        {
            return;
        }
        
        Value = RandomDouble(-1.0f, 1.0f) < 0.0 ? BelowTerrain : AboveTerrain;
    }

    public void Update()
    {
        isChanged_ = false;
    }
    
    private double RandomDouble(double min, double max)
    {
        Random random = new Random();
        return random.NextDouble() * (max - min) + min;
    }
    
}