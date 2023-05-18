using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AwarenessSystem))]
public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Text _feedbackDisplay;

    private AwarenessSystem _awareness;

    public Vector3 EyeLocation => transform.position;
    public Vector3 EyeDirection => transform.forward;
    //감지 범위
    [Header("감지 범위")]
    [SerializeField] private float _detectionRange = 10f;
    public float DetectionRange => _detectionRange;
   

    private void Awake()
    {
        _awareness = GetComponent<AwarenessSystem>();
    }
    public void OnSuspicious()
    {
        _feedbackDisplay.text = "멀리 있는게 느껴짐";
    }

    public void OnDetected(GameObject target)
    {
        _feedbackDisplay.text = "감지함 // 목표 :" + target.gameObject.name;
    }

    public void OnFullyDetected(GameObject target)
    {
        _feedbackDisplay.text = "찾았다! // 목표 : " + target.gameObject.name;
    }

    public void OnLostDetect(GameObject target)
    {
        _feedbackDisplay.text = "목표 사라짐 //  목표 : " + target.gameObject.name;
    }

    public void OnFullyLost()
    {
        _feedbackDisplay.text = "아무것도 못 찾음";
    }

    public void Report(DetectableTarget target)
    {
        _awareness.Report(target);
    }


}
