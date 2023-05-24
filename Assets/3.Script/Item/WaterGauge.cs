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
    public float maxGauge = 100f;

    private RectTransform UItransform;
    private float currentGauge = 0f;
    private bool isFilling = false;

    private void Awake()
    {
        Setup(Target);
    }

    void Update()
    {
        if (isFilling)
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
    }
        

    private void OnCollisionExit(Collision coll)
    {
        if(!coll.collider.CompareTag("Water"))
        {
            StopFilling();
            watergauge.gameObject.SetActive(false);
        }
    }

    private void FillGauge()
    {
         RaycastHit hit;
         Vector3 raycastDirection = (player.forward.normalized);
         Debug.DrawRay(player.position, raycastDirection * raycastDistance, Color.red);
         bool hitWater = Physics.Raycast(player.position, raycastDirection, out hit, raycastDistance, WaterLayer);

         if( hitWater)
         {
             if (hit.collider.CompareTag("Water"))
             {

                 float targetGauge = Mathf.Clamp(currentGauge - Time.deltaTime * 1f, 0f, maxGauge);
                 currentGauge = Mathf.Lerp(currentGauge, targetGauge, 5f);
                 watergauge.value = maxGauge - currentGauge;

                 if( currentGauge >= maxGauge)
                 {
                     isFilling = false;
                 }
             }
         }
    }

    public void StartFilling()
    {
        isFilling = true;
        Debug.Log("물담는중");
        currentGauge = maxGauge;
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
    