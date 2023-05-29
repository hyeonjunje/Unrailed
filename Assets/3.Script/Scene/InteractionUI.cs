using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractionUI : MonoBehaviour
{
    public void GameStart()
    {
        //goto 후에 바꿀 것
        SceneManager.LoadScene("TrainScene");
    }
    public void GameExit()
    {
        //goto 후에 바꿀 것
        Application.Quit();
    }

}
