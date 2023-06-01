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

    public void Setup(GameObject target)
    {
        this.Target = target;
    }

    private void Awake()
    {
        Setup(Target);
        WaterMesh.SetActive(false);
    }

    void Update()
    {
/*        if (Target != null)
        {
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(Target.transform.position + new Vector3(0, 0.1f, 0));
            UItransform.position = screenPosition + distance;
        }*/

    }
    public bool FillGauge()
    {
        Watergauge.value = currentGauge;
        currentGauge += Time.deltaTime * 2;

        //안 찬거고
        if (currentGauge < _maxGauge)
        {
            return false;
        }
        //다 찬거고
        if (currentGauge>=1)
        {
            Debug.Log("다 찼어요");
            StopFilling();
            WaterMesh.SetActive(true);
            Watergauge.gameObject.SetActive(false);
            return true;
        }
            return true;

    }

    public void StartFilling()
    {
        Watergauge.gameObject.SetActive(true);
        isFilling = true;
        Debug.Log("물담는중");
        currentGauge = 0;
    }

    public void StopFilling()
    {
        isFilling = false;
    }

}
