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
    private void Awake()
    {
        image.sprite = triggerImage[0];
        //text.color = 
    }
    public void GameStart()
    {
        //goto ÈÄ¿¡ ¹Ù²Ü °Í
        SceneManager.LoadScene("TrainScene");
  
    }
    public void GameExit()
    {
        //goto ÈÄ¿¡ ¹Ù²Ü °Í
        Application.Quit();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            image.sprite = triggerImage[1];
            text.color = new Color32(255, 196, 118, 255);
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            image.sprite = triggerImage[0];
            text.color = Color.white;
        }
    }
}
