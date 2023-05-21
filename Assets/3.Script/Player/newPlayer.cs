using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newPlayer : MonoBehaviour
{

    [SerializeField] float xAxis; 
    [SerializeField] float zAxis;
    [SerializeField] float speed = 5f;
    [SerializeField] float coolTime = 1f;
    Vector3 moveVec;

    bool isDashKeyDown;
    bool isGetItemKeyDown;
    bool isDash;
    bool isHaveItem;

    Rigidbody _rigidbody;
    Animator _animator;


    void Update()
    {
        GetInput();
        Walk();
        Turn();
        Dash();
    }


    void GetInput()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        zAxis = Input.GetAxisRaw("Vertical");
        isDashKeyDown = Input.GetButtonDown("Dash");
        isGetItemKeyDown = Input.GetButtonDown("getItem");
    }

    void Walk()
    {
        if (!isDash) moveVec = new Vector3(xAxis, 0, zAxis).normalized;

        transform.position += moveVec * speed * Time.deltaTime;
        _animator.SetBool("isWalk", moveVec != Vector3.zero);
    }

    void Turn()
    {
        transform.LookAt(moveVec + transform.position);
    }

    void Dash()
    {
        if (isDashKeyDown && !isDash)
        {

            isDash = true;
            speed *= 2;
            Invoke("DashOff", coolTime);
        }
    }

    void DashOff()
    {
        speed *= 0.5f;
        isDash = false;
    }



}
