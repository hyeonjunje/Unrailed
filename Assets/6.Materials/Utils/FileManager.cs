using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FileManager
{
    private static MapData _mapData;
    public static MapData MapData => _mapData;

    public static void SaveGame()
    {
        string filePath = Application.persistentDataPath + "/mapSave.json";
        StreamWriter saveFile = new StreamWriter(filePath);
        saveFile.Write(JsonUtility.ToJson(_mapData, true));

        saveFile.Close();
    }

    public static void LoadGame()
    {
        string filePath = Application.persistentDataPath + "/mapSave.json";
        Debug.Log(filePath);

        if(!File.Exists(filePath))
        {
            Debug.Log("No file!");
            return;
        }

        StreamReader saveFile = new StreamReader(filePath);
        JsonUtility.FromJsonOverwrite(saveFile.ReadToEnd(), _mapData);

        saveFile.Close();
    }
}
