using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlockData
{
    public string blockName;
    public Sprite blockSprite;
    public GameObject blockPrefab;
}

[CreateAssetMenu()]
public class BlocksData : ScriptableObject
{
    public List<BlockData> blocksData;  
}
