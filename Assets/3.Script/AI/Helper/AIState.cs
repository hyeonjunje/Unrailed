using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState : MonoBehaviour
{
    public Village Home { get; private set; } = null;
    public float Fear { get; private set; } = 0f;
    public int AmountCarried { get; private set; } = 0;

    public void SetHome(Village _Home)
    {
        Home = _Home;
    }
    public void SetAmountCarried(int amount)
    {
        AmountCarried = amount;
    }
}
