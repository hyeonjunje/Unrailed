using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCreator : MonoBehaviour
{
    private enum EEnvironment
    {
        grass = 1,
        field,
        rock,
        breakableRock,
        tree1,
        tree2,
        water,
    }

    [Header("Prefabs")]
    [SerializeField] private GameObject[] prefabs;

    [Header("Level")]
    [SerializeField] private Transform _level;

    public void CreateMap(Block[,] mapData)
    {
        for(int i = 0; i < mapData.GetLength(0); i++)
        {
            for(int j = 0; j < mapData.GetLength(1); j++)
            {
                int index = mapData[i, j].Index;
                if(mapData[i, j].Index != 0)
                {
                    if(prefabs[index] != null)
                    {
                        GameObject go = Instantiate(prefabs[index], _level);
                        go.transform.position = mapData[i, j].transform.position + Vector3.up;
                    }
                    // ¹°
                    if(index == 7)
                    {
                        mapData[i, j].transform.localScale -= Vector3.up * 0.8f;
                    }
                }
            }
        }
    }
}
