using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TimeShow : MonoBehaviour
{

    public BarrageLauncher gp;
    private Text t;
    // Use this for initialization
    void Start()
    {
        t = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        t.text = gp.timed.ToString();

    }
}
