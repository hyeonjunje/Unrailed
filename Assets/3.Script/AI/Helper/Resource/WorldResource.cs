using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldResource : MonoBehaviour
{
    public enum EType
    {
        Wood,
        Water,
        Stone,
        Resource
    }
    private EType _Type;
    public EType Type => _Type;

    [SerializeField] private Transform _item;
    [SerializeField] private float _resourceHp = 2.7f;
    private float _resourceScale = Mathf.Clamp01(1);

    void Start()
    {
        ResourceTracker.Instance.RegisterResource(this);
    }

    //자원 캐기

    public bool isDig()
    {
        if (_resourceHp > 0)
            return true;
        else
        {
            SpawnItem();
            ResourceTracker.Instance.DeRegisterResource(this);
            return false;
        }
          
    }

    public IEnumerator isDigCo()
    {
        WaitForSeconds wait = new WaitForSeconds(0.7f);

            yield return wait;
        while(_resourceHp > 0)
        {
            _resourceScale -= 0.25f;
            _resourceHp--;
            transform.localScale = new Vector3(_resourceScale, _resourceScale, _resourceScale);
            yield return wait;
        }
            yield return wait;
    }

    private void SpawnItem()
    {
        Transform NewItem = Instantiate(_item, new Vector3(0, 0, 0), Quaternion.identity);
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
