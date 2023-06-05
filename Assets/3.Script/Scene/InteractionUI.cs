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

    public bool Exit;
    private void Awake()
    {
        image.sprite = triggerImage[0];
        //text.color = 
    }
    public void GameStart()
    {
        if (!Exit)
        {
            //goto ÈÄ¿¡ ¹Ù²Ü °Í
            SoundManager.Instance.PlaySoundEffect("Btn_Click");
            SceneManager.LoadScene("InGame");
        }
       
  
    }
    public void GameExit()
    {
        if (Exit)
        {
            //goto ÈÄ¿¡ ¹Ù²Ü °Í
            SoundManager.Instance.PlaySoundEffect("Btn_Click");
            Application.Quit();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SoundManager.Instance.PlaySoundEffect("Btn_OnMouse");
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
