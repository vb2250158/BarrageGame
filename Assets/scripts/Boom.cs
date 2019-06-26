using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boom : MonoBehaviour
{
    /// <summary>
    /// 刚体
    /// </summary>
    private Rigidbody2D rigid2d;
    /// <summary>
    /// 加速度
    /// </summary>
    private Vector2 addSpeed;
    /// <summary>
    /// 剩余生存时间
    /// </summary>
    private float liveTime;
    /// <summary>
    /// 已经存活的计时
    /// </summary>
    private float timed;
    /// <summary>
    /// 变化速度
    /// </summary>
    private DeltaSpeed[] deltaSpeeds;


    private Aimshoot[] aimshoots;


    private BarrageLauncher danmuShoots;

    private Vector2 speed;
    /// <summary>
    /// 使用前初始化
    /// </summary>
    /// <param name="danmuGo"></param>
    /// <returns></returns>
    public Boom init(Vector2 danmuGo)
    {
        danmuShoots = gameObject.AddComponent<BarrageLauncher>();
        rigid2d = GetComponent<Rigidbody2D>();
        speed=danmuGo;
        rigid2d.velocity = danmuGo;
        return this;
    }

    /// <summary>
    /// 设置诱导
    /// </summary>
    /// <param name="_aimshoots"></param>
    /// <returns></returns>
    public Boom setAimshoot(List<Aimshoot> _aimshoots)
    {
        this.aimshoots = new Aimshoot[_aimshoots.Count];

        for (int i = 0; i < _aimshoots.Count; i++)
        {
            aimshoots[i] = new Aimshoot(_aimshoots[i].startTime, _aimshoots[i].type);
        }
        return this;
    }


    public Boom addDanmuShoot(List<DanmuShoot> _danmuShoots)
    {
        this.danmuShoots.danmus = _danmuShoots;
        return this;
    }

    /// <summary>
    /// 设置变化速度
    /// </summary>
    /// <param name="_deltaSpeeds"></param>
    /// <returns></returns>
    public Boom setDeltaSpeed(List<DeltaSpeed> _deltaSpeeds)
    {
        this.deltaSpeeds = new DeltaSpeed[_deltaSpeeds.Count];
        for (int i = 0; i < _deltaSpeeds.Count; i++)
        {
            deltaSpeeds[i] = new DeltaSpeed(_deltaSpeeds[i].startDeltaTime, _deltaSpeeds[i].speed, _deltaSpeeds[i].addSpeed);
        }
        return this;
    }
    /// <summary>
    /// 设置弹幕存活时间
    /// </summary>
    /// <param name="_liveTime"></param>
    /// <returns></returns>
    public Boom setLiveTime(float _liveTime)
    {

        liveTime = _liveTime;
        return this;
    }
    private void Update()
    {
        liveTime -= Time.deltaTime;
        timed += Time.deltaTime;
    }
    private void FixedUpdate()
    {

        if (liveTime < 0)
        {
            Destroy(this.gameObject);
        }
        //加速度设置
        rigid2d.velocity += addSpeed;

        //遍历变化时间，达到条件进行速度变化设置
        foreach (DeltaSpeed deltaSpeed in deltaSpeeds)
        {
            if (timed > deltaSpeed.startDeltaTime && deltaSpeed.On)
            {
                deltaSpeed.On = false;
                this.addSpeed = deltaSpeed.addSpeed;
                if (deltaSpeed.speed != Vector2.zero)
                {


                    rigid2d.velocity = deltaSpeed.speed;

                }

            }
        }

        foreach (Aimshoot aimshoot in aimshoots)
        {
            if (timed > aimshoot.startTime && aimshoot.On)
            {
                aimshoot.On = false;

             //   Debug.Log("s");
                Transform target;
                if (aimshoot.type == AimshootType.Player && GameObject.FindGameObjectWithTag("Player") != null)
                {
                    target = GameObject.FindGameObjectWithTag("Player").transform;
                    rigid2d.velocity = (Vector2)(target.position - transform.position).normalized * rigid2d.velocity.magnitude;

                }
                else
                {
                    rigid2d.velocity = (Vector2)(transform.parent.position - transform.position).normalized * rigid2d.velocity.magnitude;
                }
            }




        }


    }





}
