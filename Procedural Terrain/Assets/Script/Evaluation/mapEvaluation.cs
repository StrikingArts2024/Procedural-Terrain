using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mesh;


public class TerrainDiversityCalculator
{
    /*public struct TerrainType
    {
        public string name;
        public float height;
        public Color color;
    }*/

    

    public TerrainType[] regions; // mapGenerator에서 정의된 regions 배열

   



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
        // 모든 범위를 초과하는 경우 기본값을 반환 (예: 마지막 지형)
        return regions[regions.Length - 1];
    }

    // 지형 다양성 지수 계산 함수 (Shannon's Diversity Index)
    public float CalculateDiversityIndex(float[,] noiseMap)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        int[] terrainCounts = new int[regions.Length];
        int totalCells = width * height;

        // 각 지형의 개수 카운트
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                TerrainType terrainType = ClassifyTerrain(noiseMap[x, y]);
                int index = Array.FindIndex(regions, r => r.name == terrainType.name);
                terrainCounts[index]++;
            }
        }

        // Shannon's Diversity Index 계산
        float diversityIndex = 0f;
        for (int i = 0; i < terrainCounts.Length; i++)
        {
            if (terrainCounts[i] > 0)
            {
                float proportion = (float)terrainCounts[i] / totalCells;
                diversityIndex -= proportion * Mathf.Log(proportion);
            }
        }

        return diversityIndex;
    }

    // 지형별 분포 출력 함수
    public void PrintTerrainDistribution(float[,] noiseMap)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        int[] terrainCounts = new int[regions.Length];
        int totalCells = width * height;

        // 각 지형의 개수 카운트
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                TerrainType terrainType = ClassifyTerrain(noiseMap[x, y]);
                int index = Array.FindIndex(regions, r => r.name == terrainType.name);
                terrainCounts[index]++;
            }
        }

        // 지형 분포 출력
        foreach (TerrainType terrainType in regions)
        {
            int count = terrainCounts[Array.FindIndex(regions, r => r.name == terrainType.name)];
            float percentage = ((float)count / totalCells) * 100f;
            Debug.Log($"{terrainType.name}: {percentage:F2}%");
        }
    }
}








public class mapEvaluation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void evaluation(float[,] noiseMap, TerrainType[] regions)
    {
        {
            //지형 분포 분석
            TerrainDiversityCalculator terrainDiversityCalculator = new TerrainDiversityCalculator();
            /*float diversityIndex = terrainDiversityCalculator.CalculateDiversityIndex(noiseMap);
            Debug.Log($"Terrain Diversity Index: {diversityIndex}");

            terrainDiversityCalculator.PrintTerrainDistribution(noiseMap);*/

        }
    }

}

