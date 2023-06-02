using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterGauge : MonoBehaviour 
{
    public Slider watergauge;
    public Transform player;
    public LayerMask WaterLayer;
    public GameObject Target;
    public GameObject item;
    public float raycastDistance = 5f;
    public Vector3 distance = Vector3.up * 0.4f;
    public float minGauge = 100f;
    public float fillSpeed = 50f;
    public GameObject WaterMesh;

    private RectTransform UItransform;
    private float currentGauge = 0f;
    private bool isFilling = false;


    private void Awake()
    {
        Setup(Target);
        WaterMesh.SetActive(false);
    }

    void Update()
    {
        if (isFilling )
        {
            FillGauge();
        }
        if (Target != null)
        {
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(Target.transform.position + new Vector3(0, 0.1f, 0));
            UItransform.position = screenPosition + distance;
        }
        
    }



    private void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.CompareTag("Water") && item.name == "Item_Bucket(Clone)")
        {
            StartFilling();
            watergauge.gameObject.SetActive(true);

        }
        else
        {
            watergauge.gameObject.SetActive(false);
        }
    }
        

    private void OnCollisionExit(Collision coll)
    {
        if(!coll.collider.CompareTag("Water"))
        {
            StopFilling();
            watergauge.gameObject.SetActive(false);
        }
    }

    public void FillGauge()
    {
       float targetGauge = Mathf.Clamp(currentGauge + Time.deltaTime * fillSpeed, 0f, minGauge); //minGauge = 100f //value가 0이면 꽉찬상태
       currentGauge = Mathf.Lerp(currentGauge, targetGauge, 5f);
       watergauge.value = minGauge - currentGauge;
    


        if ( currentGauge >= minGauge )
        {
            isFilling = false;
        }
        if(watergauge.value == 0)
        {
            watergauge.gameObject.SetActive(false);
            WaterMesh.SetActive(true);
            
        }
        
    }

    public void StartFilling()
    {
        isFilling = true;
        Debug.Log("물담는중");
        currentGauge = minGauge;
    }
        
    public void StopFilling()
    {
        isFilling = false;
        currentGauge = 0;

       

    }
    
   public void Setup(GameObject target)
    {
        this.Target = target;
        UItransform = GetComponent<RectTransform>();
    }
    
}
    