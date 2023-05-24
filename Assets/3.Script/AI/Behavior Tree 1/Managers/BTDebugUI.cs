using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BTDebugUI : MonoBehaviour
{
    [SerializeField] private BehaviorTree _tree;
    [SerializeField] private Text text;

    private void Start()
    {
        text.text = "";
    }

    private void Update()
    {
        text.text = _tree.GetDebugText();
    }

}
