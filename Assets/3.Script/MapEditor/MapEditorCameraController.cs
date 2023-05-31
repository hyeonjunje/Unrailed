using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditorCameraController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _moveFasterSpeed;
    [SerializeField] private float _zoomSpeed;
    [SerializeField] private float _zoomFasterSpeed;

    [SerializeField] private float _maxZoom, _minZoom;

    private float _currentMoveSpeed;
    private float _currentZoomSpeed;

    private float _originOrthographicSize;
    private Vector3 _originPos = Vector3.zero;
    private Quaternion _originQut = Quaternion.identity;

    private Camera _cam;
    private MapEditorMK2 _mapEditor;

    private void Awake()
    {
        _mapEditor = FindObjectOfType<MapEditorMK2>();
        _cam = GetComponent<Camera>();

        _originOrthographicSize = _cam.orthographicSize;
        _originPos = transform.position;
        _originQut = transform.rotation;

        _currentMoveSpeed = _moveSpeed;
        _currentZoomSpeed = _zoomSpeed;
    }

    private void Update()
    {
        if(_mapEditor.isInteract)
        {
            InputKey();

            Move();
            Zoom();
        }
    }

    private void InputKey()
    {
        // 초기화
        if (Input.GetKeyDown(KeyCode.Escape))
            InitPos();

        // 속도
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            _currentMoveSpeed = _moveFasterSpeed;
            _currentZoomSpeed = _zoomFasterSpeed;
        }
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            _currentMoveSpeed = _moveSpeed;
            _currentZoomSpeed = _zoomSpeed;
        }
    }

    private void Zoom()
    {
        float scroll = -Input.GetAxisRaw("Mouse ScrollWheel");
        _cam.orthographicSize += scroll * _currentZoomSpeed;

        _cam.orthographicSize = Mathf.Clamp(_cam.orthographicSize, _minZoom, _maxZoom);
    }

    // 움직임
    private void Move()
    {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        transform.position += moveDir * _currentMoveSpeed * Time.deltaTime;
    }

    // 초기화
    private void InitPos()
    {
        _cam.orthographicSize = _originOrthographicSize;

        transform.position = _originPos;
        transform.rotation = _originQut;
    }
}
