using UnityEditor;
using UnityEngine;

public class SlopeAndElevationWindow : EditorWindow
{
    private MapGenerator mapGenerator; // TerrainManager ��ũ��Ʈ�� ���� ������Ʈ
    private float[,] noiseMap; // ������Ʈ���� �޾ƿ� noiseMap
    private float averageSlope; // ��� ��絵
    private float elevationVariability; // �� ��ȭ��
    private float meshHeightMultiplier = 1f; // ���� ����

    [MenuItem("Window/Slope and Elevation")]
    public static void ShowWindow()
    {
        GetWindow<SlopeAndElevationWindow>("Slope and Elevation");
    }

    private void OnGUI()
    {
        mapGenerator = (MapGenerator)EditorGUILayout.ObjectField("Terrain Manager", mapGenerator, typeof(MapGenerator), true);
/*        meshHeightMultiplier = EditorGUILayout.FloatField("Height Multiplier", meshHeightMultiplier); // ���� ���� �Է� �ʵ�
*/
        if (/*GUILayout.Button("Get Noise Map")*/true)
        {
            if (mapGenerator != null)
            {
                noiseMap = mapGenerator.GetNoiseMap(); // GetNoiseMap �޼��带 ȣ���Ͽ� noiseMap�� �����ɴϴ�.
                CalculateSlopeAndElevation(noiseMap); // ��絵 �� �� ��ȭ�� ���
                Repaint(); // UI ����
            }
            else
            {
                Debug.LogWarning("Please assign a Terrain Manager object.");
            }
        }

        if (noiseMap != null)
        {
            GUILayout.Label("Average Slope: " + averageSlope.ToString("F4"));
            GUILayout.Label("Elevation Variability: " + elevationVariability.ToString("F4"));
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
        if (mapGenerator != null)
        {
            noiseMap = mapGenerator.GetNoiseMap();
            meshHeightMultiplier = mapGenerator.GetmeshHeightMultiplier();
            CalculateSlopeAndElevation(noiseMap); // ��絵 �� �� ��ȭ�� ���
            Repaint(); // UI ����
        }
    }

    private void CalculateSlopeAndElevation(float[,] noiseMap)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        float totalSlope = 0f;
        float totalElevation = 0f;
        float minElevation = float.MaxValue;
        float maxElevation = float.MinValue;

        // ��絵 �� �� ���
        for (int y = 0; y < height - 1; y++)
        {
            for (int x = 0; x < width - 1; x++)
            {
                float currentHeight = noiseMap[x, y] * meshHeightMultiplier;
                float rightHeight = noiseMap[x + 1, y] * meshHeightMultiplier;
                float bottomHeight = noiseMap[x, y + 1] * meshHeightMultiplier;

                // ��絵 ���
                float slopeX = Mathf.Abs(rightHeight - currentHeight);
                float slopeY = Mathf.Abs(bottomHeight - currentHeight);
                totalSlope += (slopeX + slopeY) / 2f;

                // �� ��ȭ�� ���
                totalElevation += currentHeight;
                minElevation = Mathf.Min(minElevation, currentHeight);
                maxElevation = Mathf.Max(maxElevation, currentHeight);
            }
        }

        // ��� ��絵 �� �� ��ȭ��
        int totalCells = (width - 1) * (height - 1);
        averageSlope = totalSlope / totalCells;
        elevationVariability = maxElevation - minElevation;
    }
}