/*
 절차적 맵 생성 알고리즘의 수치적 평가 방법을 선택할 때, 맵의 구조적 특성, 복잡성, 다양성, 자연스러움 등을 측정하는 다양한 기준을 사용할 수 있습니다. 샌드박스 게임의 맵을 수치적으로 평가할 수 있는 방법들은 다음과 같습니다:
//개발 완
1. 지형 다양성 지수 (Terrain Diversity Index)
목표: 맵 내 다양한 지형 요소의 균형과 분포를 평가.
평가 방법:
각 지형(산, 평야, 강, 바다 등)의 종류와 그들이 차지하는 면적을 측정한 후, 서로 얼마나 고르게 분포되어 있는지 평가합니다. 다양성이 높을수록 다양한 지형이 고르게 분포되어 있음을 의미합니다.
Shannon's Diversity Index 또는 Simpson's Diversity Index를 사용하여 계산할 수 있습니다.

//개발 완
2. 평균 경사도 및 고도 변화율 (Average Slope and Elevation Variability)
목표: 맵의 고도 변화를 측정하여 지형의 복잡성을 평가.
평가 방법:
지형의 경사도를 분석하여 평균 경사도와 고도의 변화율을 계산합니다.
이를 통해 평지와 산악 지형이 적절하게 조화되어 있는지, 혹은 지나치게 평면적이거나 복잡한지 평가할 수 있습니다.
고도 변화율은 좌표계상의 모든 좌표에 대한 고도의 표준 편차로 계산할 수 있습니다.


3. 지형 연속성 (Terrain Contiguity)
목표: 같은 유형의 지형이 얼마나 큰 영역을 차지하는지, 그리고 불연속적으로 나뉘는지 평가.
평가 방법:
같은 유형의 지형이 얼마나 넓은 영역에서 연속성을 유지하는지 확인합니다. 예를 들어, 산맥이나 숲이 분리되지 않고 넓게 이어져 있는지, 아니면 불규칙하게 나누어져 있는지를 분석.
이를 위해 연결 성분 분석(Connected Component Analysis)을 사용할 수 있으며, 맵에서 연속된 동일한 지형의 면적을 측정하여 계산합니다.


4. 생성된 지형의 균형성 (Terrain Balance)
목표: 자원 분포와 지형 요소가 균형 있게 배치되었는지 평가.
평가 방법:
특정 자원이나 지형의 빈도와 분포가 지나치게 한쪽에 치우치지 않고 균형 있게 배치되었는지 확인합니다.
푸아송 분포(Poisson Distribution) 또는 기타 통계적 모델을 사용하여 자원 및 지형의 분포가 얼마나 균일한지 분석할 수 있습니다.


5. 지형의 자연스러움 평가 (Naturalness of Terrain)
목표: 맵이 현실적이거나 자연스러운 지형을 갖추었는지 수치적으로 평가.
평가 방법:
자연 지형의 특성을 분석한 후 이를 바탕으로 생성된 맵의 지형이 자연적인 형상을 가지고 있는지 비교할 수 있습니다. 예를 들어, 실제 하천의 흐름 패턴과 생성된 하천의 패턴을 비교하거나, 산과 계곡의 분포가 현실적인지 분석.
자연스러움을 평가하기 위해 자연 지형 데이터셋과의 유사도를 측정하는 방법을 사용할 수 있습니다.


6. 지형의 연결성 (Connectivity)
목표: 맵 상의 여러 지역들이 어떻게 연결되어 있는지 평가.
평가 방법:
맵 내의 주요 지역 간 연결성을 분석하여, 플레이어가 이동할 때 얼마나 자연스럽고 효율적으로 이동할 수 있는지를 평가합니다. 이는 A* 알고리즘과 같은 경로 탐색 알고리즘을 사용하여 주요 지점 간 최적 경로 길이를 계산함으로써 측정할 수 있습니다.
연결성이 지나치게 높으면 지형이 단순해질 수 있고, 너무 낮으면 탐험이 불편해질 수 있으므로 적절한 균형이 중요합니다.


7. 프랙탈 차원 분석 (Fractal Dimension Analysis)
목표: 지형의 복잡성을 수치적으로 측정.
평가 방법:
지형의 윤곽이나 높이 변화가 얼마나 복잡하게 구성되어 있는지를 프랙탈 차원을 이용해 측정할 수 있습니다. 프랙탈 차원이 높을수록 더 복잡하고 세밀한 지형을 의미하며, 낮을수록 단순한 구조를 나타냅니다.
이를 통해 생성된 지형이 얼마나 복잡한 구조를 갖는지 수치적으로 평가할 수 있습니다.


8. 지형 생성 속도 및 리소스 사용량
목표: 알고리즘의 성능을 정량적으로 평가.
평가 방법:
맵 생성 시간, 메모리 사용량, CPU 및 GPU 부하를 수집하여 알고리즘의 효율성을 평가합니다. 대규모 맵을 생성할 때 성능 저하 없이 빠르고 효율적으로 작동하는지 분석하는 것이 중요합니다.


9. 모듈성 분석 (Modularity Analysis)
목표: 맵의 다양한 지역이 독립적으로 기능하는지, 또는 서로 연관성을 가지는지를 분석.
평가 방법:
맵을 여러 모듈로 나누고, 각 모듈이 독립적으로 기능하는지 또는 연관성을 가지는지를 평가할 수 있습니다. 이는 지역 간의 자원 흐름, 지형적 특성의 연계성 등을 분석하여 평가할 수 있습니다.

이러한 방법들을 종합적으로 사용하면, 알고리즘이 생성한 맵을 다양한 기준으로 수치화하고 비교할 수 있으며, 각각의 알고리즘이 어떤 특성을 잘 반영하고 있는지 정량적으로 평가할 수 있습니다.
 */