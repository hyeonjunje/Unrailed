using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseUI : MonoBehaviour
{
    [SerializeField] private GameObject setPauseObj;
    [SerializeField] private GameObject blurPost;

    private void Awake()
    {
        setPauseObj.SetActive(false);
        blurPost.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !setPauseObj.activeSelf)
        {
            blurPost.SetActive(true);
            setPauseObj.SetActive(true);
        }
    }
    public void GotoLobby()
    {
        SceneManager.LoadScene("IntroScene");
    }
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
  
    public void Respawn()
    {
        //리스폰 나중에 민경씨 완성하면 개발
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void AudioSetting()
    {
        //현준씨 오디오 완성시 개발
    }
    public void Continue()
    {
        blurPost.SetActive(false);
        setPauseObj.SetActive(false);
    }
}
