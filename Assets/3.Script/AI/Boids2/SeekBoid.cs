using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekBoid : AgentBehavior
{
    public override Steering GetSteering()
    {
        Steering steer = new Steering();
        steer.Linear = target.transform.position - transform.position;
        steer.Linear.Normalize();
        steer.Linear = steer.Linear * agent.maxAccel;
        return steer;
    }
}
