using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartObject_Tree : SmartObject
{
    private float _resourceScale = 1f;
    private int _resourceHp = 3;
    public Transform ItemPrefab;

    public void Cutting()
    {
        StartCoroutine(TreeCo());
    }

    IEnumerator TreeCo()
    {
        while (_resourceHp > 0)
        {
            ToggleState();
            yield return new WaitForSeconds(0.5f);
        }
        Destroy(gameObject, 0.1f);
        SpawnItem();
    }



    public void ToggleState()
    {
        if (_resourceHp <= 0)
        {
            Destroy(gameObject, 0.1f);
            SpawnItem();
        }

        _resourceScale -= 0.25f;
        _resourceHp--;
        transform.localScale = new Vector3(_resourceScale, _resourceScale, _resourceScale);
    }


    private void SpawnItem()
    {
        Transform NewItem = Instantiate(ItemPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        NewItem.SetParent(transform.parent);
        NewItem.localPosition = (new Vector3(0, 0.5f, 0));
        NewItem.localRotation = Quaternion.identity;

        DisableAllChildColliders(NewItem);
    }

    private void DisableAllChildColliders(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Collider childCollider = child.GetComponent<Collider>();
            if (childCollider != null)
            {
                childCollider.enabled = false;
            }

            // 자식 객체의 자식 객체들도 재귀적으로 탐색
            DisableAllChildColliders(child);
        }
    }


}
