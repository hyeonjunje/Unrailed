using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopUI : MonoBehaviour
{
    [Header("MapSizeArea")]
    [SerializeField] private Text xSize;
    [SerializeField] private Text ySize;
    [SerializeField] private Button xSizeUpButton;
    [SerializeField] private Button xSizeDownButton;
    [SerializeField] private Button ySizeUpButton;
    [SerializeField] private Button ySizeDownButton;

    [Header("Toggle")]
    [SerializeField] private Toggle gridOnToggle;
    [SerializeField] private Toggle counterToggle;

    [Header("GroundArea")]
    [SerializeField] private Button[] groundButtons;

    [Header("ETC")]
    [SerializeField] private MapEditorMK2 mapEditor;
    [SerializeField] private LineGrid lineGrid;

    private int _currentXSize;
    private int _currentYSize;
    public int CurrentXSize
    {
        get { return _currentXSize; }
        set
        {
            _currentXSize = value;
            xSize.text = _currentXSize.ToString();
            lineGrid.MakeGrid(-19.5f, -9.5f, _currentXSize, _currentYSize);
        }
    }
    public int CurrentYSize
    {
        get { return _currentYSize; }
        set
        {
            _currentYSize = value;
            ySize.text = _currentYSize.ToString();
            lineGrid.MakeGrid(-19.5f, -9.5f, _currentXSize, _currentYSize);
        }
    }


    private void Awake()
    {
        CurrentXSize = 40;
        CurrentYSize = 20;
        lineGrid.MakeGrid(-19.5f, -9.5f, CurrentXSize, CurrentYSize);
        SetOnClickButtonEvent();
        SetOnCheckToggleEvent();
    }

    private void SetOnClickButtonEvent()
    {
        // 사이즈 조절 버튼
        xSizeUpButton.onClick.AddListener(() => CurrentXSize++);
        xSizeDownButton.onClick.AddListener(() => CurrentXSize--);
        ySizeUpButton.onClick.AddListener(() => CurrentYSize++);
        ySizeDownButton.onClick.AddListener(() => CurrentYSize--);

        xSizeUpButton.onClick.AddListener(() => mapEditor.ResizeXMap(1));
        xSizeDownButton.onClick.AddListener(() => mapEditor.ResizeXMap(-1));
        ySizeUpButton.onClick.AddListener(() => mapEditor.ResizeYMap(1));
        ySizeDownButton.onClick.AddListener(() => mapEditor.ResizeYMap(-1));

        // ground 버튼들
        for(int i = 0; i < groundButtons.Length; i++)
        {
            int index = i;
            groundButtons[i].onClick.AddListener(() => mapEditor.materialIndex = index);
            groundButtons[i].onClick.AddListener(() => SetButtonOutline(index));
        }
    }

    private void SetOnCheckToggleEvent()
    {
        gridOnToggle.isOn = false;
        lineGrid.gameObject.SetActive(false);

        gridOnToggle.onValueChanged.AddListener((flag) => lineGrid.gameObject.SetActive(gridOnToggle.isOn));
    }

    private void SetButtonOutline(int index)
    {
        for(int i = 0; i < groundButtons.Length; i++)
        {
            groundButtons[i].GetComponent<Outline>().enabled = false;
            if (i == index)
                groundButtons[i].GetComponent<Outline>().enabled = true;
        }
    }
}
