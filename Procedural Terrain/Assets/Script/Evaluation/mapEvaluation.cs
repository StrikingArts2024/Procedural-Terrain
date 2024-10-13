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

    

    public TerrainType[] regions; // mapGenerator���� ���ǵ� regions �迭

   



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
        // ��� ������ �ʰ��ϴ� ��� �⺻���� ��ȯ (��: ������ ����)
        return regions[regions.Length - 1];
    }

    // ���� �پ缺 ���� ��� �Լ� (Shannon's Diversity Index)
    public float CalculateDiversityIndex(float[,] noiseMap)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        int[] terrainCounts = new int[regions.Length];
        int totalCells = width * height;

        // �� ������ ���� ī��Ʈ
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                TerrainType terrainType = ClassifyTerrain(noiseMap[x, y]);
                int index = Array.FindIndex(regions, r => r.name == terrainType.name);
                terrainCounts[index]++;
            }
        }

        // Shannon's Diversity Index ���
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

    // ������ ���� ��� �Լ�
    public void PrintTerrainDistribution(float[,] noiseMap)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        int[] terrainCounts = new int[regions.Length];
        int totalCells = width * height;

        // �� ������ ���� ī��Ʈ
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                TerrainType terrainType = ClassifyTerrain(noiseMap[x, y]);
                int index = Array.FindIndex(regions, r => r.name == terrainType.name);
                terrainCounts[index]++;
            }
        }

        // ���� ���� ���
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
            //���� ���� �м�
            TerrainDiversityCalculator terrainDiversityCalculator = new TerrainDiversityCalculator();
            /*float diversityIndex = terrainDiversityCalculator.CalculateDiversityIndex(noiseMap);
            Debug.Log($"Terrain Diversity Index: {diversityIndex}");

            terrainDiversityCalculator.PrintTerrainDistribution(noiseMap);*/

        }
    }

}

