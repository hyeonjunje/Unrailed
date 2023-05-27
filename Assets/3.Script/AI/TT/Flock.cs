using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    private FlockSettings _settings;


    [HideInInspector]
    public Vector3 Position;
    [HideInInspector]
    public Vector3 Forward;

    [HideInInspector]
    public Vector3 CentreOfFlockmates;
    [HideInInspector]
    public int NumPerceivedFlockmates;

    [HideInInspector]
    public Vector3 Center;
    [HideInInspector]
    public Vector3 TargetPosition;
    [HideInInspector]
    public Vector3 SeparationPosition;



    private Transform _transform;
    private Transform _target;

    private void Awake()
    {
        _transform = transform;

    }
    public void Init(FlockSettings _settings, Transform target)
    {
        this._target = target;
        this._settings = _settings;
        Position = _transform.position;
        Forward = _transform.forward;
    }


    public void UpdateFlock()
    {
        Center = Vector3.zero;
        TargetPosition = Vector3.zero;
        SeparationPosition = Vector3.zero;

        //Cohesion
        if (NumPerceivedFlockmates != 0)
        {
            //무리의 중심 위치(Vector3) / 주위의 무리들 수(int) 나누기 
            CentreOfFlockmates /= NumPerceivedFlockmates;

            //중심에서 내 위치까지의 벡터
            Vector3 offsetToFlockmatesCentre = (CentreOfFlockmates - Position);
            Center = offsetToFlockmatesCentre;

        }
        //정렬

        //Separation 



        Position = _transform.position;
        Forward = _transform.forward;

    }

    public bool IsHeadingForCollision()
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

    public Vector3 ObstacleRays()
    {
        //장애물 회피 
        //여러가지 방향
        Vector3[] rayDirections = BoidHelper.Directions;
        for (int i = 0; i < rayDirections.Length; i++)
        {
            Vector3 dir = _transform.TransformDirection(rayDirections[i]);
            Ray ray = new Ray(Position, dir);
            if (!Physics.SphereCast(ray, _settings.BoundsRadius, _settings.CollisionAvoidDst, _settings.ObstacleMask))
            {
                // 충돌이 발생하지 않는다면 해당 방향 Vector 반환
                return dir;
            }
        }

        //유효한 방향이 없다면 전방 방향 반환 
        return Forward;
    }







}
