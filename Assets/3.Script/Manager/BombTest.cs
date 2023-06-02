using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombTest : MonoBehaviour
{
    [SerializeField] private LayerMask banglayer;
    public Rigidbody rig;
    public ReSource resource_a;
    public GameObject Ballon;
    public GameObject Dynamite;
    [SerializeField] private float radius = 2f;
    [SerializeField] private float distance = 0f;
    private Player player;
    public float speed = 0.5f;
    public bool isrespawn = false;

    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(3f);

        Debug.Log("터졌다");

        Collider[] hitCollider = Physics.OverlapSphere(transform.position, radius, banglayer);
        for(int i = 0; i< hitCollider.Length; i++)
        {
            resource_a = hitCollider[i].GetComponent<ReSource>();
            if (resource_a != null && Dynamite.activeSelf)
            {
                resource_a.SpawnItem();
            }
            Destroy(hitCollider[i].gameObject);
        }
        distance = Vector3.Distance(transform.position, player.transform.position);
        if(distance < radius) //거리가 radius 보다 작으면
        {
            Debug.Log("플레이어 사망");
            ReSpawn();
        }
        Destroy(gameObject);
    }
    
    public void ReSpawn()
    {
        player.transform.localPosition += Vector3.up * 10f ;
        isrespawn = true;
        Ballon.SetActive(true);
        
    }

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        Ballon.SetActive(false);
    }

    public void Setup()
    {
        StartCoroutine(Explosion());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Setup();
        }
       
    }

   

    
    /* private void OnTriggerEnter(Collider other)
     {
         if (Input.GetKeyDown(KeyCode.J))
         {

         }
     }*/
}
