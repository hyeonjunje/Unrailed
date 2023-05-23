using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class BlockTransformerData : ScriptableObject
{
    // 중앙으로 갈 수록 높아지나??
    public bool isHeight;
    public GameObject originBlock;
    public GameObject[] transformedBlock;
}
