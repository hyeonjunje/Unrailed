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
    public int WoodEmote = 10;
    [HideInInspector]
    public int WaterEmote = 11;
    [HideInInspector]
    public int StoneEmote = 12;
    [HideInInspector]
    public int ResourceEmote = 13;



    Dictionary<int, Sprite> _portraitData;
    public Sprite[] Portrait;

    private void Awake()
    {
        _portraitData = new Dictionary<int, Sprite>();
        Init();
    }

    private void Init()
    {
        _portraitData.Add(AxeEmote, Portrait[0]);
        _portraitData.Add(PickEmote, Portrait[1]);
        _portraitData.Add(BucketEmote, Portrait[2]);
        _portraitData.Add(WarningEmote, Portrait[3]);

        _portraitData.Add(WoodEmote, Portrait[4]);
        _portraitData.Add(WaterEmote, Portrait[5]);
        _portraitData.Add(StoneEmote, Portrait[6]);
        _portraitData.Add(ResourceEmote, Portrait[7]);
    }

    public Sprite GetEmote(int id)
    {
        return _portraitData[id];
    }
}
