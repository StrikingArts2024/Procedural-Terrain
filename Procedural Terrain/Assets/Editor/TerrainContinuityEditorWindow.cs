using UnityEngine;
using UnityEditor;

public class TerrainContinuityEditorWindow : EditorWindow
{
    private MapGenerator mapGenerator; // MapGenerator 스크립트에 대한 참조
    private float continuityScore; // 연속성 점수

    [MenuItem("Window/Terrain Continuity Calculator")]
    public static void ShowWindow()
    {
        GetWindow<TerrainContinuityEditorWindow>("Terrain Continuity Calculator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Terrain Continuity Calculator", EditorStyles.boldLabel);

        // MapGenerator 스크립트를 연결하는 필드
        mapGenerator = (MapGenerator)EditorGUILayout.ObjectField("Map Generator", mapGenerator, typeof(MapGenerator), true);

        // 연속성 평가 버튼
        if (GUILayout.Button("Evaluate Terrain Continuity"))
        {
            if (mapGenerator != null)
            {
                TerrainContinuityCalculator calculator = new TerrainContinuityCalculator
                {
                    regions = mapGenerator.regions, // MapGenerator에서 regions 가져오기
                    noiseMap = mapGenerator.noiseMap // MapGenerator에서 noiseMap 가져오기
                };
                continuityScore = calculator.CalculateContinuityScore();
                Debug.Log($"Terrain Continuity Score: {continuityScore:F2}");
            }
            else
            {
                Debug.LogWarning("Please assign a Map Generator.");
            }
        }

        // 연속성 점수 출력
        GUILayout.Label($"Continuity Score: {continuityScore:F2}");
    }
}

public class TerrainContinuityCalculator
{
    public TerrainType[] regions; // 지형 타입 배열
    public float[,] noiseMap; // 노이즈 맵 데이터

    // 인접한 셀들을 기준으로 연속성 평가
    public float CalculateContinuityScore()
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        int similarNeighbors = 0;
        int totalComparisons = 0;

        // 모든 셀에 대해 인접한 셀과 지형 유형을 비교
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                TerrainType currentType = ClassifyTerrain(noiseMap[x, y]);

                // 오른쪽 셀과 비교
                if (x < width - 1)
                {
                    TerrainType rightType = ClassifyTerrain(noiseMap[x + 1, y]);
                    if (currentType.name == rightType.name)
                        similarNeighbors++;
                    totalComparisons++;
                }

                // 아래쪽 셀과 비교
                if (y < height - 1)
                {
                    TerrainType belowType = ClassifyTerrain(noiseMap[x, y + 1]);
                    if (currentType.name == belowType.name)
                        similarNeighbors++;
                    totalComparisons++;
                }
            }
        }

        // 연속성 점수 계산: 인접한 셀이 같은 유형일 확률을 반환
        return totalComparisons > 0 ? (float)similarNeighbors / totalComparisons : 0f;
    }

    // 지형 분류 함수 (noiseMap의 값을 이용)
    public TerrainType ClassifyTerrain(float noiseValue)
    {
        for (int i = 0; i < regions.Length; i++)
        {
            if (noiseValue < regions[i].height)
            {
                return regions[i];
            }
        }
        return regions[regions.Length - 1];
    }
}
