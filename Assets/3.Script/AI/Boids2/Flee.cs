using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flee : AgentBehavior
{
    public override Steering GetSteering()
    {
        Steering steer = new Steering();
        steer.Linear = transform.position - target.transform.position;
        steer.Linear.Normalize();
        steer.Linear = steer.Linear * agent.maxAccel;
        return steer;
    }
}
