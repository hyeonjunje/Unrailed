using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldResource : MonoBehaviour
{
    public enum EType
    {
        Wood,
        Water,
        Stone
    }
    [SerializeField] EType _Type;
    [SerializeField] int MinAmount = 15;
    [SerializeField] int MaxAmount = 50;
    [SerializeField] Transform _item;

    private float _resourceScale = Mathf.Clamp01(1);
    [SerializeField] float _resourceHp = 2.7f;

    public EType Type => _Type;
    public int AvailableAmount { get; private set; } = 0;

    void Start()
    {
        ResourceTracker.Instance.RegisterResource(this);
    }




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
        WaitForSeconds wait = new WaitForSeconds(0.8f);
            yield return wait;
        while(_resourceHp > 0)
        {
            _resourceScale -= 0.3f;
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

            // ÀÚ½Ä °´Ã¼ÀÇ ÀÚ½Ä °´Ã¼µéµµ Àç±ÍÀûÀ¸·Î Å½»ö
            DisableAllChildColliders(child);
        }
    }

}
