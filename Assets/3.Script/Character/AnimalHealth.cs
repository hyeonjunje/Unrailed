using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalHealth : MonoBehaviour
{
    [SerializeField] private int _animalHp = 8;

    private int _currentHp;
    public int CurrentHp
    {
        get { return _currentHp; }
        set
        {
            _currentHp = value;

            if(_currentHp == 0)
            {
                Death();
            }
        }
    }

    private void Awake()
    {
        CurrentHp = _animalHp;
    }

    public void Hit()
    {
        CurrentHp--;
    }

    public void Explosion()
    {
        while (CurrentHp != 0)
            Hit();
    }

    private void Death()
    {
        Destroy(gameObject);
    }
}
