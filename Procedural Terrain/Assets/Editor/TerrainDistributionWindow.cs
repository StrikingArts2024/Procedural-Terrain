using UnityEditor;
using UnityEngine;

public class TerrainDistributionWindow : EditorWindow
{
    private MapGenerator mapGenerator; // TerrainManager ��ũ��Ʈ�� ���� ������Ʈ
    private TerrainDiversityCalculator terrainDiversityCalculator; // TerrainDiversityCalculator �ν��Ͻ�
    private float[,] noiseMap; // ������Ʈ���� �޾ƿ� noiseMap

    [MenuItem("Window/Terrain Distribution")]
    public static void ShowWindow()
    {
        GetWindow<TerrainDistributionWindow>("Terrain Distribution");
    }

    private void OnGUI()
    {
        Debug.Log("ongui");
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
        Debug.Log("repaint");

        Repaint(); // ������ ���� ������Ʈ�� �� UI�� ����
    }

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
        float graphHeight = position.height - 30;
        float barWidth = graphWidth / terrainCounts.Length;

        for (int i = 0; i < terrainCounts.Length; i++)
        {
            float percentage = (float)terrainCounts[i] / totalCells * 100f;
            float barHeight = (percentage / 100f) * graphHeight;

            // ���� �׸���
            GUI.Box(new Rect(10 + i * barWidth, graphHeight + 10 - barHeight, barWidth - 2, barHeight), GUIContent.none);
            GUI.Label(new Rect(10 + i * barWidth, graphHeight + 10, barWidth - 2, 20), terrainDiversityCalculator.regions[i].name + ": " + percentage.ToString("F2") + "%");
        }
    }
}
