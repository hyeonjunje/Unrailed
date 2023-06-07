using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BTDebugUI : MonoBehaviour
{
    private BehaviorTree _tree;
    private Helper _helper;
    [SerializeField] private Text text;

    private void Start()
    {
        text.text = "";
    }


    private void Update()
    {
        if(_tree==null)
        {
            _helper = FindObjectOfType<Helper>();
            if(_helper!=null)
            {
                _tree = _helper.GetComponent<BehaviorTree>();

            }
        }
        else
        {
            text.text = _tree.GetDebugText();

        }
    }

}
