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
        workBench = FindObjectOfType<TrainWorkBench>();
        fireEffect.gameObject.SetActive(false);

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
