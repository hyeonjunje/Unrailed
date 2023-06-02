using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerUI : MonoBehaviour
{
    public Transform followTarget;
    public Transform followCam;

    [SerializeField] bool EmotUI;
    private void Awake()
    {
        SoundManager.Instance.StopAllSound();
        SoundManager.Instance.PlaySoundBgm("Lobby_Bgm");

        followTarget = FindObjectOfType<Player>().transform;

        if (EmotUI)
        followCam = GameObject.FindGameObjectWithTag("MainCamera").transform;

 
    }
    // Update is called once per frame
    void Update()
    {
        if(followTarget != null)
        transform.position = followTarget.position;

        if (EmotUI)
        {


            Vector3 orderUIVec = new Vector3(followTarget.position.x - -0.5f, followTarget.position.y + 3.87f, followTarget.position.z - 3.11f);
            transform.position = orderUIVec;
        }
    }
}
