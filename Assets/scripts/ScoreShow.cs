using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ScoreShow : MonoBehaviour {
    public PlayerScore ps { get; set; }
    
    void Start () {
		
	}
	
	
	void Update () {
		
	}
    /// <summary>
    /// 设置文本内容
    /// </summary>
    /// <param name="_ps"></param>
    /// <returns></returns>
    public ScoreShow SetPs(PlayerScore _ps) {
        ps=_ps;
        transform.Find("Text").GetComponent<Text>().text=(ps.name+"\n"+ps.value);
        return this;
    }
    /// <summary>
    /// 获得数字显示的transform
    /// </summary>
    /// <returns></returns>
    public Transform getNumber_C() {
        return transform.Find("Number");
    }
    

}
