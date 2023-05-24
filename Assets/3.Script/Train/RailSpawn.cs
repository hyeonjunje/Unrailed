using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailSpawn : MonoBehaviour
{
    public List<GameObject> obj=  new List<GameObject>();

    float timeSpawn;
    [SerializeField] int countUp;
    [SerializeField] RailPooling poolRail;
    
    private void Awake()
    {
        poolRail = FindObjectOfType<RailPooling>();
    }
    public void Update()
    {
        timeSpawn += Time.deltaTime;
        if(timeSpawn > .1f)
        {
            TimeUp();
            timeSpawn = 0;
        }
    }

    void TimeUp()
    {

        if (countUp >= obj.Count)
        {
            return;
        }

        poolRail.TransformRail(obj[countUp].transform.position);
        //obj[countUp].transform.position = poolRail.
        countUp++;

     
    }
}
