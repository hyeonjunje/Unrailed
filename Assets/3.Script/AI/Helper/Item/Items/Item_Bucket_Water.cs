using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_Bucket_Water : MonoBehaviour
{
    public Slider Watergauge;
    public GameObject WaterMesh;
    public GameObject Target;

    public RectTransform UItransform;
    public bool isFilling = false;
    private float currentGauge = Mathf.Clamp(0, 0, 1);
    private Vector3 distance = Vector3.up * 0.4f;
    private float fillSpeed = 50f;
    private float _maxGauge = 100f;


    private void Awake()
    {
        WaterMesh.SetActive(false);
    }

    public void FillGauge()
    {
        if(Watergauge.value<0.9f)
        {
            Watergauge.gameObject.SetActive(true);
            Watergauge.value = Mathf.MoveTowards(Watergauge.value, 0.9f, Time.deltaTime*0.5f);
        }
        else
        {
            WaterMesh.SetActive(true);
            Watergauge.gameObject.SetActive(false);
            Watergauge.value = Mathf.MoveTowards(Watergauge.value, 1f, Time.deltaTime);
        }

    }

    public bool Stop()
    {
        if(Watergauge.value>=1)
        {
            return true;
        }
        return false;
    }

}
