using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Boid Settings", menuName ="ScriptableObject/Boid Settings")]
public class BoidSettings : ScriptableObject
{
    public float minSpeed = 2;
    public float maxSpeed = 5;
    [Header("무리를 인식하는 범위")]
    public float perceptionRadius = 2.5f;
    [Header("충돌을 피하기 위해 인식하는 범위")]
    public float avoidanceRadius = 1;
    [Header("조종력을 가지는 최대 힘의 크기")]
    public float maxSteerForce = 3;


    //가중치가 높을수록 해당 특성 강조

    [Header("무리의 평균 이동 방향을 따라가요")]
    public float alignWeight = 1;
    [Header("무리의 중심 위치를 향해 모여요")]
    public float cohesionWeight = 1;
    [Header("충돌을 피하는게 중요해요")]
    public float seperateWeight = 1;
    [Header("목표 지점에 빠르게 도착하는게 중요해요")]
    public float targetWeight = 1;


    [Header("충돌 대상 레이어")]
    public LayerMask obstacleMask;
    
    //경계 반경, 충돌 회피에 사용됨
    public float boundsRadius = .27f;
    //충돌을 피하기 위한 가중치
    public float avoidCollisionWeight = 10;
    //충돌을 감지하기 위한 거리
    public float collisionAvoidDst = 5;
}
