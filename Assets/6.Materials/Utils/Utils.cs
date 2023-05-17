using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    /// <summary>
    /// 모든 자기의 자식들을 Destroy해주는 함수
    /// </summary>
    /// <param name="parent">자식을 다 없애줄 부모 오브젝트</param>
    public static void DestroyAllChild(this Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (!child.Equals(parent))
                GameObject.Destroy(child.gameObject);
        }
    }
}
