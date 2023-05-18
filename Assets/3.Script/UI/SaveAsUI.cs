using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveAsUI : MonoBehaviour
{
    [SerializeField] private InputField inputField;
    [SerializeField] private Button saveButton;

    private MapEditorMK2 mapEditor;

    private void Awake()
    {
        mapEditor = FindObjectOfType<MapEditorMK2>();
        saveButton.onClick.AddListener(() => SaveAs());    
    }

    private void OnEnable()
    {
        mapEditor.isDraw = false;
    }

    private void OnDisable()
    {
        mapEditor.isDraw = true;
    }

    private void SaveAs()
    {
        mapEditor.SaveMap(inputField.text);
    }
}
