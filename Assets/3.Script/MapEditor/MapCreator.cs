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

    [Header("물 높이")]
    [SerializeField] private float waterHeight = 0.8f;

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
                        if(index == (int)EEnvironment.grass)
                            go.transform.position = mapData[i, j].transform.position + Vector3.up * 0.5f;
                        else
                            go.transform.position = mapData[i, j].transform.position + Vector3.up;
                        mapData[i, j].transform.localScale = Vector3.one;
                    }
                    // 물
                    if(index == (int)EEnvironment.water)
                    {
                        mapData[i, j].transform.localScale = Vector3.one - Vector3.up * (1 - waterHeight);
                        mapData[i, j].transform.localPosition -= Vector3.up * ((1 - waterHeight) / 2);
                    }
                }
            }
        }
    }
}
