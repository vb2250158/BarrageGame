using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Quad : MonoBehaviour {



	// Use this for initialization
	void Start () {
  
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    private void OnTriggerExit2D(Collider2D collision)
    {
        Destroy(collision.gameObject);
    }
}
