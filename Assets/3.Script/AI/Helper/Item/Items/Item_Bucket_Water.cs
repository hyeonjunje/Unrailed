using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_Bucket_Water : SimpleInteraction
{
    public Slider Watergauge;
    public GameObject WaterMesh;

    protected Item_Bucket _bucket;
    protected bool _full = false;

    private void Awake()
    {
        _bucket = GetComponent<Item_Bucket>();
        WaterMesh.SetActive(false);
    }

    public override bool Perform()
    {
        Debug.Log("�� ����");
        FillGauge();
        return !Stop();
    }

    public override bool CanPerform()
    {
        Debug.Log(_bucket.IsOn);
        //�絿�̸� �� ���¶�� ����
        return base.CanPerform() && _bucket.IsOn && !_bucket.Full;
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
            Watergauge.value = Mathf.MoveTowards(Watergauge.value, 1f, Time.deltaTime);
            Watergauge.gameObject.SetActive(false);
        }

    }
    private void OnDisable()
    {
        Watergauge.value = 0;
    }

    public bool Stop()
    {
        if(Watergauge.value>=1)
        {
            _bucket.BucketisFull();
            Debug.Log("�� �ٶ����");
            return true;
        }
        return false;
    }

}
