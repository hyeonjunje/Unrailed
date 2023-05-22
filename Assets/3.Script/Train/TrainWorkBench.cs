using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainWorkBench : TrainMovement
{
    // Start is called before the first frame update

    
    Animator anim;
    [SerializeField] GameObject prefabsRail;

    [SerializeField] Transform[] railSpawnPos;

    public int spawnIndex;
    private void Awake()
    {
        GetMesh();
        fireEffect.gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        TrainMovePos();
        if (!isBurn)
        {
        }
    }

    public void MakingRail()
    {
        //스폰되는 레일 로직 쓰기
        
    }
}