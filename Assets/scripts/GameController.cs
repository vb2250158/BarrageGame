using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public Player player;

    public float liveTime;

    public int hp = 1;

    public bool gameEnd;

    public int Hp
    {
        get => hp; set
        {
            hp = value;
            if (hp <= 0)
            {
                OnDie();
            }
        }
    }

    private void OnDie()
    {
        gameEnd = true;
        player.gameObject.SetActive(false);
    }

    private void Start()
    {
        player.onHit = OnHit;
    }

    private void Update()
    {
        if (gameEnd)
        {
            return;
        }
        liveTime += Time.deltaTime;
        Points.Time = liveTime;
    }

    private void OnHit()
    {
        Hp--;
    }
}
