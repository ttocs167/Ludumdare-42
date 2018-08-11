using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManagement : MonoBehaviour {
    public bool started;
    public int coinCount;


	// Use this for initialization
	void Start () {
        started = false;
        coinCount = 0;
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!started && Input.GetKeyDown("space"))
        {
            started = true;
        }
        Debug.Log(coinCount);
	}
    public void Death()
    {
        Debug.Log("Death");
    }
}
