using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 弹幕发射基本参数
/// </summary>
[System.Serializable]
public class DanmuShoot
{
    /// <summary>
    /// 弹幕起始发射时间
    /// </summary>
    public float startTime;
    /// <summary>
    /// FA射周期
    /// </summary>
    public float shootTime;
    /// <summary>
    /// 粒子存在时间
    /// </summary>
    public float liveTime;
    /// <summary>
    /// 弹幕发射方向
    /// </summary>
    public Vector2 danmuGo;
    /// <summary>
    /// 每轮弹幕发射频率
    /// </summary>
    public float danmuFrequency;
    /// <summary>
    /// 弹幕发射数量设置
    /// </summary>
    public LaunchNumber launchNumber;
    /// <summary>
    /// 弹幕预制
    /// </summary>
    public DanmuPreform danmuPreform;
    /// <summary>
    /// 速度变化 
    /// </summary>
    public DeltaSpeed[] deltaSpeeds;
    /// <summary>
    /// 追击开始时间
    /// </summary>
    public Aimshoot[] aimshoot;
    /// <summary>
    /// 随机偏差值
    /// </summary>
    public RandmOffset randmOffset;

    /// <summary>
    /// 超级弹幕
    /// </summary>
    public DanmuShoot[] EXDanmu;

}
[System.Serializable]
public class DanmuPreform
{
    /// <summary>
    /// 随机的预制体
    /// </summary>
    public GameObject[] danmuObjects;
    /// <summary>
    /// 是否随机生成
    /// </summary>
    public bool isRandom;
}


[System.Serializable]
public class DeltaSpeed
{
    /// <summary>
    /// 开始变化的时间
    /// </summary>
    public float startDeltaTime;
    /// <summary>
    /// 重新设置速度
    /// </summary>
    public Vector2 speed;
    /// <summary>
    /// 设置一个加速度
    /// </summary>
    public Vector2 addSpeed;

    public DeltaSpeed(float startDeltaTime, Vector2 speed, Vector2 addSpeed)
    {
        On = true;
        this.startDeltaTime = startDeltaTime;
        this.speed = speed;
        this.addSpeed = addSpeed;
    }

    public bool On { get; set; }


}
/// <summary>
/// 弹幕发射数量
/// </summary>
[System.Serializable]
public class LaunchNumber
{
    /// <summary>
    /// 每次弹幕发射数量
    /// </summary>
    public int number;
    /// <summary>
    /// 每个弹幕距离偏差值
    /// </summary>
    public float deviation;
    /// <summary>
    /// 随着时间偏移
    /// </summary>
    public bool timedelta;
}
/// <summary>
/// 弹幕偏差
/// </summary>
[System.Serializable]
public class RandmOffset
{
    /// <summary>
    /// 起始位置偏移量
    /// </summary>
    public Vector2 startPosition;
}
[System.Serializable]
public class Aimshoot
{
    /// <summary>
    /// 起始诱导时间
    /// </summary>
    public float startTime;
    /// <summary>
    /// 诱导对象
    /// </summary>
    public AimshootType type;

    public bool On { get; set; }
    public Aimshoot(float startTime, AimshootType type)
    {
        On = true;
        this.startTime = startTime;
        this.type = type;
    }
}

public enum AimshootType
{
    Player,Slif
}

public class BarrageLauncher : MonoBehaviour
{
    public DanmuShoot[] danmus;




    public float timed { get; set; }

    void Start()
    {



        foreach (DanmuShoot danmu in danmus)
        {
            StartCoroutine(danmuInit(danmu));
        }
    }

    void Update()
    {
        timed += Time.deltaTime;
    }
    /*IEnumerator spawnStart()    //生成弹幕的方法
    {
        for (int i = 0; i < danmuNum; i++)
        {
            //生成弹幕
            xyDis.x = xDis * speed;
            xyDis.y = yDis * speed;
            clone = Instantiate(danmu, spawn.transform.position, spawn.transform.rotation);
            clone.transform.parent = spawn.transform;
            clone.GetComponent<Rigidbody>().AddForce(xyDis);
            xDis = xDis + xOffset;
            //弹幕发射时间间隔
            yield return new WaitForSeconds(danmuTime);
        }*/

