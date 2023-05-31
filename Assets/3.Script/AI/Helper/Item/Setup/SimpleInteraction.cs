using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleInteraction : BaseInteraction
{
    protected int MaxSimultaneousUsers = 1;
    public override bool CanPerform()
    {
        return true;
    }

    public override bool LockInteraction()
    {
        throw new System.NotImplementedException();
    }

    public override bool UnlockInteraction()
    {
        throw new System.NotImplementedException();
    }
}
