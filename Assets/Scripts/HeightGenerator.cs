using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using Unity.Mathematics;
using UnityEngine;
using Random = System.Random;

public static class HeightGenerator
{
    private static int numNoiseOctaves_;
    private static float noiseScale_;
    private static float persistence_;
    private static float lacunarity_;
    private static Random seededGenerator_;
    private static Vector3[] octaveOffsets;

    static HeightGenerator()
    {
        numNoiseOctaves_ = 3;
        noiseScale_ = 20.0f;
        persistence_ = 0.3f;
        lacunarity_ = 1.4f;
        seededGenerator_ = new Random();

        octaveOffsets = new Vector3[numNoiseOctaves_];
        
        for (int i = 0; i < numNoiseOctaves_; ++i)
        {
            octaveOffsets[i] = new Vector3(RandomDouble(-10000, 10000), RandomDouble(-10000, 10000), RandomDouble(-10000, 10000));
        }
    }

    public static int GetRandomHeight(int x, int y, int z)
    {
        float height = 0.0f;
        
        float amplitude = 1.0f;
        float frequency = 1.0f;

        for (int i = 0; i < numNoiseOctaves_; ++i)
        {
            float sampleX = (x + octaveOffsets[i].x) / noiseScale_ * frequency;
            float sampleY = (y + octaveOffsets[i].y) / noiseScale_ * frequency;
            float sampleZ = (z + octaveOffsets[i].z) / noiseScale_ * frequency;

            // Get perlin values from -1 to 1
            float current = noise.cnoise(new float3(sampleX, sampleY, sampleZ));// - 0.5f) * 2.0f;
            height += current * amplitude;

            amplitude *= persistence_; // Persistence should be between 0 and 1 - amplitude decreases with each octave.
            frequency *= lacunarity_;  // Lacunarity should be greater than 1 - frequency increases with each octave.
        }

        return height < 0.0f ? Vertex.BelowTerrain : Vertex.AboveTerrain;
    }

    private static float RandomDouble(double min, double max)
    {
        return (float)(seededGenerator_.NextDouble() * (max - min) + min);
    }
    
}
