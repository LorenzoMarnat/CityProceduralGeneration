using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] Transform parent = null;

    [SerializeField] List<GameObject> buildingPrefabs = null;

    [SerializeField, Range(10, 200)] int gridSize = 10;

    [SerializeField] float buildingSize = 1f;

    [SerializeField] float noiseSize = 10f;

    [SerializeField] int seed = -1;

    [SerializeField] int pickMod = 0;

    private Vector3 townCenter;
    // Start is called before the first frame update
    void Start()
    {
        // Resize grass asset to match the grid
        buildingPrefabs[0].transform.localScale = new Vector3(buildingSize, 0.1f, buildingSize);

        // Find center of the grid
        townCenter = new Vector3((gridSize / 2f) * buildingSize, 0, (gridSize / 2f) * buildingSize);

        if (seed < 0)
            seed = Random.Range(0, 100);

        GenerateGrid();
    }

    private void GenerateGrid()
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                int buildingToInstantiate = PickBuilding(i, j);
                GameObject building = Instantiate(buildingPrefabs[buildingToInstantiate], new Vector3(i * buildingSize, 0, j * buildingSize), Quaternion.identity);
                building.transform.parent = parent;
            }
        }
    }

    // Pick a building depending on picking mod 
    private int PickBuilding(int i, int j)
    {
        switch (pickMod)
        {
            case 0: return PickBuildingRandom();
            case 1: return PickBuildingPerlinNoise(i, j);
            case 2: return PickBuildingTownCenter(i, j);
            case 3: return PickBuildingTownCenterWithNoise(i, j);
            default:
                Debug.Log("Invalid picking mod");
                return 0;
        }
    }
    // Pick a random building in building list
    private int PickBuildingRandom()
    {
        return Random.Range(0, buildingPrefabs.Count);
    }

    // Pick a building following Perlin Noise
    // The higher "noiseSize" is, the larger areas with the same building will be
    private int PickBuildingPerlinNoise(int i, int j)
    {
        return (int)(Mathf.PerlinNoise(i / noiseSize + seed, j / noiseSize + seed) * buildingPrefabs.Count);
    }

    // Pick a building depending on the distance between building's position and town center
    private int PickBuildingTownCenter(int i, int j)
    {
        Vector3 pos = new Vector3(i * buildingSize, 0, j * buildingSize);
        float distanceToCenter = Vector3.Distance(pos, townCenter);

        // Grid radius
        float size = (gridSize * buildingSize) / 2f;

        if (distanceToCenter < 0.33f * size)
            return buildingPrefabs.Count - 1;
        else if (distanceToCenter < 0.66f * size)
            return buildingPrefabs.Count - 2;
        else if (distanceToCenter < size)
            return buildingPrefabs.Count - 3;
        else if (distanceToCenter < 1.2f * size)
            return buildingPrefabs.Count - 4;
        else
            return 0;
    }

    // Pick a building using both distance with town center and Perlin noise
    private int PickBuildingTownCenterWithNoise(int i, int j)
    {
        Vector3 pos = new Vector3(i * buildingSize, 0, j * buildingSize);
        float distanceToCenter = Vector3.Distance(pos, townCenter);
        float size = (gridSize * buildingSize) / 2f;

        int noise = (int)(Mathf.PerlinNoise(i / noiseSize + seed, j / noiseSize + seed) * 100);

        if (distanceToCenter < 0.33f * size)
        {
            if (noise < 40)
                return 4;
            else if (noise >= 40 && noise < 80)
                return 3;
            else
                return 0;
        }
        else if (distanceToCenter < 0.66f * size)
        {
            if (noise < 40)
                return 3;
            else if (noise >= 40 && noise < 80)
                return 2;
            else
                return 0;
        }
        else if (distanceToCenter < size)
        {
            if (noise < 40)
                return 2;
            else if (noise >= 40 && noise < 70)
                return 1;
            else
                return 0;
        }
        else if (distanceToCenter < 1.2f * size)
        {
            if (noise < 50)
                return 1;
            else
                return 0;
        }
        else
        {
            if (noise < 30)
                return 1;
            else
                return 0;
        }
    }
}
