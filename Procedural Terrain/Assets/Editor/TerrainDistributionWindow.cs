using UnityEditor;
using UnityEngine;

public class TerrainDistributionWindow : EditorWindow
{
    private MapGenerator mapGenerator; // TerrainManager 스크립트를 가진 오브젝트
    private TerrainDiversityCalculator terrainDiversityCalculator; // TerrainDiversityCalculator 인스턴스
    private float[,] noiseMap; // 오브젝트에서 받아올 noiseMap

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
                noiseMap = mapGenerator.GetNoiseMap(); // GetNoiseMap 메서드를 호출하여 noiseMap을 가져옵니다.
                terrainDiversityCalculator = new TerrainDiversityCalculator
                {
                    regions = mapGenerator.regions // mapGenerator의 regions를 terrainDiversityCalculator에 설정
                };
                Repaint(); // UI 갱신
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
            mapGenerator.OnNoiseMapUpdated += OnNoiseMapUpdated; // 이벤트 구독
        }
    }

    private void OnDisable()
    {
        if (mapGenerator != null)
        {
            mapGenerator.OnNoiseMapUpdated -= OnNoiseMapUpdated; // 이벤트 구독 해제
        }
    }

    private void OnNoiseMapUpdated()
    {
        Debug.Log("repaint");

        Repaint(); // 노이즈 맵이 업데이트될 때 UI를 갱신
    }

    private void DisplayTerrainDistribution(float[,] noiseMap)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        int[] terrainCounts = new int[terrainDiversityCalculator.regions.Length];
        int totalCells = width * height;

        // 각 지형의 개수 카운트
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                TerrainType terrainType = terrainDiversityCalculator.ClassifyTerrain(noiseMap[x, y]);
                int index = System.Array.FindIndex(terrainDiversityCalculator.regions, r => r.name == terrainType.name);
                terrainCounts[index]++;
            }
        }

        // 그래프 그리기
        float graphWidth = position.width - 20;
        float graphHeight = position.height - 30;
        float barWidth = graphWidth / terrainCounts.Length;

        for (int i = 0; i < terrainCounts.Length; i++)
        {
            float percentage = (float)terrainCounts[i] / totalCells * 100f;
            float barHeight = (percentage / 100f) * graphHeight;

            // 막대 그리기
            GUI.Box(new Rect(10 + i * barWidth, graphHeight + 10 - barHeight, barWidth - 2, barHeight), GUIContent.none);
            GUI.Label(new Rect(10 + i * barWidth, graphHeight + 10, barWidth - 2, 20), terrainDiversityCalculator.regions[i].name + ": " + percentage.ToString("F2") + "%");
        }
    }
}
