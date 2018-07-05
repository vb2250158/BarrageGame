using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Anibf : TimeEvent 
{
    public string evetName;
    public Anibf(float eventTime, string evetName)
    {
        this.eventTime = eventTime;
        On = true;
    }


}
/// <summary>
/// 时间事件系统
/// </summary>
[System.Serializable]
public class TimeEvent
{
    /// <summary>
    /// 事件触发时机
    /// </summary>
    public float eventTime;
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="eventTime"></param>
    public TimeEvent(float eventTime)
    {
        this.eventTime = eventTime;
        On = true;
    }                        
    public TimeEvent() { }
    public bool On { get; set; }

}



public class Anic : MonoBehaviour
{

    public Anibf[] anibfs;

    private float timed;
    private Animator ani;



    void Start()
    {
        ani = GetComponent<Animator>();
    }



    void Update()
    {
        timed += Time.deltaTime;


        foreach (var item in anibfs)
        {
            if (item.On && item.eventTime > timed)
            {
                ani.SetTrigger(item.evetName);
            }
        }


    }
}
