using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentBoid : MonoBehaviour
{
    public GameObject Target;
    public GameObject Boid;
    public List<GameObject> Boids;

    private void Start()
    {
        Boids = new List<GameObject>();

        for(int i=0; i<5; i++)
        {
            Vector3 spawn = new Vector3(i % 4, 0.33f, i / 4);
            GameObject temp = Instantiate(Boid, transform.position + (spawn * 6f), transform.rotation);
            temp.GetComponent<BaseBoid>().Target = gameObject;
            Boids.Add(temp);
        }
    }

    private void Update()
    {
        transform.position += (Target.transform.position - transform.position).normalized * Time.deltaTime * 1.0f;
    }
}
