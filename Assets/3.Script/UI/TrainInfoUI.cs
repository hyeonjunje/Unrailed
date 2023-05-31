using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class TrainInfoUI : MonoBehaviour
{
    [SerializeField] private Text speedText;
    [SerializeField] private Text railText;
    [SerializeField] private Text boltText;

    [SerializeField] private GoalManager railCount;
    [SerializeField] private TrainEngine trainEngine;

    private int num;
    private float time;
    // Start is called before the first frame update
    private void Awake()
    {
        railCount = FindObjectOfType<GoalManager>();
        trainEngine = FindObjectOfType<TrainEngine>();
    }
    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        int i = Random.Range(0, 10);
        if(time > i)
        {
            num = Random.Range(1, 6);
            time = 0;
        }
        railText.text = $"{railCount.railCon.Count - trainEngine.rails.Count}m";

        if(trainEngine._trainMoveSpeed <= 0) speedText.text = $".{trainEngine._trainMoveSpeed}m/s";

        else
        {
            speedText.text = $".{trainEngine._trainMoveSpeed * 200 + num}m/s";
        }
    }
}
