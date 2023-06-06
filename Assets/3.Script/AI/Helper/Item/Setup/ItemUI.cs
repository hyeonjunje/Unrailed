using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    [SerializeField] private Image[] _backGround;

    [SerializeField] private Image _image;

    private EmoteManager _emoteManager;
    private AI_Item _item;
    private void Awake()
    {
        _emoteManager = FindObjectOfType<EmoteManager>();
    }

    void Update()
    {
        CheckItemIsOn();
    }


    private void CheckItemIsOn()
    {
        foreach (var item in ItemManager.Instance.RegisteredObjects)
        {
            //아이템이 바닥에 있다면
            if (!item.IsOn)
            {
                foreach(var background in _backGround)
                {
                    background.enabled = true;
                }

                _image.sprite = _emoteManager.GetEmote(item.ID);
                Vector3 screenPosition = Camera.main.WorldToScreenPoint(item.transform.position+Vector3.up*1.5f);
                transform.position = screenPosition;
                break;
            }
            else
            {
                foreach (var background in _backGround)
                {
                    background.enabled = false;
                }

            }
        }
    }
}
