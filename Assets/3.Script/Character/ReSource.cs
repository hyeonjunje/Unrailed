using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ReSource : MonoBehaviour, IDig
{

    [SerializeField] float ResourceHp = 2.7f;
    [SerializeField] float Delay = 0.5f;
    [SerializeField] Transform ItemPrefab;
    private float ResourceScale = 1f;
    private bool isPlayerLooking = false;
    private bool isDig = false;// ó���� false���� ���� : Player�� ������ �ٲ���� �Ǵ� �ɱ�?
    //Coroutine co;
    //public bool IsDig
    //{
    //    get { return isDig; }
    //    set { isDig=value; }
    //}

  
    public void OnDig(Vector3 hitposition)
    {
        if (!isDig)
        {
            StartCoroutine(OnDig_co());
        }
    }
    /*public void OffDig()
    {
        StopAllCoroutines();
    }*/
   
    private void DisableAllChildColliders(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Collider childCollider = child.GetComponent<Collider>();
            if (childCollider != null)
            {
                childCollider.enabled = false;
            }

            // �ڽ� ��ü�� �ڽ� ��ü�鵵 ��������� Ž��
            DisableAllChildColliders(child);
        }
    }

    private void SpawnItem()
    {
        // 1. ������ �����ؼ� ������ �����Ѵ�
        Transform NewItem = Instantiate(ItemPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        NewItem.SetParent(transform.parent);
        NewItem.localPosition = (new Vector3(0, 0.5f, 0));
        NewItem.localRotation = Quaternion.identity;

        DisableAllChildColliders(NewItem);
    }

    private IEnumerator OnDig_co()
    {
        if (!isDig)
        {
            isDig = true;
            yield return new WaitForSeconds(Delay);

            if (ResourceHp > 0)
            {
                ResourceHp--;
                ResourceScale -= 0.3f;
                transform.localScale = new Vector3(ResourceScale, ResourceScale, ResourceScale);
            }
            if (ResourceHp < 0.1f)
            {
                SpawnItem();
                Destroy(gameObject, 0f);
                ResourceScale = 1;
            }
            isDig = false;
            //yield return null;

        }

    }

}