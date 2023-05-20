using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReSource : MonoBehaviour, IDig
{
    [SerializeField] int ResourceHp = 3;
    [SerializeField] float ResourceScale = 1f;
    [SerializeField] bool isDig = false;
    [SerializeField] float Delay = 1f;
    [SerializeField] float CurrentTime = 0f;
    [SerializeField] Transform ItemPrefab;

    public void OnDig(Vector3 hitposition)
    {
        if (isDig)
        {
            StartCoroutine(OnDig_co());
            isDig = false;
            if (ResourceHp > 0)
            {
                ResourceHp--;
                ResourceScale -= 0.1f;

                transform.localScale = new Vector3(ResourceScale, ResourceScale, ResourceScale);
            }
            if (ResourceHp == 0)
            {
                SpawnItem();
                Destroy(gameObject);
            }
        }
    }

    private void SpawnItem()
    {
        // 1. 아이템 생성해서 변수에 저장한다
        Transform NewItem = Instantiate(ItemPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        NewItem.SetParent(transform.parent);
        NewItem.localPosition = (new Vector3(0, 0.5f, 0));
        NewItem.localRotation = Quaternion.identity;




    }

    private IEnumerator OnDig_co()
    {
        CurrentTime = 0;
        while (true)
        {
            CurrentTime += Time.deltaTime;
            if (CurrentTime >= Delay)
            {
                isDig = true;
                break;
            }
            yield return null;
        }

    }
}