using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InteractionUI : MonoBehaviour
{
    public Image image;
    public Text text;
    public Sprite[] triggerImage;
    public void GameStart()
    {
        //goto ÈÄ¿¡ ¹Ù²Ü °Í
        SceneManager.LoadScene("TrainScene");
        image.sprite = triggerImage[0];
        //text.color = 
    }
    public void GameExit()
    {
        //goto ÈÄ¿¡ ¹Ù²Ü °Í
        Application.Quit();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        image.sprite = triggerImage[1];
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            image.sprite = triggerImage[0];
    }
}
