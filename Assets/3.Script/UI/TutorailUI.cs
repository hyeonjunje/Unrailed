using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorailUI : MonoBehaviour
{

    public Sprite[] inPutUI;
    public Sprite[] outPutUI;

    public Image playerInputDir;
    public Image playerInputSelect;

    [SerializeField] Player player;

    [SerializeField] float time;
    [SerializeField] int dirNum;
    // Start is called before the first frame update
    void Awake()
    {
        player = FindObjectOfType<Player>();
        dirNum = 0;
    }

    // Update is called once per frame
    void Update()
    {
        InputTuto();
        InputAnimation(dirNum);
    }

    public void InputTuto()
    {
        time += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.W)) dirNum = 1;
        if (Input.GetKeyDown(KeyCode.A)) dirNum = 2;
        if (Input.GetKeyDown(KeyCode.S)) dirNum = 3;
        if (Input.GetKeyDown(KeyCode.D)) dirNum = 0;

        if (Input.GetKeyDown(KeyCode.Space)) gameObject.SetActive(false);


        /*
         나중에 player void를 public으로 변경해서 코드 고칠 것 05.29 goto

            void GetInput()

            xAxis = Input.GetAxisRaw("Horizontal");
            zAxis = Input.GetAxisRaw("Vertical");
            dashKeyDown = Input.GetButtonDown("Dash");
            getItemKeyDown = Input.GetButtonDown("getItem");

         -> if(player.xAxis > 0) 
         -> if(player.xAxis < 0) 
         -> if(player.YAxis > 0) 
         -> if(player.YAxis < 0) 

            if(player.getItemKeyDown) 
         */
    }
    void InputAnimation(int dirNum)
    {
        if (time < 0.5f)
        {
            playerInputDir.sprite = outPutUI[dirNum];
        }

        else if (time < 1f)
        {
            playerInputDir.sprite = inPutUI[dirNum];
        }

        else
        {
            time = 0;
        }
    }
}
