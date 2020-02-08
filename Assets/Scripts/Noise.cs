using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise {
    
    public static float CaveNoise(int x, int z, float scale, float frequency, float amplitude, int octaves, int seed) {
        float total = 0;
        for (int i = 0; i < octaves; i++) {
            float sampleX = x / scale * frequency;
            float sampleY = z / scale * frequency;
            float perlinValue = Mathf.PerlinNoise(sampleX + seed, sampleY + seed);
            perlinValue = perlinValue * 2 - 1;
            total += perlinValue * amplitude;
            amplitude *= 2;
            frequency *= 0.5f;
        }
        return total;
    }

}
