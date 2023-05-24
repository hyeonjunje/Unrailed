using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    [SerializeField] float Delay = 1f;
    [SerializeField] GameObject water;
    private float CurrentTime = 0f;
    private bool isDraw = false;

    public void OnDraw(Vector3 hitposition)
    {
        if (!isDraw)
        {
            StartCoroutine(OnDraw_co());
            isDraw = true;
        }

    }
    private IEnumerator OnDraw_co()
    {
        CurrentTime = 0;
        while (true)
        {
            CurrentTime += Time.deltaTime;
            if (CurrentTime >= Delay)
            {
                isDraw = false;
                break;
            }
            yield return null;
        }
    }
}
