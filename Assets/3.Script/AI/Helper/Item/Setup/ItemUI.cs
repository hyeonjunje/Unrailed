using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    [SerializeField] private Image[] _backGround;
    [SerializeField] private Image _image;
    private EmoteManager _emoteManager;
    private ItemUI2 _ui;
    [HideInInspector]
    public AI_Item _item;
    private void Awake()
    {
        _emoteManager = FindObjectOfType<EmoteManager>();
        _ui = FindObjectOfType<ItemUI2>();
    }
    private void Update()
    {
        CheckItemIsOn();
    }

    public void CheckItemIsOn()
    {
        if(_item==null)
        {
            foreach (var item in ItemManager.Instance.RegisteredObjects)
            {
                if (!item.IsOn && item != _ui._item)
                {
                    foreach (var background in _backGround)
                    {
                        background.enabled = true;
                    }
                    _image.enabled = true;
                    _image.sprite = _emoteManager.GetEmote(item.ID);
                    _item = item;
                    break;
                }
            }
        }

        else if(_item!=null&&_item!=_ui._item&&!_item.IsOn)
        {
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(_item.transform.position + Vector3.up * 1.5f);
            transform.position = screenPosition;

        }
        else if (_item != null && _item.IsOn)
        {
            foreach (var background in _backGround)
            {
                background.enabled = false;
            }
            _image.enabled = false;
            _item = null;
        }


    }
}
