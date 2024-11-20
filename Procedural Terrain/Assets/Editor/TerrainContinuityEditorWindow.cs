using UnityEngine;
using UnityEditor;

public class TerrainContinuityEditorWindow : EditorWindow
{
    private MapGenerator mapGenerator; // MapGenerator ��ũ��Ʈ�� ���� ����
    private float continuityScore; // ���Ӽ� ����

    [MenuItem("Window/Terrain Continuity Calculator")]
    public static void ShowWindow()
    {
        GetWindow<TerrainContinuityEditorWindow>("Terrain Continuity Calculator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Terrain Continuity Calculator", EditorStyles.boldLabel);

        // MapGenerator ��ũ��Ʈ�� �����ϴ� �ʵ�
        mapGenerator = (MapGenerator)EditorGUILayout.ObjectField("Map Generator", mapGenerator, typeof(MapGenerator), true);

        // ���Ӽ� �� ��ư
        if (GUILayout.Button("Evaluate Terrain Continuity"))
        {
            if (mapGenerator != null)
            {
                TerrainContinuityCalculator calculator = new TerrainContinuityCalculator
                {
                    regions = mapGenerator.regions, // MapGenerator���� regions ��������
                    noiseMap = mapGenerator.noiseMap // MapGenerator���� noiseMap ��������
                };
                continuityScore = calculator.CalculateContinuityScore();
                Debug.Log($"Terrain Continuity Score: {continuityScore:F2}");
            }
            else
            {
                Debug.LogWarning("Please assign a Map Generator.");
            }
        }

        // ���Ӽ� ���� ���
        GUILayout.Label($"Continuity Score: {continuityScore:F2}");
    }
}

public class TerrainContinuityCalculator
{
    public TerrainType[] regions; // ���� Ÿ�� �迭
    public float[,] noiseMap; // ������ �� ������

    // ������ ������ �������� ���Ӽ� ��
    public float CalculateContinuityScore()
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        int similarNeighbors = 0;
        int totalComparisons = 0;

        // ��� ���� ���� ������ ���� ���� ������ ��
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                TerrainType currentType = ClassifyTerrain(noiseMap[x, y]);

                // ������ ���� ��
                if (x < width - 1)
                {
                    TerrainType rightType = ClassifyTerrain(noiseMap[x + 1, y]);
                    if (currentType.name == rightType.name)
                        similarNeighbors++;
                    totalComparisons++;
                }

                // �Ʒ��� ���� ��
                if (y < height - 1)
                {
                    TerrainType belowType = ClassifyTerrain(noiseMap[x, y + 1]);
                    if (currentType.name == belowType.name)
                        similarNeighbors++;
                    totalComparisons++;
                }
            }
        }

        // ���Ӽ� ���� ���: ������ ���� ���� ������ Ȯ���� ��ȯ
        return totalComparisons > 0 ? (float)similarNeighbors / totalComparisons : 0f;
    }

    // ���� �з� �Լ� (noiseMap�� ���� �̿�)
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
