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

    /// <summary>
    /// 정해준 index부터 모든 자식을 Destory해주는 함수
    /// </summary>
    /// <param name="parent">자식을 없애줄 부모 오브젝트</param>
    /// <param name="index">몇번째 자식부터 없애줄지 결정</param>
    public static void DestroyChildren(this Transform parent, int index)
    {
        int currentIndex = 0;

        foreach(Transform child in parent)
        {
            if(!child.Equals(parent))
            {
                if (currentIndex >= index)
                    GameObject.Destroy(child.gameObject);

                currentIndex++;
            }
        }
    }
}
