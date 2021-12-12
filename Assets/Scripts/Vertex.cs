using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex
{
    public const int Uninitialized = -2;
    public const int AboveTerrain = 1;
    public const int BelowTerrain = -1;

    private Vector3Int position_;
    
    private int value_;
    private bool final_;

    public Vertex(int x, int y, int z)
    {
        position_ = new Vector3Int(x, y, z);
        
        value_ = Uninitialized;
        final_ = false;
    }

    public Vector3Int GetPosition()
    {
        return position_;
    }
    
    public int GetValue()
    {
        return value_;
    }

    public void TrySetValue(int value)
    {
        if (!IsCollapsed())
        {
            SetValue(value);
        }
    }
    
    public void SetValue(int value)
    {
        if (!final_)
        {
            value_ = value;
        }
    }

    public void SetValueFinal(int value)
    {
        value_ = value;
        final_ = true;
    }
    
    public bool IsCollapsed()
    {
        return value_ != Uninitialized;
    }

}
