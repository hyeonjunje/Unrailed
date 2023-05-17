using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainWheelRotate : MonoBehaviour
{

    void Update()
    {
        transform.Rotate(Vector3.right * 200 * Time.deltaTime);
    }
}
