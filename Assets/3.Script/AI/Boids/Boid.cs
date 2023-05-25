using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    private BoidSettings _settings;

    [HideInInspector]
    public Vector3 Position;
    [HideInInspector]
    public Vector3 Forward;

    private Vector3 _velocity;

    //이동 방향
    [HideInInspector]
    public Vector3 AvgFlockHeading;

    //평균 회피 방향
    [HideInInspector]
    public Vector3 AvgAvoidanceHeading;

    //무리 중심 위치를 나타내는 벡터
    [HideInInspector]
    public Vector3 CentreOfFlockmates;

    //인식된 주변의 Boids 수
    [HideInInspector]
    public int NumPerceivedFlockmates;

    private Transform _cachedTransform;
    private Transform _target;

    void Awake()
    {
        _cachedTransform = transform;
    }

    public void Initialize(BoidSettings _settings, Transform _target)
    {
        this._settings = _settings;
        this._target = _target;

        Position = _cachedTransform.position;
        Forward = _cachedTransform.forward;

        float startSpeed = (_settings.MinSpeed + _settings.MaxSpeed) / 2;
        _velocity = transform.forward * startSpeed;
    }

    public void UpdateBoid()
    {
        //가속도 초기화
        Vector3 acceleration = Vector3.zero;

        //타겟이 있다면
        if (_target != null)
        {
            Vector3 offsetToTarget = (_target.position - Position);

            //가속도 = 속도 계산(타겟과 나의 거리) * _target쪽으로 이동하는게 얼마나 중요한지 값
            acceleration = SteerTowards(offsetToTarget) * _settings.TargetWeight;
        }

        //주위에 무리가 하나도 없다면
        if (NumPerceivedFlockmates != 0)
        {
            //무리의 중심 위치(Vector3) / 주위의 무리들 수(int) 나누기 
            CentreOfFlockmates /= NumPerceivedFlockmates;

            //중심에서 내 위치까지의 벡터
            Vector3 offsetToFlockmatesCentre = (CentreOfFlockmates - Position);

            //무리로 향하는 힘
            var alignmentForce = SteerTowards(AvgFlockHeading) * _settings.AlignWeight;
            //중심으로 향하는 힘 
            var cohesionForce = SteerTowards(offsetToFlockmatesCentre) * _settings.CohesionWeight;
            //다른 Boid를 회피하는 힘
            var seperationForce = SteerTowards(AvgAvoidanceHeading) * _settings.SeperateWeight;


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
            Vector3 collisionAvoidForce = SteerTowards(collisionAvoidDir) * _settings.AvoidCollisionWeight;
            //더해주기
            acceleration += collisionAvoidForce;
        }

        //움직이게 하기
        _velocity += acceleration * Time.deltaTime;
        //이동 속도
        float speed = _velocity.magnitude;
        //이동 방향
        Vector3 dir = _velocity / speed;
        //이동 속도 제한
        speed = Mathf.Clamp(speed, _settings.MinSpeed, _settings.MaxSpeed);
        //최종 이동 속도
        _velocity = dir * speed;

        //위치 업데이트
        _cachedTransform.position += _velocity * Time.deltaTime;
        Position = _cachedTransform.position;
        Forward = Vector3.forward;
    }

    bool IsHeadingForCollision()
    {
        //현재 진행 방향이 충돌 가능성이 있는지 검사
        //충돌 가능성이 있다면 회피 동작을 실행할 수 있도록 

        RaycastHit hit;
        if (Physics.SphereCast(Position, _settings.BoundsRadius, Forward, out hit, _settings.CollisionAvoidDst, _settings.ObstacleMask))
        {
            //현재 위치에서 앞쪽으로 구체 발사
            //충돌이 발생하면 true 반환
            return true;
        }
        // 충돌하지 않았다면 false 반환
        else return false;
    }

    Vector3 ObstacleRays()
    {
        //장애물 회피 

        Vector3[] rayDirections = BoidHelper.Directions;

        for (int i = 0; i < rayDirections.Length; i++)
        {
            Vector3 dir = _cachedTransform.TransformDirection(rayDirections[i]);
            Ray ray = new Ray(Position, dir);
            if (!Physics.SphereCast(ray, _settings.BoundsRadius, _settings.CollisionAvoidDst, _settings.ObstacleMask))
            {
                // 구체의 반지름, 검사거리, 장애물 마스크
                // 충돌이 발생하지 않는다면 해당 방향 Vector 반환
                return dir;
            }
        }

        //유효한 방향이 없다면 전방 방향 반환 
        return Forward;
    }

    Vector3 SteerTowards(Vector3 vector)
    {
        //속도 계산 

        //vector을 정규화 한 값 * 최대 속도 - 현재 속도(속도 조절)
        Vector3 v = vector.normalized * _settings.MaxSpeed - _velocity;

        //계산된 속도 반환
        return Vector3.ClampMagnitude(v, _settings.MaxSteerForce);
        //v값은 MaxSteerForce로 제한 ( 갑자기 방향 바꾸지 않게 조절해주는 역할 ) 
    }
}
