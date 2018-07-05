using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class PlayerScore
{
    public string name;
    public int value;
    public int number;

    public PlayerScore(string name, int value, int number)
    {
        this.name = name;
        this.value = value;
        this.number = number;
    }
    public PlayerScore() {
    }
}

[System.Serializable]
public class RankType {
    public GameObject gj;
    public Vector2 value;
}


public class ScoreRecord : MonoBehaviour {

    public PlayerScore sc { get; set; }


    public GameObject value;
    public GameObject Nuber;
    public GameObject Grade;

    public GameObject im;

    
    
    public Sprite[] nmuberType;
    public RankType[] rt;
    private void Start()
    {
        sc = new PlayerScore();
    }

    /// <summary>
    /// 设置需要序列化的文本
    /// </summary>
    public void SetScoreName() {
        sc.name = transform.Find("InputField").GetComponent<InputField>().text;
        sc.value = (int)(Points.time);
        sc.number = Ranking.playerRanking.ps.number;
    }
    /// <summary>
    /// 序列化保存数据
    /// </summary>
    public void Seve(string levename) {
        //流对象
        StreamWriter stream;
    
        try
        {
            //打开文件
            stream = File.AppendText("./"+levename+".fds");
        }
        catch (System.Exception)
        {
            //创建文件
            stream = File.AppendText("./" + levename + ".fds");
        }
        //写入数据
        stream.WriteLine();
        stream.Write(JsonUtility.ToJson(sc));
        stream.Close();

    }
    public void end()
    {
        ShowNumber(value.transform, (int)(Points.time));
        ShowNumber(Nuber.transform, Ranking.playerRanking.ps.number);
        displayRank();
    }

    /// <summary>
    /// 显示评级
    /// </summary>
    public void displayRank()
    {
        foreach (var item in rt)
        {
            if (item.value.x  <= (int)(Points.time) && (int)(Points.time) < item.value.y)
            {
                Instantiate(item.gj, transform.Find("Grade")).transform.localPosition = Vector3.zero;
            }
        }
    }

    /// <summary>
    /// 显示评级
    /// </summary>
    /// <param name="value"></param>
    /// <param name="tf"></param>
    public void displayRank(int value,Transform tf)
    {
        foreach (var item in rt)
        {
            if (item.value.x <= value && value < item.value.y)
            {
                Instantiate(item.gj, tf).transform.localPosition = Vector3.zero;
            }
        }
    }

    /// <summary>
    /// 生成数字
    /// </summary>
    /// <param name="g"></param>
    /// <param name="number"></param>
    /// <param name="im"></param>
    /// <param name="nmuberType"></param>
    public void ShowNumber(Transform g,int number)
    {
        for (int i = number.ToString().Length; i > 0; i--)
        {
            int n = (number % (int)Mathf.Pow(10, i)) / (int)Mathf.Pow(10, i - 1);
            //实例化数值
            Instantiate(im, g)
                    .GetComponent<Image>().sprite = nmuberType[n];
        }
    }

}
