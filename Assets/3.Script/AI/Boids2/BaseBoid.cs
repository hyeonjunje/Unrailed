using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBoid : MonoBehaviour
{
    public SeekBoid Seek;

    public Seek seek;

    public Agent Agent;
    public Flee Flee;
    public float MaxSpeed;

    public GameObject Target;
    public BoidSeparation Separation;
    public BoidCohesion Cohesion;

    public UnitFSM state;
    public Agent agentScript;

    public float maxSpeed;


    public enum UnitFSM //states
    {
        Seek,
        Idle
    }
    void Start()
    {
        agentScript = gameObject.AddComponent<Agent>(); //add agent
        agentScript.maxSpeed = maxSpeed;

        changeState(UnitFSM.Idle);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            if (state == UnitFSM.Idle)
            {
                changeState(UnitFSM.Seek);
            }
            else
            {
                changeState(UnitFSM.Idle);
            }
        }
    }

    public void changeState(UnitFSM new_state)
    {

        state = new_state;

/*        switch (new_state)
        {
            case UnitFSM.Idle:

                if (gameObject.GetComponent<idle_script>() == null)
                {
                    idle = gameObject.AddComponent<idle_script>();
                }
                DestroyImmediate(seek);

                break;

            case UnitFSM.Seek:

                if (gameObject.GetComponent<Seek>() == null)
                {
                    seek = gameObject.AddComponent<Seek>();
                }
                DestroyImmediate(idle);

                break;


        }*/
    }

}
