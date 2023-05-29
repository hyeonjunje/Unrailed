using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Flock Settings", menuName = "ScriptableObject/Flock Settings")]
public class FlockSettings : ScriptableObject
{
    [Header("무리를 인식하는 범위")]
    public float PerceptionRadius = 2.5f;
    [Header("충돌을 피하기 위해 인식하는 범위")]
    public float AvoidanceRadius = 1;


    [Header("충돌 대상 레이어")]
    public LayerMask ObstacleMask;

    [Header("회피 반경")]
    public float BoundsRadius = .27f;
    [Header("충돌 감지 거리")]
    public float CollisionAvoidDst = 5;


}
