using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gameManagement : MonoBehaviour {
    public bool started;
    public int coinCount;
    public Text coinText;
    public Canvas canvas;

	// Use this for initialization
	void Start () {
        started = false;
        coinCount = 0;
        CoinUpdate();
        canvas.gameObject.SetActive(true);

    }
	
	// Update is called once per frame
	void Update () {
        if (!started && Input.GetKeyDown("space"))
        {
            started = true;
        }
    }
    public void Death()
    {
        Debug.Log("Death");
    }

    public void CoinUpdate()
    {
        coinText.text = "Coins: " + coinCount.ToString();
    }
}
