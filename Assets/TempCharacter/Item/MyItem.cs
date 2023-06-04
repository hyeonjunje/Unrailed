using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EItemType { wood, steel, rail, pick, axe, bucket, train, animal };
public enum EEquipPart { rightHand, twoHand }

public abstract class MyItem : MonoBehaviour
{
    [SerializeField] private EEquipPart _equipPart;
    [SerializeField] protected float stackInterval = 0.15f;
    [SerializeField] protected EItemType itemType;
    [SerializeField] protected LayerMask blockLayer;

    [HideInInspector] public Transform equipment;

    protected PlayerController player;

    protected virtual void Awake()
    {
        player = FindObjectOfType<PlayerController>();

        if (player == null)
            return;

        switch (_equipPart)
        {
            case EEquipPart.rightHand:
                equipment = player.RightHandTransform;
                break;
            case EEquipPart.twoHand:
                equipment = player.TwoHandTransform;
                break;
        }
    }


    public EItemType ItemType => itemType;

    public abstract Pair<Stack<MyItem>, Stack<MyItem>> PickUp(Stack<MyItem> handItem, Stack<MyItem> detectedItem);  // 줍는 메소드
    public abstract Pair<Stack<MyItem>, Stack<MyItem>> PutDown(Stack<MyItem> handItem, Stack<MyItem> detectedItem);  // 버리는 메소드
    public abstract Pair<Stack<MyItem>, Stack<MyItem>> Change(Stack<MyItem> handItem, Stack<MyItem> detectedItem);   // 교체하는 메소드
    public abstract Pair<Stack<MyItem>, Stack<MyItem>> AutoGain(Stack<MyItem> handItem, Stack<MyItem> detectedItem);  // 자동으로 먹는 메소드

    public virtual void RePosition(Transform parent, Vector3 pos)
    {
        transform.SetParent(parent);
        transform.localPosition = pos;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    public virtual bool CheckItemType(MyItem item)
    {
        return itemType.Equals(item.ItemType);
    }


    // 현재 위치 앞뒤좌우에 가장 최신 레일이 있으면 true, 없으면 false
    // true면 설치할 수 있습니다.
    public virtual bool IsInstallable()
    {
        Vector3[] dir = new Vector3[4] { Vector3.forward, Vector3.right, Vector3.left, Vector3.back };
        for (int i = 0; i < dir.Length; i++)
        {
            if (Physics.Raycast(player.CurrentBlockTransform.position, dir[i], out RaycastHit hit, 1f, blockLayer))
            {
                if (hit.transform.childCount > 0)
                {
                    RailController rail = hit.transform.GetChild(0).GetComponent<RailController>();
                    if (rail != null)
                    {
                        if (rail == FindObjectOfType<GoalManager>().lastRail)
                            return true;
                    }
                }
            }
        }
        return false;
    }


    // 검사할 레일이 이미 설치되어 있거나 가장 최신레일이면 false 아니면 true
    // true면 상호작용할 수 있다.
    public virtual bool IsRailInteractive(RailController rail)
    {
        if (rail == null)
            return true;

        return rail != FindObjectOfType<GoalManager>().lastRail && rail.isInstance != true;
    }


    public virtual void ActiveWater(bool falg)
    {

    }
}
