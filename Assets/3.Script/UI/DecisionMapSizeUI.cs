using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DecisionMapSizeUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private InputField _xInputField;
    [SerializeField] private InputField _yInputField;

    private MapEditor _mapEditor;

    private void Awake()
    {
        _mapEditor = FindObjectOfType<MapEditor>();
    }

    public void MakeBaseMap()
    {
        int x = int.Parse(_xInputField.text);
        int z = int.Parse(_yInputField.text);

        if(x > 0 && z > 0)
        {
            _mapEditor.MakeBaseMap(x, z);
        }
    }
}
