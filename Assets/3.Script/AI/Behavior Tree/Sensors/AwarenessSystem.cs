using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TrackedTarget
{
    public DetectableTarget Detectable;
    public Vector3 RawPosition;


    public float LastSensedTime = -1f;
    public float Awareness;

    //Ÿ���� �ִ��� ��� Ȯ��
    public bool UpdateAwareness(DetectableTarget target, Vector3 position, float awareness, float minAwareness)
    {
        var oldAwareness = Awareness;

        if (target != null)
            Detectable = target;
        RawPosition = position;
        LastSensedTime = Time.time;

        Awareness = Mathf.Clamp(Mathf.Max(Awareness, minAwareness) + awareness, 0f, 2f);

        //Ÿ���� �־��µ� ������ٸ�
        if (oldAwareness < 2f && Awareness >= 2f)
            return true;
        if (oldAwareness < 1f && Awareness >= 1f)
            return true;
        if (oldAwareness <= 0f && Awareness >= 0f)
            return true;

        //�ƴ϶��
        return false;
    }

    public bool DecayAwareness(float decayTime, float amount) //�ð��� �������� �ν��� ���ҽ�Ŵ
    {
        if ((Time.time - LastSensedTime) <= decayTime) //������ �ֱٿ� �̷������(false)
            return false;

        var oldAwareness = Awareness;

        Awareness -= amount;

        if (oldAwareness >= 2f && Awareness < 2f) //���� 2���� �Ǵµ� ������ 2 ����(true)
            return true;
        if (oldAwareness >= 1f && Awareness < 1f) //���� 1���� �Ǵµ� ���� 1 ����(true)
            return true;
        return Awareness <= 0f;//���� �νĵ��� ����(true)
    }

}
//[RequireComponent(typeof(EnemyAI))]
public class AwarenessSystem : MonoBehaviour
{

    //�ν��� �ּҰ�
    private float _minimumAwareness = 0f;
    //�ν��� ���� �ӵ�
    private float _awarenessBuildRate = 1f;

    //�ν� ���� ���Ұ� ���۵Ǳ� ���� ���� �ð�
    private float AwarenessDecayDelay = 0.1f;
    //���� �ӵ�
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
        //���� ������Ʈ������ �ϴ� ���� Ÿ�� û�ҹۿ� �����

        List<GameObject> toCleanup = new List<GameObject>();
        foreach (var targetGO in _targets.Keys)
        {
            //Debug.Log(_targets[targetGO].Awareness);
            if (_targets[targetGO].DecayAwareness(AwarenessDecayDelay, AwarenessDecayRate * Time.deltaTime))
            {
                //�� �̻� �ν��� �� ���� ��
                if (_targets[targetGO].Awareness <= 0)
                {
                    _enemyAI.OnFullyLost();
                    toCleanup.Add(targetGO);
                }

            }

        }
        // ����
        foreach (var target in toCleanup)
            _targets.Remove(target);

    }

    private void UpdateAwareness(GameObject targetGO, DetectableTarget target, Vector3 position, float awareness, float minAwareness)
    {
        //���⼭ Ÿ���� Ű�� �߰�
        if (!_targets.ContainsKey(targetGO))
        {
            _targets[targetGO] = new TrackedTarget();

        }
        if (_targets[targetGO].UpdateAwareness(target, position, awareness, minAwareness))
        {
            if (_targets[targetGO].Awareness >= 2f)
            {
                _enemyAI.OnFullyDetected(targetGO); //���� ����
            }
            else if (_targets[targetGO].Awareness >= 1f) //
                _enemyAI.OnDetected(targetGO);
            else if (_targets[targetGO].Awareness >= 0f)
                _enemyAI.OnSuspicious();
        }
    }

    public void Report(DetectableTarget target)
    {
        //�ν� �������� �ִ�.
        var awareness = _awarenessBuildRate * Time.deltaTime;
        UpdateAwareness(target.gameObject, target, target.transform.position, awareness, _minimumAwareness);
    }


}
