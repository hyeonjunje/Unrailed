using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    // 게임의 맵을 관리하는 스크립트
    [SerializeField] private MapCreator _mapCreator;
    [SerializeField] private MapSlicer _mapSlicer;

    public List<List<BlockMK2>>[] entireMap { get; private set; } = new List<List<BlockMK2>>[2];
    public List<BlockBundle>[] entireMapBlockBundle { get; private set; } = new List<BlockBundle>[2];
    
    // 인게임 시작하면 실행할 메소드
    public async UniTask LoadMap()
    {
        MapData[] mapData = new MapData[2];

        mapData[0] = FileManager.MapsData.mapsData[5];
        mapData[1] = FileManager.MapsData.mapsData[7];

        // 맵 생성
        for(int i = 0; i < mapData.Length; i++)
        {
            // 위치, 부모 설정
            float width = mapData[i].mapData[0].arr.Length;
            Vector3 parentPosition = Vector3.right * width * i;
            Transform currentParent = new GameObject("World " + i).transform;
            currentParent.position = parentPosition;

            entireMap[i] = await _mapCreator.CreateMapAsync(mapData[i], currentParent);
        }
        
        // 맵 자르기
        for(int i = 0; i < mapData.Length; i++)
        {
            entireMapBlockBundle[i] = await _mapSlicer.SliceMap(entireMap[i]);
        }

        // 프레임 기다리기
        await UniTask.WaitForEndOfFrame(this);
    }

    // index번째 맵 재위치
    public async UniTask RePositionAsync(int index)
    {
        await _mapCreator.RePositionAsync(entireMapBlockBundle[index]);
    }
}
