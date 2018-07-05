using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class game : MonoBehaviour {

    public static float timer = 0;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        //Debug.Log(timer.ToString());
    }
}
