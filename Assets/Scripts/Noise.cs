using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise {
    
    /// <summary>
    /// Generates a layered noise value based on a position in space
    /// </summary>
    /// <param name="x">x sample point</param>
    /// <param name="z">z sample point</param>
    /// <param name="scale">How large the noise is related to the positions</param>
    /// <param name="frequency">How frequent the noise is at the given point</param>
    /// <param name="amplitude">How strong the noise is at the given point</param>
    /// <param name="octaves">The number of octaves the noise is sampled by</param>
    /// <param name="seed">A unique seed</param>
    /// <returns>A float between +amplitude and -amplitude</returns>
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
