using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBase : MonoBehaviour
{
    [SerializeField] protected Image[] _backGround;
    [SerializeField] protected Image _image;
    protected EmoteManager _emoteManager;
    protected AI_Item _item;

    private void Awake()
    {
        _emoteManager = FindObjectOfType<EmoteManager>();
    }

    protected virtual void Update()
    {
        if(_item!=null)
        {
            if (!_item.IsOn)
            {
                foreach (var background in _backGround)
                {
                    background.enabled = true;
                }
                _image.enabled = true;
                _image.sprite = _emoteManager.GetEmote(_item.ID);

                Vector3 screenPosition = Camera.main.WorldToScreenPoint(_item.transform.position + Vector3.up * 1.5f);
                transform.position = screenPosition;
            }

            else
            {
                _image.enabled = false;
                foreach (var background in _backGround)
                {
                    background.enabled = false;
                }
            }

        }
    }
}