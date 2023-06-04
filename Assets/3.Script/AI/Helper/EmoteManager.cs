using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmoteManager : MonoBehaviour
{
    [HideInInspector]
    public int DefaultEmote = 0;
    [HideInInspector]
    public int AxeEmote = 1;
    [HideInInspector]
    public int PickEmote = 2;
    [HideInInspector]
    public int BucketEmote = 3;
    [HideInInspector]
    public int WarningEmote = 4;
    [HideInInspector]
    public int SleepEmote = 5;


    [HideInInspector]
    public int WoodEmote = 10;
    [HideInInspector]
    public int WaterEmote = 11;
    [HideInInspector]
    public int StoneEmote = 12;
    [HideInInspector]
    public int ResourceEmote = 13;


    private Dictionary<int, Sprite> _portraitData;
    [SerializeField] private Sprite[] _portrait;

    private void Awake()
    {
        _portraitData = new Dictionary<int, Sprite>();
        // Init();
    }

    private void Init()
    {
        _portraitData.Add(AxeEmote, _portrait[0]);
        _portraitData.Add(PickEmote, _portrait[1]);
        _portraitData.Add(BucketEmote, _portrait[2]);
        _portraitData.Add(WarningEmote, _portrait[3]);
        _portraitData.Add(SleepEmote, _portrait[4]);

        _portraitData.Add(WoodEmote, _portrait[5]);
        _portraitData.Add(WaterEmote, _portrait[6]);
        _portraitData.Add(StoneEmote, _portrait[7]);
        _portraitData.Add(ResourceEmote, _portrait[8]);
    }

    public Sprite GetEmote(int id)
    {
        return null;
        // return _portraitData[id];
    }
}
