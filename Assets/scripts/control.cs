using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class control : MonoBehaviour {

    public int speed;   //自机飞行速度   
    public float H;     //自机x轴运动
    public float V;     //自机y轴运动
    public GameObject bg;   //背景物体

    public float xMin;  //屏幕x最小值
    public float xMax;  //屏幕x最大值
    public float yMin;  //屏幕y最小值
    public float yMax;  //屏幕y最大值
    
    public Sprite huansu;   //缓速图片
    public Sprite normal;   //原始图片
    void Update()
    {
  
        if (Input.GetKeyDown(KeyCode.LeftShift))    //缓速开启
        {
            GetComponent<Image>().sprite = huansu;      //缓速图片
            speed = speed / 2;  
        }
        if(Input.GetKeyUp(KeyCode.LeftShift))   //缓速关闭
        {
            GetComponent<Image>().sprite = normal;      //原始图片
            speed = speed * 2;
        }
        //自机移动
        H = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
        V = Input.GetAxis("Vertical") * Time.deltaTime * speed;
        this.gameObject.transform.Translate(H, V, 0);
        //背景移动

        //背景移动最大值
        bg.gameObject.transform.position = new Vector3(
            Mathf.Clamp(bg.gameObject.transform.position.x, 3, 1008),
            Mathf.Clamp(bg.gameObject.transform.position.y, 415, 596),
            Mathf.Clamp(bg.gameObject.transform.position.z, -506, 500)
        );
        //自机移动最大值
        this.gameObject.transform.position = new Vector3(
            Mathf.Clamp(this.gameObject.transform.position.x, xMin, xMax),
            Mathf.Clamp(this.gameObject.transform.position.y, yMin, yMax),
            100
        );
    }
}
