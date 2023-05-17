using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    /// <summary>
    /// ��� �ڱ��� �ڽĵ��� Destroy���ִ� �Լ�
    /// </summary>
    /// <param name="parent">�ڽ��� �� ������ �θ� ������Ʈ</param>
    public static void DestroyAllChild(this Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (!child.Equals(parent))
                GameObject.Destroy(child.gameObject);
        }
    }
}
