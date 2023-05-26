using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * 사용 머테리얼
 * Shader Graphs_Pipe_Fill_R_Test_Section 
 */
#region Enum
public enum Line_Type
{
    Start,
    Nomal,
    End
}
public enum State
{
    Waiting,
    Charging,
    Almost,
    Finish
}
public enum Line_Loop
{
    Loop,
    none_Loop
}
#endregion
public class Pipe_Flow : MonoBehaviour
{
    [Header("연결된 오브젝트")]
    public Pipe_Flow Previous_ob;
    public Pipe_Flow Next_ob_info;

    [Header("Value Fill")]
    [Range(-0.1f, 1.3f)]
    public float Fill_Value;
    public string m_value_name = "_flow";   
    
    
    Renderer render;
    [Header("Type")]
    public Line_Type type;

    [Header("End PipeLine에만 적용")]
    public Line_Loop loop_type;
    //현재 Line의 상태(변경 X)
    [Header("State")]
    [SerializeField]
    State line_state;
    //범위값
    float _minValue=-0.2f;
    float _minValue_check=0.4f;
    float _maxValue_check=0.7f;
    float _maxValue=1.3f;

    [Header("Flow Time")]
    public float check_time = 1f;

    [Range(0f, 1f)]
    float speed = 0.5f;


    [Header("Reverse Matrial")]
    public bool Reverse = false;
    [Header("시작")]
    public bool isstart = false;

    // Start is called before the first frame update
    void Start()
    {
        render = transform.GetComponent<Renderer>();
        if(!Reverse)
        {
            render.material.SetFloat(m_value_name, _minValue);
            Fill_Value = _minValue;
        }
        else
        {
            render.material.SetFloat(m_value_name, _maxValue);
            Fill_Value = _maxValue;

        }

    }
    //유체 흐름
    void Flow()
    {
        if (isstart)
        {
            if (!Reverse)
            {

                Fill_Value += Time.deltaTime * speed;

                if (Fill_Value > _maxValue_check)
                {
                    line_state = State.Almost;
                    if (Fill_Value > _maxValue)
                    {
                        line_state = State.Finish;
                        Fill_Value = _maxValue;
                        
                        Debug.LogFormat("{0} Finish", transform.name);

                    }
                }
                else
                {
                    line_state = State.Charging;
                }
                render.material.SetFloat(m_value_name, Fill_Value);
            }
            else
            {
                Fill_Value -= Time.deltaTime * speed;

                if (Fill_Value < _minValue_check)
                {
                    line_state = State.Almost;
                    if (Fill_Value < _minValue)
                    {
                        line_state = State.Finish;
                        Fill_Value = _minValue;
                        Debug.LogFormat("{0} Finish", transform.name);

                    }
                }
                else
                {
                    line_state = State.Charging;
                }
                render.material.SetFloat(m_value_name, Fill_Value);
            }
        }


    }
    IEnumerator Start_Flow_Action()
    {
        
        Flow();
        if (line_state.Equals(State.Almost))
        {
            yield return null;
            Next_ob_info.isstart = true;
        }
        yield return new WaitForSeconds(Random.Range(0f, 0.1f));
    }
    IEnumerator Nomal_Flow_Action()
    {
        if(Previous_ob.line_state.Equals(State.Almost)|| Previous_ob.line_state.Equals(State.Finish)|| isstart)
        {
            Flow();
        }
        if(line_state.Equals(State.Almost))
        {
            yield return null;
            Next_ob_info.isstart = true;

        }
        yield return new WaitForSeconds(Random.Range(0f, 0.1f));
    }
    IEnumerator End_Flow_Action()
    {
        if(loop_type.Equals(Line_Loop.none_Loop))
        {
            Flow();
        }
        else//loop 일때
        {
            if (Previous_ob.line_state.Equals(State.Almost) || Previous_ob.line_state.Equals(State.Finish)||isstart)
            {
                Flow();
            }
            if (line_state.Equals(State.Almost))
            {
                yield return null;
                Next_ob_info.isstart = true;

            }
        }
        yield return new WaitForSeconds(Random.Range(0f, 0.1f));
    }
    //초기화
    IEnumerator Init()
    {
        yield return new WaitForSeconds(1f);
        if (line_state.Equals(State.Finish))
        {
            isstart = false;
            if (!Reverse)
            {
                render.material.SetFloat(m_value_name, _minValue);
                Fill_Value = _minValue;
                line_state = State.Waiting;
            }
            else
            {
                render.material.SetFloat(m_value_name, _maxValue);
                Fill_Value = _maxValue;
                line_state = State.Waiting;
            }
            Debug.LogFormat("{0} init Finish", transform.name);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!line_state.Equals(State.Finish))
        {
            switch (type)
            {
                case Line_Type.Start:
                    StartCoroutine(Start_Flow_Action());
                    StartCoroutine(Init());
                    break;
                case Line_Type.Nomal:
                    StartCoroutine(Nomal_Flow_Action());
                    StartCoroutine(Init());
                    break;
                case Line_Type.End:
                    StartCoroutine(End_Flow_Action());
                    StartCoroutine(Init());
                    break;
            }
        }


    }
}
