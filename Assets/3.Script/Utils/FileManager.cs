using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FileManager
{
    private static MapsData _mapsData = new MapsData();
    public static MapsData MapsData => _mapsData;

    // 저장할 때, 삭제할 때
    public static void SaveGame()
    {
        string filePath = Application.persistentDataPath + "/mapSave.json";
        StreamWriter saveFile = new StreamWriter(filePath);
        saveFile.Write(JsonUtility.ToJson(_mapsData, true));

        saveFile.Close();
    }

    // 게임 시작할 때, 불러오기 버튼 누를 때
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
        JsonUtility.FromJsonOverwrite(saveFile.ReadToEnd(), _mapsData);

        saveFile.Close();
    }
}
