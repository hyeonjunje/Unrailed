using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalHealth : MonoBehaviour
{
    public bool isHit = false;
    [SerializeField] public int AnimalHp = 0;
    public float Delay = 0.5f;
    public void OnHit()
    {
        if (!isHit)
        {
            StartCoroutine(Hit_co());
        }
    }
    public IEnumerator Hit_co()
    {
        isHit = true;

        yield return new WaitForSeconds(Delay);

        if(AnimalHp > 0 )
        {
            AnimalHp--;
        }
        if(AnimalHp <= 0)
        {
            Destroy(gameObject);
            Debug.Log("파티클 만들어야해");
        }
        isHit = false;

    }
}
