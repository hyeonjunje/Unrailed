using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapEditorUI : MonoBehaviour
{
    [SerializeField] private Button _leftButton;
    [SerializeField] private Button _rightButton;

    private MapEditor _mapEditor;
    private Transform _cam;

    private void Awake()
    {
        _mapEditor = FindObjectOfType<MapEditor>();
        _cam = Camera.main.transform;
        _leftButton.onClick.AddListener(() => _cam.position += Vector3.left);
        _rightButton.onClick.AddListener(() => _cam.position += Vector3.right);
    }
}
