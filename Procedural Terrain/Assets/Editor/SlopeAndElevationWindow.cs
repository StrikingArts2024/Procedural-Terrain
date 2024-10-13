using UnityEditor;
using UnityEngine;

public class SlopeAndElevationWindow : EditorWindow
{
    private MapGenerator mapGenerator; // TerrainManager 스크립트를 가진 오브젝트
    private float[,] noiseMap; // 오브젝트에서 받아올 noiseMap
    private float averageSlope; // 평균 경사도
    private float elevationVariability; // 고도 변화율
    private float meshHeightMultiplier = 1f; // 높이 배율

    [MenuItem("Window/Slope and Elevation")]
    public static void ShowWindow()
    {
        GetWindow<SlopeAndElevationWindow>("Slope and Elevation");
    }

    private void OnGUI()
    {
        mapGenerator = (MapGenerator)EditorGUILayout.ObjectField("Terrain Manager", mapGenerator, typeof(MapGenerator), true);
/*        meshHeightMultiplier = EditorGUILayout.FloatField("Height Multiplier", meshHeightMultiplier); // 높이 배율 입력 필드
*/
        if (/*GUILayout.Button("Get Noise Map")*/true)
        {
            if (mapGenerator != null)
            {
                noiseMap = mapGenerator.GetNoiseMap(); // GetNoiseMap 메서드를 호출하여 noiseMap을 가져옵니다.
                CalculateSlopeAndElevation(noiseMap); // 경사도 및 고도 변화율 계산
                Repaint(); // UI 갱신
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
        if (mapGenerator != null)
        {
            noiseMap = mapGenerator.GetNoiseMap();
            meshHeightMultiplier = mapGenerator.GetmeshHeightMultiplier();
            CalculateSlopeAndElevation(noiseMap); // 경사도 및 고도 변화율 계산
            Repaint(); // UI 갱신
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

        // 경사도 및 고도 계산
        for (int y = 0; y < height - 1; y++)
        {
            for (int x = 0; x < width - 1; x++)
            {
                float currentHeight = noiseMap[x, y] * meshHeightMultiplier;
                float rightHeight = noiseMap[x + 1, y] * meshHeightMultiplier;
                float bottomHeight = noiseMap[x, y + 1] * meshHeightMultiplier;

                // 경사도 계산
                float slopeX = Mathf.Abs(rightHeight - currentHeight);
                float slopeY = Mathf.Abs(bottomHeight - currentHeight);
                totalSlope += (slopeX + slopeY) / 2f;

                // 고도 변화율 계산
                totalElevation += currentHeight;
                minElevation = Mathf.Min(minElevation, currentHeight);
                maxElevation = Mathf.Max(maxElevation, currentHeight);
            }
        }

        // 평균 경사도 및 고도 변화율
        int totalCells = (width - 1) * (height - 1);
        averageSlope = totalSlope / totalCells;
        elevationVariability = maxElevation - minElevation;
    }
}
