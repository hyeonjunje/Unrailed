using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailSpawn : MonoBehaviour
{

    public List<GameObject> obj=  new List<GameObject>();
    float timeSpawn;
    [SerializeField] int countUp;
    private void Awake()
    {
        foreach (GameObject child in obj)
        {
            child.SetActive(false);
        }
        obj.Reverse();
    }
    public void Update()
    {
        timeSpawn += Time.deltaTime;
        if(timeSpawn > .5f)
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

        obj[countUp].SetActive(true);
        countUp++;

     
    }
}
