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
    public float spawnSpeed;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        GetMesh();
        fireEffect.gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        TrainMovePos();
        anim.SetBool("isBurn", isBurn);
        TrainUpgrade();
    }

    public void MakingRail()
    {
        if (!isBurn)
        {
            anim.SetInteger("GetRails", spawnIndex);
        }
        //스폰되는 레일 로직 쓰기
     
    }
    public override void TrainUpgrade()
    {
        //업그레이드 메서드
        switch (trainUpgradeLevel)
        {
            case 1:
                spawnSpeed = 3;
                break;
            case 2:
                spawnSpeed = 1.8f;

                break;
            case 3:
                spawnSpeed = 0;
                break;
        }
    }
}