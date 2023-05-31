using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainBox : TrainMovement
{
    public List<Item> woods = new List<Item>();
    public List<Item> steels = new List<Item>();

    [SerializeField] private TrainWorkBench workBench;
    public Player player;

    public int maxItem;
    // Start is called before the first frame update
    void Awake()
    {
        GetMesh();
        trainUpgradeLevel = 2;
        workBench = FindObjectOfType<TrainWorkBench>();
        fireEffect.gameObject.SetActive(false);

        StartCoroutine(Warning()); //첫 목재 석재 넣어라
    }

    // Update is called once per frame
    void Update()
    {
        TrainMovePos();
        if (!isBurn)
        {
            GiveMeItem();
        }
    }
    public override void TrainUpgrade()
    {
        base.TrainUpgrade();
        //업그레이드 메서드
        switch (trainUpgradeLevel)
        {
            case 1:
                maxItem = 3;
                break;
            case 2:
                maxItem = 4;

                break;
            default:
                maxItem = 6;
                break;
        }
    }
    private void RelayItems()
    {
        woods.Remove(woods[0]);
        steels.Remove(steels[0]);
        workBench.MakingRail();
    }
    public void GiveMeItem()
    {
        //아이템 보관은 플레이어 다 만들어지면 ㄱㄱ
        //최대 보유량 3개
        //나무3개 철 3개
        //최대 보유량 매직넘버링 말고 최대값 변수로 구현
        //목재와 철 1개씩 존재할 경우 if문으로 RelayItems 실행
        //RelayItems();
    }
}
