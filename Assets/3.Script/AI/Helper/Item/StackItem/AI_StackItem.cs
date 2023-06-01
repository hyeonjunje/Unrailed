using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AI_StackItem : MonoBehaviour
{
    [SerializeField] protected float stackInterval = 0.15f;
    [SerializeField] protected EItemType itemType;
    [SerializeField] protected LayerMask blockLayer;

    protected HelperBT _helper;

    protected virtual void Awake()
    {
        _helper = FindObjectOfType<HelperBT>();
    }


    public EItemType ItemType => itemType;

    public abstract Pair<Stack<AI_StackItem>, Stack<AI_StackItem>> PickUp(Stack<AI_StackItem> handItem, Stack<AI_StackItem> detectedItem);  // 줍는 메소드
    public abstract Pair<Stack<AI_StackItem>, Stack<AI_StackItem>> PutDown(Stack<AI_StackItem> handItem, Stack<AI_StackItem> detectedItem);  // 버리는 메소드
    public abstract Pair<Stack<AI_StackItem>, Stack<AI_StackItem>> Change(Stack<AI_StackItem> handItem, Stack<AI_StackItem> detectedItem);   // 교체하는 메소드
    public abstract Pair<Stack<AI_StackItem>, Stack<AI_StackItem>> AutoGain(Stack<AI_StackItem> handItem, Stack<AI_StackItem> detectedItem);  // 자동으로 먹는 메소드

    public virtual void RePosition(Transform parent, Vector3 pos)
    {
        transform.SetParent(parent);
        transform.localPosition = pos;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one * 2;
    }

    public virtual bool CheckItemType(AI_StackItem item)
    {
        return itemType.Equals(item.ItemType);
    }

    public virtual bool CheckConnectedRail()
    {
        RailController result = null;

        Vector3[] dir = new Vector3[4] { Vector3.forward, Vector3.right, Vector3.left, Vector3.back };
        for (int i = 0; i < dir.Length; i++)
        {
            if (Physics.Raycast(_helper.CurrentBlockTransform.position, dir[i], out RaycastHit hit, 1f, blockLayer))
            {
                if (hit.transform.childCount > 0)
                {
                    if (result == null)
                        result = hit.transform.GetChild(0).GetComponent<RailController>();
                }
            }
        }
        return result != null;
    }
}
