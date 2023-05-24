using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpassableObject : MonoBehaviour
{
    // ToDo : 20230524
    // 나무1, 2, 돌, 철, 물블럭, empty prefab에다가 다 ImpassableObject넣어야 함

    // PathFindingField는 awake에서 생성되므로 start에서 실행
    private void Start()
    {
        PathFindingField.Instance.UpdateMapData(transform.position.x, transform.position.z, false);
    }

    // 이 물체가 파괴되면 PathFindingField에서 해당 좌표를 갈 수 있는곳으로 변경
    private void OnDestroy()
    {
        PathFindingField.Instance.UpdateMapData(transform.position.x, transform.position.z, true);
    }
}
