using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sphereRay : MonoBehaviour
{
    private void Start()
    {
        RaycastHit hitData;

        //Debug.Log(Physics.Raycast(transform.position, Vector3.up, out hitData, 1000f));
        //Debug.Log(hitData.transform.name);


        Debug.Log(Physics.Raycast(transform.position, Vector3.down, out hitData, 1000f));
        Debug.Log(hitData.transform.name);
        Debug.DrawLine(transform.position, Vector3.down * 1000f, Color.red);

        Debug.Log(Physics.Raycast(transform.position, Vector3.left, out hitData, 1000f));
        Debug.Log(hitData.transform.name);
        Debug.DrawLine(transform.position, Vector3.left * 1000f, Color.red);


        Debug.Log(Physics.Raycast(transform.position, Vector3.right, out hitData, 1000f));
        Debug.Log(hitData.transform.name);
        Debug.DrawLine(transform.position, Vector3.right * 1000f, Color.red);


        Debug.Log(Physics.Raycast(transform.position, Vector3.forward, out hitData, 1000f));
        Debug.Log(hitData.transform.name);
        Debug.DrawLine(transform.position, Vector3.forward * 1000f, Color.red);


        Debug.Log(Physics.Raycast(transform.position, Vector3.back, out hitData, 1000f));
        Debug.Log(hitData.transform.name);
        Debug.DrawLine(transform.position, Vector3.back * 1000f, Color.red);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
