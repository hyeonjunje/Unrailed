using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TrackedTarget
{
    public DetectableTarget Detectable;
    public Vector3 RawPosition;


    public float LastSensedTime = -1f;
    public float Awareness;

    //타겟이 있는지 계속 확인
    public bool UpdateAwareness(DetectableTarget target, Vector3 position, float awareness, float minAwareness)
    {
        var oldAwareness = Awareness;

        if (target != null)
            Detectable = target;
        RawPosition = position;
        LastSensedTime = Time.time;

        Awareness = Mathf.Clamp(Mathf.Max(Awareness, minAwareness) + awareness, 0f, 2f);

        //타겟이 있었는데 사라졌다면
        if (oldAwareness < 2f && Awareness >= 2f)
            return true;
        if (oldAwareness < 1f && Awareness >= 1f)
            return true;
        if (oldAwareness <= 0f && Awareness >= 0f)
            return true;

        //아니라면
        return false;
    }

    public bool DecayAwareness(float decayTime, float amount) //시간이 지날수록 인식을 감소시킴
    {
        if ((Time.time - LastSensedTime) <= decayTime) //감지가 최근에 이루어졌음(false)
            return false;

        var oldAwareness = Awareness;

        Awareness -= amount;

        if (oldAwareness >= 2f && Awareness < 2f) //전에 2보다 컸는데 지금은 2 밑임(true)
            return true;
        if (oldAwareness >= 1f && Awareness < 1f) //전에 1보다 컸는데 지금 1 밑임(true)
            return true;
        return Awareness <= 0f;//현재 인식되지 않음(true)
    }

}
//[RequireComponent(typeof(EnemyAI))]
public class AwarenessSystem : MonoBehaviour
{

    //인식의 최소값
    private float _minimumAwareness = 0f;
    //인식의 증가 속도
    private float _awarenessBuildRate = 1f;

    //인식 정도 감소가 시작되기 전의 지연 시간
    private float AwarenessDecayDelay = 0.1f;
    //감소 속도
    private float AwarenessDecayRate = 0.1f;

    private Dictionary<GameObject, TrackedTarget> _targets = new Dictionary<GameObject, TrackedTarget>();
    public Dictionary<GameObject, TrackedTarget> ActiveTargets => _targets;

    private EnemyAI _enemyAI;


    private void Start()
    {
        _enemyAI = GetComponent<EnemyAI>();
    }

    private void Update()
    {
        //여기 업데이트문에서 하는 일은 타겟 청소밖에 없어요

        List<GameObject> toCleanup = new List<GameObject>();
        foreach (var targetGO in _targets.Keys)
        {
            //Debug.Log(_targets[targetGO].Awareness);
            if (_targets[targetGO].DecayAwareness(AwarenessDecayDelay, AwarenessDecayRate * Time.deltaTime))
            {
                //더 이상 인식할 수 없을 때
                if (_targets[targetGO].Awareness <= 0)
                {
                    _enemyAI.OnFullyLost();
                    toCleanup.Add(targetGO);
                }

            }

        }
        // 삭제
        foreach (var target in toCleanup)
            _targets.Remove(target);

    }

    private void UpdateAwareness(GameObject targetGO, DetectableTarget target, Vector3 position, float awareness, float minAwareness)
    {
        //여기서 타겟을 키로 추가
        if (!_targets.ContainsKey(targetGO))
        {
            _targets[targetGO] = new TrackedTarget();

        }
        if (_targets[targetGO].UpdateAwareness(target, position, awareness, minAwareness))
        {
            if (_targets[targetGO].Awareness >= 2f)
            {
                _enemyAI.OnFullyDetected(targetGO); //완전 감지
            }
            else if (_targets[targetGO].Awareness >= 1f) //
                _enemyAI.OnDetected(targetGO);
            else if (_targets[targetGO].Awareness >= 0f)
                _enemyAI.OnSuspicious();
        }
    }

    public void Report(DetectableTarget target)
    {
        //인식 범위내에 있다.
        var awareness = _awarenessBuildRate * Time.deltaTime;
        UpdateAwareness(target.gameObject, target, target.transform.position, awareness, _minimumAwareness);
    }


}
