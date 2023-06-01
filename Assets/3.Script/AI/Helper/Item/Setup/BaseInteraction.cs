using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseInteraction : MonoBehaviour
{
    public abstract bool CanPerform();

    public abstract bool Perform();
    public abstract bool LockInteraction();
    public abstract bool UnlockInteraction();
}
