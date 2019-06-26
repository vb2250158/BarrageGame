using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// code by 可爱的小陶
/// </summary>
public class Points : MonoBehaviour
{

    public static float Time;
    public Sprite[] sp;
    public GameObject pj;
    private GameObject point1a;
    private GameObject point2a;
    private GameObject point3a;
    private GameObject point4a;
    private GameObject point5a;

    private GameObject player;

    private SpriteRenderer point1;
    private SpriteRenderer point2;
    private SpriteRenderer point3;
    private SpriteRenderer point4;
    private SpriteRenderer point5;

    private SpriteRenderer sr;

    public int totalTime;
    // Use this for initialization
    void Start()
    {         //11743
        Time = 0;
        point1a = GameObject.Find("1");
        point2a = GameObject.Find("10");
        point3a = GameObject.Find("100");
        point4a = GameObject.Find("1000");
        point5a = GameObject.Find("10000");

        point1 = point1a.GetComponent<SpriteRenderer>();
        point2 = point2a.GetComponent<SpriteRenderer>();
        point3 = point3a.GetComponent<SpriteRenderer>();
        point4 = point4a.GetComponent<SpriteRenderer>();
        point5 = point5a.GetComponent<SpriteRenderer>();

        player = GameObject.Find("Player");
        end = false;
    }
    bool end;
    // Update is called once per frame
    void Update()
    {
        if (player == null || Time >= totalTime)
        {
            if (!end)
            {
               
                end = true;
                pj.SetActive(true);
                pj.GetComponent<ScoreRecord>().end();
               
            }
            
        }
        else
        {
          
            Time = Time * 100;

            float a = Time;
            float a5 = a / 10000;
            float a4 = (a % 10000) / 1000;
            float a3 = (a % 1000) / 100;
            float a2 = (a % 100) / 10;
            float a1 = (a % 10) / 1;

            int b1 = (int)(a1);
            int b2 = (int)(a2);
            int b3 = (int)(a3);
            int b4 = (int)(a4);
            int b5 = (int)(a5);

            //Debug.Log(b1);

            switch (b1)
            {
                case 0:
                    point1.sprite = sp[0];
                    break;
                case 1:
                    point1.sprite = sp[1];
                    break;
                case 2:
                    point1.sprite = sp[2];
                    break;
                case 3:
                    point1.sprite = sp[3];
                    break;
                case 4:
                    point1.sprite = sp[4];
                    break;
                case 5:
                    point1.sprite = sp[5];
                    break;
                case 6:
                    point1.sprite = sp[6];
                    break;
                case 7:
                    point1.sprite = sp[7];
                    break;
                case 8:
                    point1.sprite = sp[8];
                    break;
                case 9:
                    point1.sprite = sp[9];
                    break;
            }

            switch (b2)
            {
                case 0:
                    point2.sprite = sp[0];
                    break;
                case 1:
                    point2.sprite = sp[1];
                    break;
                case 2:
                    point2.sprite = sp[2];
                    break;
                case 3:
                    point2.sprite = sp[3];
                    break;
                case 4:
                    point2.sprite = sp[4];
                    break;
                case 5:
                    point2.sprite = sp[5];
                    break;
                case 6:
                    point2.sprite = sp[6];
                    break;
                case 7:
                    point2.sprite = sp[7];
                    break;
                case 8:
                    point2.sprite = sp[8];
                    break;
                case 9:
                    point2.sprite = sp[9];
                    break;
            }

            switch (b3)
            {
                case 0:
                    point3.sprite = sp[0];
                    break;
                case 1:
                    point3.sprite = sp[1];
                    break;
                case 2:
                    point3.sprite = sp[2];
                    break;
                case 3:
                    point3.sprite = sp[3];
                    break;
                case 4:
                    point3.sprite = sp[4];
                    break;
                case 5:
                    point3.sprite = sp[5];
                    break;
                case 6:
                    point3.sprite = sp[6];
                    break;
                case 7:
                    point3.sprite = sp[7];
                    break;
                case 8:
                    point3.sprite = sp[8];
                    break;
                case 9:
                    point3.sprite = sp[9];
                    break;
            }

            switch (b4)
            {
                case 0:
                    point4.sprite = sp[0];
                    break;
                case 1:
                    point4.sprite = sp[1];
                    break;
                case 2:
                    point4.sprite = sp[2];
                    break;
                case 3:
                    point4.sprite = sp[3];
                    break;
                case 4:
                    point4.sprite = sp[4];
                    break;
                case 5:
                    point4.sprite = sp[5];
                    break;
                case 6:
                    point4.sprite = sp[6];
                    break;
                case 7:
                    point4.sprite = sp[7];
                    break;
                case 8:
                    point4.sprite = sp[8];
                    break;
                case 9:
                    point4.sprite = sp[9];
                    break;
            }

            switch (b5)
            {
                case 0:
                    point5.sprite = sp[0];
                    break;
                case 1:
                    point5.sprite = sp[1];
                    break;
                case 2:
                    point5.sprite = sp[2];
                    break;
                case 3:
                    point5.sprite = sp[3];
                    break;
                case 4:
                    point5.sprite = sp[4];
                    break;
                case 5:
                    point5.sprite = sp[5];
                    break;
                case 6:
                    point5.sprite = sp[6];
                    break;
                case 7:
                    point5.sprite = sp[7];
                    break;
                case 8:
                    point5.sprite = sp[8];
                    break;
                case 9:
                    point5.sprite = sp[9];
                    break;
            }
        }

    }
}
