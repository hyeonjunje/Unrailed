using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerUI : MonoBehaviour
{
    public Transform followTarget;

    private void Awake()
    {
        followTarget = FindObjectOfType<Player>().transform;
    }
    // Update is called once per frame
    void Update()
    {
        if(followTarget != null)
        transform.position = followTarget.position;
    }
}
