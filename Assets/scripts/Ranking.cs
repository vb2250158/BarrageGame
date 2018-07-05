using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


public class Ranking : MonoBehaviour
{
    /// <summary>
    /// 排名样式预制体
    /// </summary>
    public GameObject te;
    /// <summary>
    /// 上下间距
    /// </summary>
    public float interval;
    /// <summary>
    /// 图片样式预制体
    /// </summary>
    public GameObject im;
    /// <summary>
    /// 排名数字样式
    /// </summary>
    public Sprite[] nmuberType;
    //public int itmeLe

    /// <summary>
    /// 玩家排名项
    /// </summary>
    public static ScoreShow playerRanking { get; set; }
    /// <summary>
    /// 排名项
    /// </summary>
    public ScoreShow[] ss { get; set; }
    /// <summary>
    /// 玩家的文本
    /// </summary>
    private Text pt;



    private float time;
    /// <summary>
    /// 排名最大显示个数
    /// </summary>
    public int MaxDisplayNumber;
    /// <summary>
    /// 前排名的个数
    /// </summary>
    public int Rank;
    /// <summary>
    /// 排行关卡的名字
    /// </summary>
    public string levename;

    public Color[] cl;
    private void Awake()
    {
        //读取排名数据
        load(levename);
        //排名列表调整
        Typesetting();
        //提取排名
        RankBeforeExtraction(Rank);
        Moveing = false;

        //玩家当前排名实例化
        playerInstantiate();
    }


    // Update is called once per frame
    void Update()
    {


        //设置玩家当前排名的文本
        pt.text = ((int)Points.time).ToString();

        RefreshRanking();


    }

    /// <summary>
    /// 提取前几名
    /// </summary>
    /// <param name="n"></param>
    public void RankBeforeExtraction(int n)
    {
        //能容纳MaxDisplayNumber个排名
        MaxDisplayNumber--;
        //检测最大排名数是否大于显示数，大于就将前n名提取出来
        if (ss.Length > MaxDisplayNumber)
        {
            for (int i = 0; i < n; i++)
            {
                Debug.Log("提取：" + i);
                //设置第前n名的位置
                ss[i].GetComponent<Transform>().localPosition = ss[(ss.Length - MaxDisplayNumber) + i].GetComponent<Transform>().localPosition;
                ss[i].GetComponent<Transform>().SetParent(transform.parent);
                //设置颜色
                ss[i].GetComponent<Image>().color = cl[1];
            }
        }
        foreach (var item in ss)
        {
            //隐藏排名（隐藏到第几）（起始隐藏排名）
            if (ss[(ss.Length - MaxDisplayNumber) + n - 1].ps.number >= item.ps.number && item.ps.number > n)
            {
                item.GetComponent<CanvasGroup>().alpha = 0;
            }
        }
    }

