using UnityEditor;
using UnityEngine;

public class TerrainDistributionWindow : EditorWindow
{
    private MapGenerator mapGenerator; // TerrainManager ��ũ��Ʈ�� ���� ������Ʈ
    private TerrainDiversityCalculator terrainDiversityCalculator; // TerrainDiversityCalculator �ν��Ͻ�
    private float[,] noiseMap; // ������Ʈ���� �޾ƿ� noiseMap
    private float shannonDiversityIndex; // ���� �پ缺 ����

    [MenuItem("Window/Terrain Distribution")]
    public static void ShowWindow()
    {
        GetWindow<TerrainDistributionWindow>("Terrain Distribution");
    }

    private void OnGUI()
    {
        mapGenerator = (MapGenerator)EditorGUILayout.ObjectField("Terrain Manager", mapGenerator, typeof(MapGenerator), true);

        if (/*GUILayout.Button("Get Noise Map")*/true)
        {
            if (mapGenerator != null)
            {
                noiseMap = mapGenerator.GetNoiseMap(); // GetNoiseMap �޼��带 ȣ���Ͽ� noiseMap�� �����ɴϴ�.
                terrainDiversityCalculator = new TerrainDiversityCalculator
                {
                    regions = mapGenerator.regions // mapGenerator�� regions�� terrainDiversityCalculator�� ����
                };
                CalculateDiversity(noiseMap); // ���� �پ缺 ���
                Repaint(); // UI ����
            }
            else
            {
                Debug.LogWarning("Please assign a Terrain Manager object.");
            }
        }

        if (noiseMap != null)
        {
            DisplayTerrainDistribution(noiseMap);
        }
    }

    private void OnEnable()
    {
        if (mapGenerator != null)
        {
            mapGenerator.OnNoiseMapUpdated += OnNoiseMapUpdated; // �̺�Ʈ ����
        }
    }

    private void OnDisable()
    {
        if (mapGenerator != null)
        {
            mapGenerator.OnNoiseMapUpdated -= OnNoiseMapUpdated; // �̺�Ʈ ���� ����
        }
    }

    private void OnNoiseMapUpdated()
    {
        Repaint(); // ������ ���� ������Ʈ�� �� UI�� ����
    }

    Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];

        for (int i = 0; i < pix.Length; i++)
            pix[i] = new Color(col.r, col.g, col.b, 1f);  // ���� ���� 1�� ����

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();

        return result;
    }

    // ���� �پ缺 �� ���� �پ缺 ���� ���
    private void CalculateDiversity(float[,] noiseMap)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        int[] terrainCounts = new int[terrainDiversityCalculator.regions.Length];
        int totalCells = width * height;

        // �� ������ ���� ī��Ʈ
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                TerrainType terrainType = terrainDiversityCalculator.ClassifyTerrain(noiseMap[x, y]);
                int index = System.Array.FindIndex(terrainDiversityCalculator.regions, r => r.name == terrainType.name);
                terrainCounts[index]++;
            }
        }

        // ���� �پ缺 ���� ���
        float H = 0f;
        for (int i = 0; i < terrainCounts.Length; i++)
        {
            if (terrainCounts[i] > 0)
            {
                float proportion = (float)terrainCounts[i] / totalCells;
                H -= proportion * Mathf.Log(proportion); // ���� ���� ������ ���
            }
        }

        shannonDiversityIndex = H;
    }

    // ���� ������ �پ缺 ���� �׷��� �׸���
    private void DisplayTerrainDistribution(float[,] noiseMap)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        int[] terrainCounts = new int[terrainDiversityCalculator.regions.Length];
        int totalCells = width * height;

        // �� ������ ���� ī��Ʈ
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                TerrainType terrainType = terrainDiversityCalculator.ClassifyTerrain(noiseMap[x, y]);
                int index = System.Array.FindIndex(terrainDiversityCalculator.regions, r => r.name == terrainType.name);
                terrainCounts[index]++;
            }
        }

        // �׷��� �׸���
        float graphWidth = position.width - 20;
        float graphHeight = position.height - 60; // ���̸� �� ���� (���� ���� ������ ����)
        float barWidth = graphWidth / terrainCounts.Length;

        for (int i = 0; i < terrainCounts.Length; i++)
        {
            float percentage = (float)terrainCounts[i] / totalCells * 100f;
            float barHeight = (percentage / 100f) * graphHeight;

            // ���� ���� ����
            GUIStyle barStyle = new GUIStyle(GUI.skin.box);
            barStyle.normal.background = MakeTex(1, 1, terrainDiversityCalculator.regions[i].colour); // regions�� �ִ� ���� ���

            // ���� �׸���
            GUI.Box(new Rect(10 + i * barWidth, graphHeight + 10 - barHeight, barWidth - 2, barHeight), GUIContent.none, barStyle);
            GUI.Label(new Rect(10 + i * barWidth, graphHeight + 10, barWidth - 2, 20), terrainDiversityCalculator.regions[i].name + ": " + percentage.ToString("F2") + "%");
        }

        // ���� �پ缺 ���� ǥ��
        GUILayout.Label("Shannon Diversity Index: " + shannonDiversityIndex.ToString("F4"));
    }
}
