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
            Time.timeScale = 0;
        }
    }
    public void GotoLobby()
    {
        SceneManager.LoadScene("IntroScene");
        Time.timeScale = 1;
    }
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }
  
    public void Respawn()
    {
        //������ ���߿� �ΰ澾 �ϼ��ϸ� ����
        Time.timeScale = 1;
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void AudioSetting()
    {
        //���ؾ� ����� �ϼ��� ����
    }
    public void Continue()
    {
        blurPost.SetActive(false);
        setPauseObj.SetActive(false);
        Time.timeScale = 1;
    }
}
