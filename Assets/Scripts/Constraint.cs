using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public abstract class Constraint
{
    public abstract void Constrain(Cube target, WaveFunction waveFunction);

    protected void Noise(Cube target)
    {
        Vector3Int location = target.GetLocation();
        Vertex[] corners = target.GetCorners();
        
        // Pick random configuration for all uninitialized vertices.
        for (int i = 0; i < 8; ++i)
        {
            Vertex corner = corners[i];
            corner.TrySetValue(HeightGenerator.GetRandomHeight(location));
        }
    }
}

public class RandomNoise : Constraint
{
    public override void Constrain(Cube target, WaveFunction waveFunction)
    {
        Noise(target);
    }
}

public class Flatten : Constraint
{
    public override void Constrain(Cube target, WaveFunction waveFunction)
    {
        Vector3Int location = target.GetLocation();
        Vertex[] corners = target.GetCorners();

        bool hasNonFlatTerrain = false;
        
        for (int x = -1; x <= 1; ++x)
        {
            for (int z = -1; z <= 1; ++z)
            {
                Cube cube = waveFunction.GetCube(location.x + x, location.y, location.z + z);

                if (cube == null)
                {
                    continue;
                }
                
                if (x == 0 && z == 0)
                {
                    continue;
                }

                if (!cube.IsCollapsed())
                {
                    continue;
                }

                if (!IsConfigurationFlat(cube.GetCorners()))
                {
                    hasNonFlatTerrain = true;
                    break;
                }
            }
        }

        if (hasNonFlatTerrain)
        {
            // Generate flat terrain.
            corners[0].SetValue(Vertex.BelowTerrain);
            corners[1].SetValue(Vertex.BelowTerrain);
            corners[2].SetValue(Vertex.BelowTerrain);
            corners[3].SetValue(Vertex.BelowTerrain);
            
            corners[4].SetValue(Vertex.AboveTerrain);
            corners[5].SetValue(Vertex.AboveTerrain);
            corners[6].SetValue(Vertex.AboveTerrain);
            corners[7].SetValue(Vertex.AboveTerrain);
        }
    }

    private bool IsConfigurationFlat(Vertex[] corners)
    {
        return corners[0].GetValue() == (Vertex.BelowTerrain) &&
               corners[1].GetValue() == (Vertex.BelowTerrain) &&
               corners[2].GetValue() == (Vertex.BelowTerrain) &&
               corners[3].GetValue() == (Vertex.BelowTerrain) &&
               corners[4].GetValue() == (Vertex.AboveTerrain) &&
               corners[5].GetValue() == (Vertex.AboveTerrain) &&
               corners[6].GetValue() == (Vertex.AboveTerrain) &&
               corners[7].GetValue() == (Vertex.AboveTerrain);
    }
}

public class NoOverhang : Constraint
{
    public override void Constrain(Cube target, WaveFunction waveFunction)
    {
        Vector3Int location = target.GetLocation();
        Vertex[] corners = target.GetCorners();

        bool terrainBelow = false;
        bool terrainAbove = false;

        for (int y = 0; y < waveFunction.GetHeight(); ++y)
        {
            Cube cube = waveFunction.GetCube(location.x, y, location.z);
            if (cube == null)
            {
                continue;
            }

            if (!cube.IsCollapsed())
            {
                continue;
            }

            if (y == location.y)
            {
                continue;
            }

            if (y < location.y)
            {
                if (cube.HasTerrain())
                {
                    terrainBelow = true;
                    break;
                }
            }
            else
            {
                if (cube.HasTerrain())
                {
                    terrainAbove = true;
                    break;
                }
            }
        }

        if (terrainBelow)
        {
            // Terrain below, no more terrain is allowed above the point.
            // All node values are 'air'.
            corners[0].SetValue(Vertex.AboveTerrain);
            corners[1].SetValue(Vertex.AboveTerrain);
            corners[2].SetValue(Vertex.AboveTerrain);
            corners[3].SetValue(Vertex.AboveTerrain);
            corners[4].SetValue(Vertex.AboveTerrain);
            corners[5].SetValue(Vertex.AboveTerrain);
            corners[6].SetValue(Vertex.AboveTerrain);
            corners[7].SetValue(Vertex.AboveTerrain);
        }
        else if (terrainAbove)
        {
            // Terrain above, all terrain below needs to be 'ground'.
            corners[0].SetValue(Vertex.BelowTerrain);
            corners[1].SetValue(Vertex.BelowTerrain);
            corners[2].SetValue(Vertex.BelowTerrain);
            corners[3].SetValue(Vertex.BelowTerrain);
            corners[4].SetValue(Vertex.BelowTerrain);
            corners[5].SetValue(Vertex.BelowTerrain);
            corners[6].SetValue(Vertex.BelowTerrain);
            corners[7].SetValue(Vertex.BelowTerrain);
        }
    }
}

public class GapFill : Constraint
{
    public override void Constrain(Cube target, WaveFunction waveFunction)
    {
        Vector3Int location = target.GetLocation();
        Vertex[] corners = target.GetCorners();
    }
}