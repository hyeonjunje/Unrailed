using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI2 : MonoBehaviour
{
    [SerializeField] private Image[] _backGround;
    [SerializeField] private Image _image;
    private EmoteManager _emoteManager;
    private ItemUI _ui;
    [HideInInspector]
    public AI_Item _item;
    private void Awake()
    {
        _emoteManager = FindObjectOfType<EmoteManager>();
        _ui = FindObjectOfType<ItemUI>();
    }
    void Update()
    {
        CheckItemIsOn();
    }


    private void CheckItemIsOn()
    {
        if (_item == null)
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
                    Vector3 screenPosition = Camera.main.WorldToScreenPoint(item.transform.position + Vector3.up * 1.5f);
                    transform.position = screenPosition;
                    _item = item;
                }
            }

        }
        else
        {
            if (_item.IsOn)
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
}
