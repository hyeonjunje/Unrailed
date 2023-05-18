using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameScene : MonoBehaviour
{
    private void Awake()
    {
        FileManager.LoadGame();

        MapCreator.Instance.CreateRandomMap();
    }
}
