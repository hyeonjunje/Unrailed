using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidSeparation : Flee
{
    public float DesiredSeparation = 5;
    public List<GameObject> targets;

    public override Steering GetSteering()
    {
        Steering steer = new Steering();
        int count = 0;

        //for each boid in the system, check if it is too close
        foreach (GameObject other in targets)
        {
            if (other != null)
            {
                float d = (transform.position - other.transform.position).magnitude;

                if ((d > 0) && (d < DesiredSeparation))
                {
                    //calculate vector pointing away from neighbor
                    Vector3 diff = transform.position - other.transform.position;
                    diff.Normalize();
                    diff /= d;
                    steer.Linear += diff;
                    count++;
                }
            }
        }//end for

        if (count > 0)
        {
            //steer.linear /= (float)count;
        }

        return steer;

    }

}
