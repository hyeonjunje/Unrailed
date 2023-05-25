using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInteractionTest : MonoBehaviour
{
    [SerializeField] private bool _isHighlight;

    [SerializeField] private float _brightAmount = 50f;
    private List<Material> materials = new List<Material>();

    private void Awake()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach(Renderer renderer in renderers)
        {
            materials.Add(renderer.material);
        }

        _brightAmount /= 256;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {


            // 밝은 상태면 원래대로
            if(_isHighlight)
            {
                foreach (Material material in materials)
                    material.color -= new Color(_brightAmount, _brightAmount, _brightAmount, 0);
            }
            else
            {
                foreach (Material material in materials)
                    material.color += new Color(_brightAmount, _brightAmount, _brightAmount, 0);
            }

            _isHighlight = !_isHighlight;
        }
    }
}
