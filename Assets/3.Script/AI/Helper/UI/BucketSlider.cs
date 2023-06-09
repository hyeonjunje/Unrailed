using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BucketSlider : MonoBehaviour
{
    private Transform _bucketTransform;
    private void Awake()
    {
        _bucketTransform = FindObjectOfType<Item_Bucket>().transform;
    }

    void Update()
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(_bucketTransform.position+Vector3.right*0.5f);
        transform.position = screenPosition;
    }
}
