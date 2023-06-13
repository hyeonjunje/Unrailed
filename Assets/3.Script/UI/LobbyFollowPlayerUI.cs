using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyFollowPlayerUI : MonoBehaviour
{
    [SerializeField] private Transform followTarget;
    [SerializeField] private Vector3 offset;

    private void Awake()
    {
        SoundManager.Instance.StopAllSound();
        SoundManager.Instance.PlaySoundBgm("Lobby_Bgm");
    }

    void Update()
    {
        Vector3 orderUIVec = followTarget.position + offset;
        transform.position = orderUIVec;
    }
}
