using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapEditorUI : MonoBehaviour
{
    [Header("CameraMoveUI")]
    [SerializeField] private Button _leftButton;
    [SerializeField] private Button _rightButton;

    [Header("EditToolUI")]
    [SerializeField] private Button _drawButton;
    [SerializeField] private Button _rowDrawButton;
    [SerializeField] private Button _eraseButton;
    
    private MapEditor _mapEditor;
    private Transform _cam;

    private void Awake()
    {
        _mapEditor = FindObjectOfType<MapEditor>();
        _cam = Camera.main.transform;
        _leftButton.onClick.AddListener(() => _cam.position += Vector3.left);
        _rightButton.onClick.AddListener(() => _cam.position += Vector3.right);

        // EditTool 버튼에 onClick event 할당
        AssignEditToolButtonEvent();
        // 제일 처음은 그리기 모드로 전환
        _drawButton.GetComponent<Image>().color = Color.yellow;
    }

    private void AssignEditToolButtonEvent()
    {
        // edit tool UI
        _drawButton.onClick.AddListener(() => _mapEditor.ChangeEditMode(EMapEditState.Draw));
        _rowDrawButton.onClick.AddListener(() => _mapEditor.ChangeEditMode(EMapEditState.DrawInLine));
        _eraseButton.onClick.AddListener(() => _mapEditor.ChangeEditMode(EMapEditState.Erase));

        _drawButton.onClick.AddListener(() => InitAllButtons());
        _rowDrawButton.onClick.AddListener(() => InitAllButtons());
        _eraseButton.onClick.AddListener(() => InitAllButtons());

        _drawButton.onClick.AddListener(() => _drawButton.GetComponent<Image>().color = Color.yellow);
        _rowDrawButton.onClick.AddListener(() => _rowDrawButton.GetComponent<Image>().color = Color.yellow);
        _eraseButton.onClick.AddListener(() => _eraseButton.GetComponent<Image>().color = Color.yellow);
    }

    private void InitAllButtons()
    {
        _drawButton.GetComponent<Image>().color = Color.white;
        _rowDrawButton.GetComponent<Image>().color = Color.white;
        _eraseButton.GetComponent<Image>().color = Color.white;
    }
}
