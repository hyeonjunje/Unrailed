using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReSource : MonoBehaviour, IDig
{
    [SerializeField] int ResourceHp = 3;
    [SerializeField] float Delay = 1f;
    [SerializeField] Transform ItemPrefab;
    private float ResourceScale = 1f;
    private float CurrentTime = 0f;
    private bool isDig = false;// 처음에 false였던 이유 : Player가 참으로 바꿔줘야 되는 걸까?

    public void OnDig(Vector3 hitposition)
    {
        if (!isDig)
        {
            StartCoroutine(OnDig_co());
            isDig = true;
            if (ResourceHp > 0)
            {
                ResourceHp--;
                ResourceScale -= 0.25f;
                transform.localScale = new Vector3(ResourceScale, ResourceScale, ResourceScale);
            }
            if (ResourceHp == 0)
            {
                SpawnItem();
                Destroy(gameObject,0.5f);
              //  gameObject.SetActive(false);
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
                isDig = false;
                break;
            }
            yield return null;
        }

    }
   
}