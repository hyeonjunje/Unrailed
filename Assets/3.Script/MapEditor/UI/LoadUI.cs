using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadUI : MonoBehaviour
{
    [SerializeField] private Button _loadButtonPrefab;
    [SerializeField] private RectTransform _content;

    [SerializeField] private MapEditor _mapEditor;

    // 활성화 될 때 불러오기 버튼 만들어오기
    private void OnEnable()
    {
        // 데이터를 로드해와서 보여주기
        for(int i = 0; i < FileManager.MapsData.mapsData.Count; i++)
        {
            int index = i;
            Button loadButton = Instantiate(_loadButtonPrefab, _content);
            loadButton.onClick.AddListener(() => _mapEditor.LoadMap(index));
        }


        _content.sizeDelta = new Vector3(_content.sizeDelta.x, (_content.transform.childCount / 2) * 310);
    }
}
