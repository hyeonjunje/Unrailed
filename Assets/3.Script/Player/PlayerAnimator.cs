using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public Animator anim;
    private PlayerInput input;
    private PlayerController control;

    private void Awake()
    {
        TryGetComponent(out anim);
        TryGetComponent(out input);
        TryGetComponent(out control);
    }
    // Update is called once per frame
    void Update()
    {
        InputBase();

        if (ShopManager.Instance.isPlayerShop) return;

        PickUpAnim();
    }
    void InputBase()
    {
       if( input.Dir != Vector3.zero)
       {
           anim.SetBool("isMove",true);
       }
       else anim.SetBool("isMove", false);
    }

    private void PickUpAnim()
    {
        if(control.CurrentHandItem == null)
        {
            anim.SetBool("isTwoHandsPickUp", false);
        }

        else
        {
            if(control.CurrentHandItem.EquipPart == EEquipPart.twoHand)
            {
                anim.SetBool("isTwoHandsPickUp", true);
            }
            else
            {
                anim.SetBool("isTwoHandsPickUp", false);
            }
        }
    }
}