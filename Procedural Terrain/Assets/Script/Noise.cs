using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public static float[,] GenerateNoiseMap(bool gradientbool, int mapWidth, int mapHeight, int seed, float scale, float frequency, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        System.Random ping = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = ping.Next(-100000, 100000) + offset.x;
            float offsetY = ping.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }
        if (scale <= 0)
        {
            scale = 0.0001f;
        }
        float maxNoiseHeight = float.MinValue;
        float minNoiseHegiht = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;


        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float amplitude = 1;
                /*float frequency = 0.1f;*/
                float noiseHeight = 0;


                for (int i = 0; i < octaves; i++)
                {
                    float smapleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
                    float smapleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;

                    float perlineValue = Mathf.PerlinNoise(smapleX, smapleY) * 2 - 1;
                    /*float perlineValue = (float)(ping.NextDouble() * 2 - 1);*/

                    noiseHeight += perlineValue * amplitude;
                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                //Gradient Noise
                float distanceFromCenter = Vector2.Distance(new Vector2(x, y), new Vector2(halfWidth,halfHeight));
                float maxDistance = Mathf.Max(halfWidth, halfHeight);
                float gradient = Mathf.Clamp01(1 - (distanceFromCenter / maxDistance)); // 중심에서 멀어질수록 0에 가까워짐
                noiseHeight += 1;
                /*noiseHeight = gradient;*/

                if (gradientbool)
                {
                     noiseHeight *= gradient; // Perlin Noise와 Gradient Noise를 곱함

                }

                noiseHeight -= 1;

                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;

                }
                else if (noiseHeight < minNoiseHegiht)
                {
                    minNoiseHegiht = noiseHeight;
                }
                noiseMap[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHegiht, maxNoiseHeight, noiseMap[x, y]);
            }
        }
        return noiseMap;
    }
}







