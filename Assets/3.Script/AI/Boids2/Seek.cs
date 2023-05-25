using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seek : MonoBehaviour
{
    BaseBoid bb;
    GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        bb = gameObject.GetComponent<BaseBoid>();
        target = bb.Target;


        if (bb.Seek == null)
        {
            bb.Seek = gameObject.AddComponent<SeekBoid>();
            bb.Seek.target = target;
            bb.Seek.weight = 0.7f;
            bb.Seek.enabled = true;


            bb.Cohesion = gameObject.AddComponent<BoidCohesion>();
            bb.Cohesion.targets = bb.Target.GetComponent<ParentBoid>().Boids;
            bb.Cohesion.weight = 0.4f;
            bb.Cohesion.enabled = true;

            bb.Separation = gameObject.AddComponent<BoidSeparation>();
            bb.Separation.targets = bb.Target.GetComponent<ParentBoid>().Boids;
            bb.Separation.weight = 70.0f;
            bb.Separation.enabled = true;

        }

    }


    private void OnDestroy()
    {
        DestroyImmediate(bb.Seek);
    }


    private void OnDrawGizmos()
    {
        UnityEditor.Handles.Label(transform.position + Vector3.up * 3, "Seek");
    }
}
