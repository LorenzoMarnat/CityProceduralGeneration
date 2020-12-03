using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField]
    Transform parent = null;

    [SerializeField]
    List<GameObject> buildingPrefabs = null;

    [SerializeField, Range(10, 200)]
    int gridSize = 10;

    [SerializeField]
    float buildingSize = 1f;

    [SerializeField]
    int seed = -1;

    [SerializeField]
    int pickMod = 0;

    private Vector3 townCenter;
    // Start is called before the first frame update
    void Start()
    {
        townCenter = new Vector3((gridSize / 2f) * buildingSize, 0, (gridSize / 2f) * buildingSize);

        if (seed < 0)
            seed = Random.Range(0, 100);

        GenerateGrid();
    }

    private void GenerateGrid()
    {
        for(int i = 0;i < gridSize;i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                int buildingToInstantiate = PickBuilding(i, j);
                GameObject building = Instantiate(buildingPrefabs[buildingToInstantiate], new Vector3(i*buildingSize, 0, j*buildingSize), Quaternion.identity);
                building.transform.parent = parent;
            }
        }
    }

    private int PickBuilding(int i, int j)
    {
        switch(pickMod)
        {
            case 0: return PickBuildingRandom();
            case 1: return PickBuildingPerlinNoise(i, j);
            case 2: return PickBuildingTownCenter(i, j);
            default: Debug.Log("Invalid picking mod");
                return 0;
        }
    }
    private int PickBuildingRandom()
    {
        return Random.Range(0, buildingPrefabs.Count);
    }

    private int PickBuildingPerlinNoise(int i, int j)
    {
        return (int)(Mathf.PerlinNoise(i/10f + seed, j/10f + seed) * buildingPrefabs.Count);
    }

    private int PickBuildingTownCenter(int i, int j)
    {
        Vector3 pos = new Vector3(i * buildingSize, 0, j * buildingSize);
        float distanceToCenter = Vector3.Distance(pos, townCenter);
        float size = (gridSize * buildingSize) / 2f;

        if (distanceToCenter < 0.33f * size)
            return buildingPrefabs.Count - 1;
        else if(distanceToCenter < 0.66f * size)
            return buildingPrefabs.Count - 2;
        else if (distanceToCenter < size)
            return buildingPrefabs.Count - 3;
        else if (distanceToCenter < 1.2f * size)
            return buildingPrefabs.Count - 4;
        else
            return 0;
    }
}