    /// <summary>
    /// 刷新排名,动画、位置等
    /// </summary>
    public void RefreshRanking()
    {
        if (Moveing)
        {
            return;
        }


        if (playerRanking.ps.number > MaxDisplayNumber + 1)
        {

            //查看分数是否低于
            if ((int)Points.time > ss[playerRanking.ps.number - 2].ps.value)
            {
                Moveing = true;
                //位置下降消失
                StartCoroutine(MoveLocalPositionAndAlpha(
                    transform,
                    transform.localPosition + (interval * Vector3.down),
                    10,
                    ss[playerRanking.ps.number - 2].GetComponent<CanvasGroup>()
                    , ss[playerRanking.ps.number - (MaxDisplayNumber - Rank) - 2].GetComponent<CanvasGroup>()
                    ));
                playerRanking.ps.number = ss[playerRanking.ps.number - 2].ps.number;
                ScoreShowNumberRefresh(playerRanking);

                return;
            }
        }
        else if (playerRanking.ps.number == MaxDisplayNumber + 1)
        {
            //查看分数是否低于
            if ((int)Points.time > ss[playerRanking.ps.number - 2].ps.value)
            {
                Debug.Log("对象转化");
                for (int i = 0; i < Rank; i++)
                {
                    ss[i].GetComponent<Transform>().SetParent(transform);
                }
                playerRanking.GetComponent<Transform>().SetParent(transform);
                Moveing = true;
                //位置上升
                StartCoroutine(ChangeOfPosition(
                    playerRanking.GetComponent<Transform>(),
                    ss[playerRanking.ps.number - 2].GetComponent<Transform>(),
                    10
                    ));
                playerRanking.ps.number = ss[playerRanking.ps.number - 2].ps.number;
                ss[playerRanking.ps.number - 1].ps.number = playerRanking.ps.number + 1;
                ScoreShowNumberRefresh(playerRanking);
                ScoreShowNumberRefresh(ss[playerRanking.ps.number - 1]);
            }
        }
        else if (playerRanking.ps.number>1)
        {
            Debug.Log("排名检测中:" + (playerRanking.ps.number) + ":" + ss[playerRanking.ps.number - 2].ps.number);
            //查看分数是否低于
            if ((int)Points.time > ss[playerRanking.ps.number - 2].ps.value)
            {
                Moveing = true;
                //位置上升
                StartCoroutine(ChangeOfPosition(
                playerRanking.GetComponent<Transform>(),
                ss[playerRanking.ps.number - 2].GetComponent<Transform>(),
                10));
                playerRanking.ps.number = ss[playerRanking.ps.number - 2].ps.number;
                ss[playerRanking.ps.number - 1].ps.number = playerRanking.ps.number + 1;
                ScoreShowNumberRefresh(playerRanking);
                ScoreShowNumberRefresh(ss[playerRanking.ps.number - 1]);
                if (playerRanking.ps.number == 3)
                {
                    Debug.Log("我到前三名啦！");
                    ss[playerRanking.ps.number - 1].GetComponent<Image>().color = ss[playerRanking.ps.number ].GetComponent<Image>().color;
                }
            }
        }
       


    }

    private bool Moveing;

    /// <summary>
    /// UI移动并消失
    /// </summary>
    /// <param name="_tr"></param>
    /// <param name="tg"></param>
    /// <param name="_time"></param>
    /// <param name="_cg"></param>
    /// <returns></returns>
    IEnumerator MoveLocalPositionAndAlpha(Transform _tr, Vector3 tg, float _time, CanvasGroup _cg)
    {


        tg = new Vector3(tg.x, tg.y);
        while (Vector3.Distance(_tr.localPosition, tg) > 1)
        {
            Debug.Log(tg);
            _tr.localPosition = Vector3.MoveTowards(_tr.localPosition, tg, _time);
            yield return null;
            _cg.alpha *= 0.95f;
        }
        _cg.alpha = 0;
        _tr.localPosition = tg;
        Moveing = false;
    }

    /// <summary>
    /// UI移动并 s消失,c显示
    /// </summary>
    /// <param name="_tr"></param>
    /// <param name="tg"></param>
    /// <param name="_time"></param>
    /// <param name="_cgs"></param>
    /// <param name="_cgc"></param>
    /// <returns></returns>
    IEnumerator MoveLocalPositionAndAlpha(Transform _tr, Vector3 tg, float _time, CanvasGroup _cgs, CanvasGroup _cgc)
    {

        tg = new Vector3(tg.x, tg.y);
        _cgc.alpha = 0.1f;
        while (Vector3.Distance(_tr.localPosition, tg) > 1)
        {

            _tr.localPosition = Vector3.MoveTowards(_tr.localPosition, tg, _time);
            yield return null;
            _cgs.alpha *= 0.95f;
            _cgc.alpha *= 1.05f;

        }
        _cgc.alpha = 1;
        _cgs.alpha = 0;
        _tr.localPosition = tg;
        Moveing = false;
    }


    /// <summary>
    /// 玩家排名项初始化
    /// </summary>
    public void playerInstantiate()
    {
        playerRanking = Instantiate(te, transform).GetComponent<ScoreShow>();
        playerRanking
        .SetPs(new PlayerScore("", (int)Points.time, ss.Length + 1))
        .transform.localPosition = Vector3.down * interval * ss.Length;
        pt = playerRanking.transform.Find("Text").GetComponent<Text>();
        pt.fontSize += 20;
        pt.alignment = TextAnchor.MiddleLeft;
        ScoreShowNumberInstantiate(playerRanking);
        pt.transform.parent.SetParent(transform.parent);

        //设置玩家背景色
        playerRanking.GetComponent<Image>().color = cl[0];

    }