/*
 ������ �� ���� �˰����� ��ġ�� �� ����� ������ ��, ���� ������ Ư��, ���⼺, �پ缺, �ڿ������� ���� �����ϴ� �پ��� ������ ����� �� �ֽ��ϴ�. ����ڽ� ������ ���� ��ġ������ ���� �� �ִ� ������� ������ �����ϴ�:
//���� ��
1. ���� �پ缺 ���� (Terrain Diversity Index)
��ǥ: �� �� �پ��� ���� ����� ������ ������ ��.
�� ���:
�� ����(��, ���, ��, �ٴ� ��)�� ������ �׵��� �����ϴ� ������ ������ ��, ���� �󸶳� ���� �����Ǿ� �ִ��� ���մϴ�. �پ缺�� �������� �پ��� ������ ���� �����Ǿ� ������ �ǹ��մϴ�.
Shannon's Diversity Index �Ǵ� Simpson's Diversity Index�� ����Ͽ� ����� �� �ֽ��ϴ�.

//���� ��
2. ��� ��絵 �� �� ��ȭ�� (Average Slope and Elevation Variability)
��ǥ: ���� �� ��ȭ�� �����Ͽ� ������ ���⼺�� ��.
�� ���:
������ ��絵�� �м��Ͽ� ��� ��絵�� ���� ��ȭ���� ����մϴ�.
�̸� ���� ������ ��� ������ �����ϰ� ��ȭ�Ǿ� �ִ���, Ȥ�� ����ġ�� ������̰ų� �������� ���� �� �ֽ��ϴ�.
�� ��ȭ���� ��ǥ����� ��� ��ǥ�� ���� ���� ǥ�� ������ ����� �� �ֽ��ϴ�.


3. ���� ���Ӽ� (Terrain Contiguity)
��ǥ: ���� ������ ������ �󸶳� ū ������ �����ϴ���, �׸��� �ҿ��������� �������� ��.
�� ���:
���� ������ ������ �󸶳� ���� �������� ���Ӽ��� �����ϴ��� Ȯ���մϴ�. ���� ���, ����̳� ���� �и����� �ʰ� �а� �̾��� �ִ���, �ƴϸ� �ұ�Ģ�ϰ� �������� �ִ����� �м�.
�̸� ���� ���� ���� �м�(Connected Component Analysis)�� ����� �� ������, �ʿ��� ���ӵ� ������ ������ ������ �����Ͽ� ����մϴ�.


4. ������ ������ ������ (Terrain Balance)
��ǥ: �ڿ� ������ ���� ��Ұ� ���� �ְ� ��ġ�Ǿ����� ��.
�� ���:
Ư�� �ڿ��̳� ������ �󵵿� ������ ����ġ�� ���ʿ� ġ��ġ�� �ʰ� ���� �ְ� ��ġ�Ǿ����� Ȯ���մϴ�.
Ǫ�Ƽ� ����(Poisson Distribution) �Ǵ� ��Ÿ ����� ���� ����Ͽ� �ڿ� �� ������ ������ �󸶳� �������� �м��� �� �ֽ��ϴ�.


5. ������ �ڿ������� �� (Naturalness of Terrain)
��ǥ: ���� �������̰ų� �ڿ������� ������ ���߾����� ��ġ������ ��.
�� ���:
�ڿ� ������ Ư���� �м��� �� �̸� �������� ������ ���� ������ �ڿ����� ������ ������ �ִ��� ���� �� �ֽ��ϴ�. ���� ���, ���� ��õ�� �帧 ���ϰ� ������ ��õ�� ������ ���ϰų�, ��� ����� ������ ���������� �м�.
�ڿ��������� ���ϱ� ���� �ڿ� ���� �����ͼ°��� ���絵�� �����ϴ� ����� ����� �� �ֽ��ϴ�.


6. ������ ���Ἲ (Connectivity)
��ǥ: �� ���� ���� �������� ��� ����Ǿ� �ִ��� ��.
�� ���:
�� ���� �ֿ� ���� �� ���Ἲ�� �м��Ͽ�, �÷��̾ �̵��� �� �󸶳� �ڿ������� ȿ�������� �̵��� �� �ִ����� ���մϴ�. �̴� A* �˰���� ���� ��� Ž�� �˰����� ����Ͽ� �ֿ� ���� �� ���� ��� ���̸� ��������ν� ������ �� �ֽ��ϴ�.
���Ἲ�� ����ġ�� ������ ������ �ܼ����� �� �ְ�, �ʹ� ������ Ž���� �������� �� �����Ƿ� ������ ������ �߿��մϴ�.


7. ����Ż ���� �м� (Fractal Dimension Analysis)
��ǥ: ������ ���⼺�� ��ġ������ ����.
�� ���:
������ �����̳� ���� ��ȭ�� �󸶳� �����ϰ� �����Ǿ� �ִ����� ����Ż ������ �̿��� ������ �� �ֽ��ϴ�. ����Ż ������ �������� �� �����ϰ� ������ ������ �ǹ��ϸ�, �������� �ܼ��� ������ ��Ÿ���ϴ�.
�̸� ���� ������ ������ �󸶳� ������ ������ ������ ��ġ������ ���� �� �ֽ��ϴ�.


8. ���� ���� �ӵ� �� ���ҽ� ��뷮
��ǥ: �˰����� ������ ���������� ��.
�� ���:
�� ���� �ð�, �޸� ��뷮, CPU �� GPU ���ϸ� �����Ͽ� �˰����� ȿ������ ���մϴ�. ��Ը� ���� ������ �� ���� ���� ���� ������ ȿ�������� �۵��ϴ��� �м��ϴ� ���� �߿��մϴ�.


9. ��⼺ �м� (Modularity Analysis)
��ǥ: ���� �پ��� ������ ���������� ����ϴ���, �Ǵ� ���� �������� ���������� �м�.
�� ���:
���� ���� ���� ������, �� ����� ���������� ����ϴ��� �Ǵ� �������� ���������� ���� �� �ֽ��ϴ�. �̴� ���� ���� �ڿ� �帧, ������ Ư���� ���輺 ���� �м��Ͽ� ���� �� �ֽ��ϴ�.

�̷��� ������� ���������� ����ϸ�, �˰����� ������ ���� �پ��� �������� ��ġȭ�ϰ� ���� �� ������, ������ �˰����� � Ư���� �� �ݿ��ϰ� �ִ��� ���������� ���� �� �ֽ��ϴ�.
 */