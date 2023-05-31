using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldResource : MonoBehaviour
{
    public enum EType
    {
        Wood,
        Water,
        Stone
    }
    [SerializeField] EType _Type;
    [SerializeField] int MinAmount = 15;
    [SerializeField] int MaxAmount = 50;

    private float _resourceScale = Mathf.Clamp01(1);
    [SerializeField] float _resourceHp = 2.7f;

    public EType Type => _Type;
    public int AvailableAmount { get; private set; } = 0;

    void Start()
    {
        ResourceTracker.Instance.RegisterResource(this);
        AvailableAmount = Random.Range(MinAmount, MaxAmount);
    }

    public bool isDig()
    {
        _resourceScale -= 0.3f;
        _resourceHp--;
        transform.localScale = new Vector3(_resourceScale, _resourceScale, _resourceScale);
        return _resourceHp > 0 ? true : false;
        
    }
}
