using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoad : MonoBehaviour {


    public KeyCode reset;
    public int loadID;

    public GameObject loadText;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(reset))
        {
            SceneManager.LoadSceneAsync(loadID);
        }
        
	}

    public void load(int level)
    {
        if (loadText!=null)
        {
            loadText.SetActive(true);
        }
        SceneManager.LoadSceneAsync(level);
    }

  
}
