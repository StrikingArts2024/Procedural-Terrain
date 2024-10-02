using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MapGenerator : MonoBehaviour
{
    public int mapWidth;
    public int mapHeight;
    public float noiseScale;

    public int octaves;
    public float persistance;
    public float lacunarity;


    public bool autoUpdate;
    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap( mapWidth, mapHeight, noiseScale, octaves, persistance, lacunarity);


        MapDisplay display=FindObjectOfType<MapDisplay>();
        display.DrawNoiseMap(noiseMap);
    }
}
