using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BoxItem
{
    public BoxItem(EItemType type) { itemType = type; }
    public EItemType itemType;
}

public class TrainBox : TrainMovement
{
    public List<BoxItem> woods = new List<BoxItem>();
    public List<BoxItem> steels = new List<BoxItem>();

    [SerializeField] private TrainWorkBench workBench;
    public Player player;

    public int maxItem;
    public bool madeReady;
    // Start is called before the first frame update
    void Awake()
    {
        GetMesh();
        trainUpgradeLevel = 2;
        workBench = FindObjectOfType<TrainWorkBench>();
        fireEffect.gameObject.SetActive(false);

        StartCoroutine(Warning()); //ù ���� ���� �־��
    }

    // Update is called once per frame
    void Update()
    {
        TrainMovePos();
        TrainUpgrade();
        if (!isBurn)
        {
            
        }
    }
    public override void TrainUpgrade()
    {
        base.TrainUpgrade();
        //���׷��̵� �޼���
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
        workBench.MakingRail();
    } 
    public void GiveMeItem(EItemType itemtype, Stack<MyItem> itemCount)
    {
        switch (itemtype)
        {
            case EItemType.wood:
                    woods.Add(new BoxItem(itemtype));
                itemCount.Peek();
                break;

            case EItemType.steel:
                steels.Add(new BoxItem(itemtype));
                itemCount.Peek();
                break;
        }
    }
}
