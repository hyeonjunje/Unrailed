using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombTest : MonoBehaviour
{
    [SerializeField] private LayerMask banglayer;
    [SerializeField] private float radius = 2f;
    [SerializeField] private ParticleSystem ExplosionEffect;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Setup();
        }
    }

    public void Setup()
    {
        StartCoroutine(ExplosionCo());
    }


    private IEnumerator ExplosionCo()
    {
        ExplosionEffect.Play();
        yield return new WaitForSeconds(3f);

        // 터질 수 있는거 감지해서 터짐
        Collider[] hitCollider = Physics.OverlapSphere(transform.position, radius, banglayer);
        for(int i = 0; i< hitCollider.Length; i++)
        {
            ReSource resource = hitCollider[i].GetComponent<ReSource>();
            AnimalHealth animal = hitCollider[i].GetComponent<AnimalHealth>();
            if (resource != null)
            {
                resource.Explosion();
            }
            else if(animal != null)
            {
                animal.Explosion();
            }
        }

        // 플레이어 터짐
        PlayerController player = FindObjectOfType<PlayerController>();
        if(player != null && Vector3.Distance(transform.position, player.transform.position) < radius)
        {
            player.Respawn();
        }

        Destroy(gameObject);
    }
}
