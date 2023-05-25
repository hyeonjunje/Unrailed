using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Boid Settings", menuName ="ScriptableObject/Boid Settings")]
public class BoidSettings : ScriptableObject
{
    public float MinSpeed = 2;
    public float MaxSpeed = 5;
    [Header("무리를 인식하는 범위")]
    public float PerceptionRadius = 2.5f;
    [Header("충돌을 피하기 위해 인식하는 범위")]
    public float AvoidanceRadius = 1;
    [Header("방향 돌리는 힘")]
    public float MaxSteerForce = 3;


    //가중치가 높을수록 해당 특성 강조

    [Header("무리의 평균 이동 방향을 따라가요")]
    public float AlignWeight = 1;
    [Header("무리의 중심 위치를 향해 모여요")]
    public float CohesionWeight = 1;
    [Header("충돌을 피하는게 중요해요")]
    public float SeperateWeight = 1;
    [Header("목표 지점에 빠르게 도착하는게 중요해요")]
    public float TargetWeight = 1;


    [Header("충돌 대상 레이어")]
    public LayerMask ObstacleMask;
    
    //경계 반경, 충돌 회피에 사용됨
    public float BoundsRadius = .27f;
    //충돌을 피하기 위한 가중치
    public float AvoidCollisionWeight = 10;
    //충돌을 감지하기 위한 거리
    public float CollisionAvoidDst = 5;
}
