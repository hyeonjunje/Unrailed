using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainWheelRotate : MonoBehaviour
{
    [SerializeField] bool isBolt;

    [SerializeField] float speed = 2;
    void Update()
    {
        if (!isBolt)
        {
            transform.Rotate(Vector3.right * 200 * Time.deltaTime * speed);
        }
        else
        {
            transform.Rotate(Vector3.up * 100 * Time.deltaTime);
        }
    }
}