    /// <summary>
    /// 读取排名并且实例化
    /// </summary>
    public void load(string levename)
    {
        string[] data=null;
        try
        {
            data = File.ReadAllLines("./"+ levename + ".fds");
        }
        catch (System.Exception)
        {
            Debug.LogError("找不到排名数据");
        }
        

        for (int i = 0; i < data.Length; i++)
        {
            if (data[i].Equals(""))
            {
                continue;
            }
            Debug.Log(Instantiate(te, transform).GetComponent<ScoreShow>().SetPs(JsonUtility.FromJson<PlayerScore>(data[i])));
           
        }
    }




    /// <summary>
    /// 排名调整
    /// </summary>
    public void Typesetting()
    {
        ss = transform.GetComponentsInChildren<ScoreShow>();

        if (ss.Length < 1)
        {
            return;
        }
        for (int i = 0; i < ss.Length; i++)
        {

            for (int j = ss.Length - 1; j > i; j--)
            {

                if (ss[j].ps.value > ss[j - 1].ps.value)
                {
                    ScoreShow s = ss[j];
                    ss[j] = ss[j - 1];
                    ss[j - 1] = s;

                }
            }

        }
        //调整排名元素的位置
        for (int i = 0; i < ss.Length; i++)
        {

            ss[i].transform.localPosition = Vector3.down * interval * i;
            ss[i].ps.number = i + 1;
        }



        //调整整个排行榜位置
        transform.localPosition -= ss[ss.Length - 1].transform.localPosition;

        foreach (var item in ss)
        {
            //排名数值实例化
            ScoreShowNumberInstantiate(item);
            //评级实例化
            sr.displayRank(item.ps.value,item.transform.Find("Rank"));
        }

    }




    public ScoreRecord sr;




    /// <summary>
    /// 排名数值实例化
    /// </summary>
    /// <param name="s"></param>
    public void ScoreShowNumberInstantiate(ScoreShow s)
    {
        for (int i = s.ps.number.ToString().Length; i > 0; i--)
        {
            int n = (s.ps.number % (int)Mathf.Pow(10, i)) / (int)Mathf.Pow(10, i - 1);
            //实例化数值
            Instantiate(im, s.getNumber_C())
                    .GetComponent<Image>().sprite = nmuberType[n];
            
        }
    }

    /// <summary>
    /// 排名数值刷新
    /// </summary>
    /// <param name="s"></param>
    public void ScoreShowNumberRefresh(ScoreShow s)
    {
        Image[] t = s.getNumber_C().GetComponentsInChildren<Image>();
        foreach (var item in t)
        {
            Destroy(item.gameObject);

        }
        ScoreShowNumberInstantiate(s);
    }

    /// <summary>
    /// 位置交换
    /// </summary>
    /// <param name="_tr1"></param>
    /// <param name="_tr2"></param>
    /// <param name="_time"></param>
    /// <returns></returns>
    IEnumerator ChangeOfPosition(Transform _tr1, Transform _tr2, float _time)
    {
        Vector3 tg1 = new Vector3(_tr2.position.x, _tr2.position.y);
        Vector3 tg2 = new Vector3(_tr1.position.x, _tr1.position.y);
        while (Vector3.Distance(_tr1.position, tg1) > 1)
        {
            _tr1.position = Vector3.MoveTowards(_tr1.position, tg1, _time);
            _tr2.position = Vector3.MoveTowards(_tr2.position, tg2, _time);
            yield return null;

        }
        Moveing = false;
    }

    /// <summary>
    /// 移动一个物体的相对位置
    /// </summary>
    /// <param name="_tr"></param>
    /// <param name="tg"></param>
    /// <param name="_time"></param>
    /// <returns></returns>
    IEnumerator MoveLocalPositionTo(Transform _tr, Vector3 tg, float _time)
    {

        tg = new Vector3(tg.x, tg.y);
        while (Vector3.Distance(_tr.localPosition, tg) > 1)
        {
            _tr.localPosition = Vector3.MoveTowards(_tr.localPosition, tg, _time);
            yield return null;

        }

    }
}
