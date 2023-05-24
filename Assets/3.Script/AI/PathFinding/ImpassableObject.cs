using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpassableObject : MonoBehaviour
{
    // ToDo : 20230524
    // 나무1, 2, 돌, 철, 물블럭, empty prefab에다가 다 ImpassableObject넣어야 함

    // PathFindingField는 awake에서 생성되므로 start에서 실행
    public Vector3 originPos;

    private void Start()
    {
        originPos = new Vector3(transform.position.x, 0, transform.position.z);

        if(PathFindingField.Instance != null)
            PathFindingField.Instance.UpdateMapData(originPos.x, originPos.z, false);
    }

    // 이 물체가 파괴되면 PathFindingField에서 해당 좌표를 갈 수 있는곳으로 변경
    private void OnDestroy()
    {
        if(PathFindingField.Instance != null)
            PathFindingField.Instance.UpdateMapData(originPos.x, originPos.z, true);
    }
}
