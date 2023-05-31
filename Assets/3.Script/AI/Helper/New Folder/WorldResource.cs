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

    public EType Type => _Type;
    public int AvailableAmount { get; private set; } = 0;

    void Start()
    {
        ResourceTracker.Instance.RegisterResource(this);
        AvailableAmount = Random.Range(MinAmount, MaxAmount);
    }
}
