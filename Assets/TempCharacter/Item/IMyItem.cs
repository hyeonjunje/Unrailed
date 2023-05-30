using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EItemType { wood, steel, rail, pick, axe, bucket, train, animal };

public interface IMyItem
{
    public EItemType ItemType { get; }

    // �ݴ� �޼ҵ�
    public bool PickUp(int count = 0);

    // ������ �޼ҵ�
    public bool PutDown(int count = 0);

    // üũ�ϴ� �޼ҵ�
    public bool CheckItemType(IMyItem otherItem);
}
