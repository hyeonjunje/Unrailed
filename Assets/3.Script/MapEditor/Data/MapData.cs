using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapData 
{
    public int mapId;
    public MyArr<int>[] mapData;

    public MapData(int mapId, MyArr<int>[] mapData)
    {
        this.mapId = mapId;
        this.mapData = mapData;
    }
}
