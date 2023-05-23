using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterGauge : MonoBehaviour
{
    public Slider watergauge;
    public Transform player;
    public LayerMask waterLayer;
    public GameObject Target;
    public float raycastDistance = 1f;
    public Vector3 distance = Vector3.up * 0.4f;

    private RectTransform UItransform;
    private float currentGauge = 0f;
    private float maxGauge = 100f;
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

    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            StartFilling(); 
            watergauge.gameObject.SetActive(true);  
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Water"))
        {
            StopFilling();
            watergauge.gameObject.SetActive(false);
        }
    }

    private void FillGauge()
    {
        RaycastHit hit;
        Debug.DrawRay(player.position, player.forward * raycastDistance , Color.red);
            if (Physics.Raycast(player.position, player.forward, out hit, raycastDistance, waterLayer))
        {
            if (hit.collider.CompareTag("Water"))
            {
                float targetGauge = Mathf.Clamp(currentGauge + Time.deltaTime * 1f, 3f, maxGauge);
                currentGauge = Mathf.Lerp(currentGauge, targetGauge, 5f);
                watergauge.value = currentGauge;

                if (currentGauge >= maxGauge)
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
    }
        
    public void StopFilling()
    {
        isFilling = false;
    }
    
   public void Setup(GameObject target)
    {
        this.Target = target;
        UItransform = GetComponent<RectTransform>();
    }
}
    