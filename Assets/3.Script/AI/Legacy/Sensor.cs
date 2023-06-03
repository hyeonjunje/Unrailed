using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    private EnemyAI _enemyAI;

    private void Start()
    {
        _enemyAI = GetComponent<EnemyAI>();
    }

    void Update()
    {
        for (int index = 0; index < DetectableTargetManager.Instance.AllTargets.Count; ++index)
        {
            var candidateTarget = DetectableTargetManager.Instance.AllTargets[index];

            // 스스로일 경우 스킵
            if (candidateTarget.gameObject == gameObject)
                continue;

            //감지 범위(10)안에 들어온 경우
            if (Vector3.Distance(_enemyAI.EyeLocation, candidateTarget.transform.position) <= _enemyAI.DetectionRange)
                _enemyAI.Report(candidateTarget);
        }
    }
}
