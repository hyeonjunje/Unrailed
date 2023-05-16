using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MyArr<T>
{
    public T[] arr;

    public MyArr(int size)
    {
        arr = new T[size];
    }
}
