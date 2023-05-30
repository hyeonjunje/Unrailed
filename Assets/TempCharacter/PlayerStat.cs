using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 이건 스크립터블 오브젝트로 짜는게 좋을듯
public class PlayerStat : MonoBehaviour
{
    [Header("이동")]
    public float moveSpeed;
    public float dashSpeed;
    public float dashDuration;
    public float dashCoolTime;

    [Header("아이템")]
    public int handAmount;
    public int stackAmount;
    public float stackinterval;

    [Header("감지 거리")]
    public float detectRange;

    [Header("레이어")]
    public LayerMask blockLayer;
}
