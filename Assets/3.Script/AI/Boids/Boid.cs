using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    BoidSettings settings;

    [HideInInspector]
    public Vector3 position;
    [HideInInspector]
    public Vector3 forward;
    Vector3 velocity;

    //이동 방향
    [HideInInspector]
    public Vector3 avgFlockHeading;

    //평균 회피 방향
    [HideInInspector]
    public Vector3 avgAvoidanceHeading;

    //무리 중심 위치를 나타내는 벡터
    [HideInInspector]
    public Vector3 centreOfFlockmates;

    //인식된 주변의 Boids 수
    [HideInInspector]
    public int numPerceivedFlockmates;

    Transform cachedTransform;
    Transform target;

    void Awake()
    {
        cachedTransform = transform;
    }

    public void Initialize(BoidSettings settings, Transform target)
    {
        this.settings = settings;
        this.target = target;

        position = cachedTransform.position;
        forward = cachedTransform.forward;

        float startSpeed = (settings.minSpeed + settings.maxSpeed) / 2;
        velocity = transform.forward * startSpeed;
    }

    public void UpdateBoid()
    {
        //가속도 초기화
        Vector3 acceleration = Vector3.zero;

        //타겟이 있다면
        if (target != null)
        {
            Vector3 offsetToTarget = (target.position - position);

            //가속도 = 속도 계산(타겟과 나의 거리) * target쪽으로 이동하는게 얼마나 중요한지 값
            acceleration = SteerTowards(offsetToTarget) * settings.targetWeight;
        }

        //주위에 무리가 하나도 없다면
        if (numPerceivedFlockmates != 0)
        {
            //무리의 중심 위치(Vector3) / 주위의 무리들 수(int) 나누기 
            centreOfFlockmates /= numPerceivedFlockmates;

            //중심에서 내 위치까지의 벡터
            Vector3 offsetToFlockmatesCentre = (centreOfFlockmates - position);

            //무리로 향하는 힘
            var alignmentForce = SteerTowards(avgFlockHeading) * settings.alignWeight;
            //중심으로 향하는 힘 
            var cohesionForce = SteerTowards(offsetToFlockmatesCentre) * settings.cohesionWeight;
            //다른 Boid를 회피하는 힘
            var seperationForce = SteerTowards(avgAvoidanceHeading) * settings.seperateWeight;


            //다 더해주기
            acceleration += alignmentForce;
            acceleration += cohesionForce;
            acceleration += seperationForce;
        }

        //충돌 가능성이 있다면
        if (IsHeadingForCollision())
        {
            //충돌이 발생하지 않는 방향 
            Vector3 collisionAvoidDir = ObstacleRays();

            //충돌을 피하기 위한 가중치 * 속도(방향)
            Vector3 collisionAvoidForce = SteerTowards(collisionAvoidDir) * settings.avoidCollisionWeight;
            //더해주기
            acceleration += collisionAvoidForce;
        }

        //움직이게 하기
        velocity += acceleration * Time.deltaTime;
        //이동 속도
        float speed = velocity.magnitude;
        //이동 방향
        Vector3 dir = velocity / speed;
        //이동 속도 제한
        speed = Mathf.Clamp(speed, settings.minSpeed, settings.maxSpeed);
        //최종 이동 속도
        velocity = dir * speed;

        //위치 업데이트
        cachedTransform.position += velocity * Time.deltaTime;
        //cachedTransform.forward = 
        position = cachedTransform.position;
        forward = Vector3.forward;
        //forward = dir;
    }

    bool IsHeadingForCollision()
    {
        //현재 진행 방향이 충돌 가능성이 있는지 검사
        //충돌 가능성이 있다면 회피 동작을 실행할 수 있도록 

        RaycastHit hit;
        if (Physics.SphereCast(position, settings.boundsRadius, forward, out hit, settings.collisionAvoidDst, settings.obstacleMask))
        {
            //현재 위치에서 앞쪽으로 구체 발사
            //충돌이 발생하면 true 반환
            return true;
        }
        // 충돌하지 않았다면 false 반환
        else { }
        return false;
    }

    Vector3 ObstacleRays()
    {
        //장애물 회피 

        Vector3[] rayDirections = BoidHelper.directions;

        for (int i = 0; i < rayDirections.Length; i++)
        {
            Vector3 dir = cachedTransform.TransformDirection(rayDirections[i]);
            Ray ray = new Ray(position, dir);
            if (!Physics.SphereCast(ray, settings.boundsRadius, settings.collisionAvoidDst, settings.obstacleMask))
            {
                // 구체의 반지름, 검사거리, 장애물 마스크
                // 충돌이 발생하지 않는다면 해당 방향 Vector 반환
                return dir;
            }
        }

        //유효한 방향이 없다면 전방 방향 반환 
        return forward;
    }

    Vector3 SteerTowards(Vector3 vector)
    {
        //속도 계산 

        //vector을 정규화 한 값 * 최대 속도 - 현재 속도(속도 조절)
        Vector3 v = vector.normalized * settings.maxSpeed - velocity;
        //v값은 maxSteerForce로 제한 ( 갑자기 방향 바꾸지 않게 조절해주는 역할 ) 
        //계산된 속도 반환
        return Vector3.ClampMagnitude(v, settings.maxSteerForce);
    }
}