    /// <summary>
    /// 创建一种弹幕
    /// </summary>
    /// <param name="_danmuShoot"></param>
    /// <returns></returns>
    IEnumerator danmuInit(DanmuShoot _danmuShoot)
    {


        //发射检测时间
        while (timed < _danmuShoot.startTime)
        {
            yield return null;
        }






        //整个弹幕发射器的生命周期
        for (float live = 0; live < _danmuShoot.shootTime; live += _danmuShoot.danmuFrequency)
        {



            ShootSystem(_danmuShoot);



            //ShootSystem(_danmuShoot);




            yield return new WaitForSeconds(_danmuShoot.danmuFrequency);

        }


    }



    /// <summary>
    /// 弹幕预制体系统(构成一轮弹幕)
    /// </summary>
    /// <param name="_danmuShoot"></param>
    /// <param name="danmuNumber">如果不是随机，就会用到该数值作为顺序发射</param>
    public void ShootSystem(DanmuShoot _danmuShoot)
    {
        for (int i = 0; i < _danmuShoot.launchNumber.number; i++)
        {
            GameObject danmuObject;
            //是否随机构成
            if (_danmuShoot.danmuPreform.isRandom)
            {
                int n = Random.Range(0, _danmuShoot.danmuPreform.danmuObjects.Length);
                danmuObject = Instantiate(_danmuShoot.danmuPreform.danmuObjects[n], transform);

            }
            else
            {
                danmuObject = Instantiate(_danmuShoot.danmuPreform.danmuObjects[i], transform);
            }

            if (_danmuShoot.launchNumber.timedelta)
            {
                danmuObject.GetComponent<Boom>()
                  .init(Vector2Rote(_danmuShoot.danmuGo, _danmuShoot.launchNumber.deviation * (i + 1)*timed))
                  .setLiveTime(_danmuShoot.liveTime)
                  .setDeltaSpeed(_danmuShoot.deltaSpeeds)
                  .setAimshoot(_danmuShoot.aimshoot)
                  .addDanmuShoot(_danmuShoot.EXDanmu)
                  .GetComponent<Transform>().localPosition = getOffset(_danmuShoot.randmOffset.startPosition);
            }
            else
            {
                danmuObject.GetComponent<Boom>()
                 .init(Vector2Rote(_danmuShoot.danmuGo, _danmuShoot.launchNumber.deviation * (i + 1)))
                 .setLiveTime(_danmuShoot.liveTime)
                 .setDeltaSpeed(_danmuShoot.deltaSpeeds)
                 .setAimshoot(_danmuShoot.aimshoot)
                 .addDanmuShoot(_danmuShoot.EXDanmu)
                 .GetComponent<Transform>().localPosition = getOffset(_danmuShoot.randmOffset.startPosition);
            }

           

        }

    }


    /// <summary>
    /// 向量大小偏移
    /// </summary>
    /// <param name="v2"></param>
    /// <param name="Offset"></param>
    /// <returns></returns>
    public Vector2 Vector2Offset(Vector2 v2, Vector2 Offset)
    {

        return new Vector2(Random.Range(v2.x - Offset.x, v2.x + Offset.x), Random.Range(v2.y - Offset.y, v2.y + Offset.y));
    }
    /// <summary>
    /// 获得zero点的偏移值内的一个向量
    /// </summary>
    /// <param name="_offset"></param>
    /// <returns></returns>
    public Vector2 getOffset(Vector2 _offset)
    {
        return new Vector2(Random.Range(-_offset.x, _offset.x), Random.Range(-_offset.y, _offset.y));
    }

    /// <summary>
    /// 根据时间去改变当前向量位置
    /// </summary>
    /// <param name="v2"></param>
    /// <returns></returns>
    public Vector2 vector2TimeRote(Vector2 v2)
    {
        return new Vector2(Mathf.Sin(timed) * v2.x, Mathf.Cos(timed) * v2.y);
    }

    /// <summary>
    /// 旋转向量
    /// </summary>
    /// <param name="v2"></param>
    /// <param name="_deviation">旋转的角度</param>
    /// <returns></returns>
    public static Vector2 Vector2Rote(Vector2 v2, float _deviation)
    {
        //求出新的角度
        float sin = Mathf.Sin(Mathf.PI * _deviation / 180);
        float cos = Mathf.Cos(Mathf.PI * _deviation / 180);
        //计算出新的向量
        return new Vector2(v2.x * cos + v2.y * sin, v2.x * -sin + v2.y * cos);
    }





    /// <summary>
    /// 把Y轴长度调给X轴
    /// </summary>
    /// <param name="v2"></param>
    /// <param name="Deviation"></param>
    /// <returns></returns>
    public Vector2 flipYToX(Vector2 v2, float Deviation)
    {
        return new Vector2(v2.x + Deviation, v2.y - Deviation);
    }

}
