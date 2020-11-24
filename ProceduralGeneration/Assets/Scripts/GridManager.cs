using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField]
    Transform parent;

    [SerializeField]
    List<GameObject> buildingPrefabs;

    [SerializeField, Range(10, 200)]
    int gridSize = 10;

    [SerializeField]
    float buildingSize = 1f;
    // Start is called before the first frame update
    void Start()
    {
        GenerateGrid();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GenerateGrid()
    {
        for(int i = 0;i < gridSize;i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                int buildingToInstantiate = PickBuilding();
                GameObject building = Instantiate(buildingPrefabs[buildingToInstantiate], new Vector3(i*buildingSize, 0, j*buildingSize), Quaternion.identity);
                building.transform.parent = parent;
            }
        }
    }

    private int PickBuilding()
    {
        return Random.Range(0, buildingPrefabs.Count);
    }
}
