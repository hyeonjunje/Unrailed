using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    [HideInInspector]
    public Vector3 Position;

    [HideInInspector]
    public Vector3 CentreOfFlockmates;

    [HideInInspector]
    public int NumPerceivedFlockmates;

    [HideInInspector]
    public Vector3 Center;
    [HideInInspector]
    public Vector3 TargetPosition;

    private Transform _transform;
    private Transform _target;

    private void Awake()
    {
        _transform = transform;

    }
    public void Init(Transform target)
    {
        this._target = target;
        Position = _transform.position;
    }

    public void UpdateFlock()
    {
        Center = Vector3.zero;
        TargetPosition = Vector3.zero;

        Position = _transform.position;

        if (NumPerceivedFlockmates != 0)
        {
            //무리의 중심 위치(Vector3) / 주위의 무리들 수(int) 나누기 
            CentreOfFlockmates /= NumPerceivedFlockmates;

            //중심에서 내 위치까지의 벡터
            Vector3 offsetToFlockmatesCentre = (CentreOfFlockmates - Position);
            Center = offsetToFlockmatesCentre;

        }

        if(_target!=null)
        {
            TargetPosition = _target.position;
        }
    }
}
