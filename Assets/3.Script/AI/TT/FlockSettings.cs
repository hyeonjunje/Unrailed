using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Flock Settings", menuName = "ScriptableObject/Flock Settings")]
public class FlockSettings : ScriptableObject
{
    [Header("충돌 대상 레이어")]
    public LayerMask ObstacleMask;

    [Header("회피 반경")]
    public float BoundsRadius = .27f;
    [Header("충돌 감지 거리")]
    public float CollisionAvoidDst = 5;


}
