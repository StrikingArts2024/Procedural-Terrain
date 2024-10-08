using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed,float scale, int octaves, float persistance, float lacunarity,Vector2 offset)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        System.Random ping = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for(int i = 0; i < octaves; i++)
        {
            float offsetX = ping.Next(-100000, 100000)+offset.x;
            float offsetY = ping.Next(-100000, 100000)+offset.y;
            octaveOffsets[i]=new Vector2(offsetX, offsetY); 
        }
        if (scale <= 0)
        {
            scale = 0.0001f;
        }
        float maxNoiseHeight = float.MinValue;
        float minNoiseHegiht = float.MaxValue;

        float halWidth = mapWidth / 2f;
        float halHeight = mapHeight / 2f;


        for(int y=0;y<mapHeight; y++)
        {
            for(int x = 0; x < mapWidth; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;


                for(int i=0;i<octaves; i++)
                {
                    float smapleX = (x-halWidth) / scale * frequency + octaveOffsets[i].x;
                    float smapleY = (y-halHeight) / scale * frequency + octaveOffsets[i].y;

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
