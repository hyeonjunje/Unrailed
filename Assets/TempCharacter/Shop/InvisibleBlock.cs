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

    public void Show()
    {
        if (isEmpty)
            return;

        _meshRenderer.enabled = true;
    }

    public void UnShow()
    {
        if (isEmpty)
            return;

        _meshRenderer.enabled = false;
    }
}
