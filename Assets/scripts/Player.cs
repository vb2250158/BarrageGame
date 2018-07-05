using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ChangeKey
{
    public KeyCode speedChange;
    public KeyCode draw;
}

[System.Serializable]
public class MoveClamp
{
    public Vector2 min;
    public Vector2 max;

}
[System.Serializable]
public class Tx
{
    public GameObject draw;
    public GameObject death;
}

public class Player : MonoBehaviour
{

    public float speed;
    public float rateOfChange;

    public ChangeKey changeKey;
    public Sprite[] sp;
    public MoveClamp playerClamp;
    public MoveClamp bgClamp;
    public Tx tx;


    private GameObject bg;
    private SpriteRenderer sr;



    private Vector2 v;


    // Use this for initialization
    void Start()
    {

        sr = GetComponent<SpriteRenderer>();
        bg = GameObject.FindGameObjectWithTag("bg");

    }

    // Update is called once per frame
    void Update()
    {
        v.x = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
        v.y = Input.GetAxis("Vertical") * Time.deltaTime * speed;
        //按下减速
        if (Input.GetKey(changeKey.speedChange))
        {
            sr.sprite = sp[1];
            v *= rateOfChange;

        }
        else 
        {
            sr.sprite = sp[0];
        }

        //按下画画
        if (Input.GetKeyDown(changeKey.draw))
        {

            tx.draw.SetActive(true);
        }
        else if (Input.GetKeyUp(changeKey.draw))
        {
            tx.draw.SetActive(false);
        }

        bg.transform.position -= (Vector3)v / 8;
        transform.position += (Vector3)v;

        //自机移动最大值
        this.gameObject.transform.position = new Vector2(
            Mathf.Clamp(this.gameObject.transform.position.x, playerClamp.min.x, playerClamp.max.x),
            Mathf.Clamp(this.gameObject.transform.position.y, playerClamp.min.y, playerClamp.max.y)
        );
        //背景移动最大值
        bg.gameObject.transform.position = new Vector2(
            Mathf.Clamp(bg.gameObject.transform.position.x, bgClamp.min.x, bgClamp.max.x),
            Mathf.Clamp(bg.gameObject.transform.position.y, bgClamp.min.y, bgClamp.max.y)
        );

       
    }




    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "bg")
        {
            return;
        }
        Destroy(this.gameObject);
    }

}
