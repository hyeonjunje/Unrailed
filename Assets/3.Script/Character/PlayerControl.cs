using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{

    public float movespeed = 5f;
    float hAxis;
    float vAxis;
    bool wDown;
    private bool ismove = false;
    Animator ani;
    Rigidbody rigid;
    Vector3 moveVec;
    [SerializeField] private float distance = 0f;


    void Start()
    {
        ani = transform.GetComponent<Animator>();
        rigid = transform.GetComponent<Rigidbody>();
    }

    void Update()
    {
        Move();
        DigUp();
        //();
        //RootMove 구현해주세요
    }
    void Move()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;
        transform.position += moveVec * movespeed * Time.deltaTime;
        if (Input.GetButton("Horizontal"))
        {
            ani.SetBool("isMove", moveVec != Vector3.zero);
        }
        if (Input.GetButton("Vertical"))
        {
            ani.SetBool("isMove", moveVec != Vector3.zero);
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            ani.SetBool("isMove", true);
            transform.position += moveVec * 3f * Time.deltaTime;
        }
        else
        {
            ani.SetBool("isMove", false);
        }
        transform.LookAt(transform.position + moveVec);
        if (Input.GetKey(KeyCode.Space))
        {
            ani.SetTrigger("Action");
        }
        if (Input.GetKey(KeyCode.E))
        {
            ani.SetTrigger("Root");
        }
    }

    void DigUp()
    {
        RaycastHit hit;
        Debug.DrawRay(transform.TransformPoint(0, 0.5f, 0), transform.forward * distance, Color.red);
        if (Physics.Raycast(transform.TransformPoint(0, 0.5f, 0), transform.forward, out hit, distance))
        {
            Debug.Log("발사" + hit.transform.name);
            IDig target = hit.collider.GetComponent<IDig>();
            if (target != null)
            {
                target.OnDig(hit.point);
            }
        }
    }
}