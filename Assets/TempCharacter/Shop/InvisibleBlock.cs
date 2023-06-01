using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleBlock : MonoBehaviour
{
    // 블럭에 다 붙일거야

    public bool isEmpty = false; // 빈곳이면 isEmpty true로 해줘!

    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    /*public void Show()
    {
        if (isEmpty)
            return;

        _meshRenderer.enabled = true;

        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetChild(0).gameObject.SetActive(true);
        }
    }

    public void UnShow()
    {
        if (isEmpty)
            return;

        _meshRenderer.enabled = false;

        // 블럭의 자식들을 다 안보이게 한다. 안보이게 하는 방법은 자식들의 첫번째 자식인 MeshObject를 setActive.false 해준다.
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
        }
    }*/
}
