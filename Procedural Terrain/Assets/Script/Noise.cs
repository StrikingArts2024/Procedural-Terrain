using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed,float scale, int octaves, float persistance, float lacunarity)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        System.Random ping = new System.Random(seed);
        Vector2[]ocra
        //    https://www.youtube.com/watch?v=MRNFcywkUSA&list=PLFt_AvWsXl0eBW2EiBtl_sxmDtSgZBxB3&index=3
        // 7분부터 다시 코딩
        if (scale <= 0)
        {
            scale = 0.0001f;
        }
        float maxNoiseHeight = float.MinValue;
        float minNoiseHegiht = float.MaxValue;

        for(int y=0;y<mapHeight; y++)
        {
            for(int x = 0; x < mapWidth; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;


                for(int i=0;i<octaves; i++)
                {
                    float smapleX = x / scale * frequency;
                    float smapleY = y / scale * frequency;

                    float perlineValue = Mathf.PerlinNoise(smapleX, smapleY)*2-1;
                    noiseHeight += perlineValue* amplitude;
                    amplitude *= persistance;
                    frequency *= lacunarity;
                }
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

        for(int y = 0; y < mapHeight; y++)
        {
            for(int x = 0; x < mapWidth; x++)
            {
                noiseMap[x,y] = Mathf.InverseLerp(minNoiseHegiht, maxNoiseHeight, noiseMap[x,y]);
            }
        }
        return noiseMap;
    }
}
