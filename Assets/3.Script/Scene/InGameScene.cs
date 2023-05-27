using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameScene : MonoBehaviour
{
    [SerializeField] private GameObject _loadingSceneUI;
    [SerializeField] private MapCreator _mapCreator;

    private void Awake()
    {
        FileManager.LoadGame();

        // 로딩 씬 보여주기
        _loadingSceneUI.SetActive(true);

        // 보여줄동안 
        // 플레이어 생성
        // 맵 생성
        // 동물, 도적, ai 생성
        // 긴 맵을 만들고 자르기??
        // astar 맵 최신화
        StartCoroutine(LodingCo());
        Debug.Log(Time.realtimeSinceStartup);

        // 이 작업 다 끝나고 로딩 씬 풀고
        // 자른 맵 내려오면서 맵 조립
    }

/*    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
            StartCoroutine(LodingCo());
    }*/

    private IEnumerator LodingCo()
    {
        // 맵 생성
        // 플레이어 생성
        // 동물, 도적, ai 생성
        yield return StartCoroutine(_mapCreator.CreateMapCo(0));
        Debug.Log(Time.realtimeSinceStartup);

        _loadingSceneUI.SetActive(false);

    }
}
