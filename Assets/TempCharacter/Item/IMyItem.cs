using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EItemType { wood, steel, rail, pick, axe, bucket, train, animal };

public interface IMyItem
{
    public EItemType ItemType { get; }

    // 줍는 메소드
    public bool PickUp(int count = 0);

    // 버리는 메소드
    public bool PutDown(int count = 0);

    // 체크하는 메소드
    public bool CheckItemType(IMyItem otherItem);
}
