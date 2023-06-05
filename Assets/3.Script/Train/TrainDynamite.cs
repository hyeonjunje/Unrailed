using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainDynamite : TrainMovement
{

    [SerializeField] private Transform spawnPos;
    [SerializeField] private GameObject prefabs;

    public bool isMake;
    [SerializeField] private float spawnSelect;
    [SerializeField] private float spawnTime;
    [SerializeField] private Text text;
    // Start is called before the first frame update
    void Awake()
    {
        GetMesh();
        spawnTime = spawnSelect;

    }

    // Update is called once per frame
    void Update()
    {
        TrainMovePos();
        //isMake 건들일 수 있게 만들 것 05 25
        if (!isBurn || !isReady)
        {
            if (isMake)
            {
                spawnTime -= Time.deltaTime;

                if (spawnTime <= 0)
                {
                    MakeDynamite();
                    isMake = false;
                    spawnTime = spawnSelect;
                }
            }
            int i = (int)spawnTime;
            text.text = i.ToString("D2");
        }
    
    }

    void MakeDynamite()
    {
        Instantiate(prefabs,spawnPos.position, Quaternion.identity,transform);
    }
    public void RequestDynamite(Player player)
    {
        //플레이어 아이템 제거 player.items.Remove()
        isMake = true;
    }
}
