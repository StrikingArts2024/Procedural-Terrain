using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Mesh;

public class MapGenerator : MonoBehaviour
{
    public bool gradient;

    //�� �򰡸� ���� ��ũ��Ʈ
    public event Action OnNoiseMapUpdated; // ������ �� ������Ʈ �̺�Ʈ

    public mapEvaluation mapEvaluation;
    //�� ������ ���� ��ũ��Ʈ


    public enum DrawMode { NoiseMap, ColourMap,Mesh};
    public DrawMode drawMode;

    const int mapChunkSize = 241;
    [Range(0, 6)]
    public int levelOfDetail;


    public float noiseScale;
    [Range(0, 1)]
    public float frequency;
    [Range(0, 10)]
    public int octaves;
    [Range(0,1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;

    public bool autoUpdate;
    public float[,] noiseMap;
    public TerrainType[] regions;
    public void GenerateMap()
    {




        noiseMap = Noise.GenerateNoiseMap(gradient, mapChunkSize, mapChunkSize, seed, noiseScale, frequency, octaves, persistance, lacunarity, offset);



        Color[] colourMap = new Color[mapChunkSize * mapChunkSize];
        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        colourMap[y * mapChunkSize + x] = regions[i].colour;
                        break;

                    }
                }
            }
        }

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        }
        else if (drawMode == DrawMode.ColourMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromCoulourMap(colourMap, mapChunkSize, mapChunkSize));

        }
        else if (drawMode == DrawMode.Mesh)
        {
            MeshData meshData = MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail);
            display.DrawMesh(meshData, TextureGenerator.TextureFromCoulourMap(colourMap, mapChunkSize, mapChunkSize));

            mapEvaluation.evaluation(noiseMap,regions);

            /*if (OnNoiseMapUpdated != null)
            {
                OnNoiseMapUpdated.Invoke();
            }*/
            OnNoiseMapUpdated.Invoke();

        }
    }
    public float[,] GetNoiseMap()
    {
        return noiseMap;
    }
    public float GetmeshHeightMultiplier()
    {
        return meshHeightMultiplier;
    }
    void OnValidate()
    {
        
        if(lacunarity < 1)
        {
            lacunarity = 1;
        }
        if(octaves < 0)
        {
            octaves = 0;
        }

    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;
}
