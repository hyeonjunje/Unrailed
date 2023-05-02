using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rigid;
    [SerializeField] private float speed = 10f;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 velocity = new Vector3(x, 0, z);
        velocity *= speed;
        rigid.velocity = velocity;
    }
}
