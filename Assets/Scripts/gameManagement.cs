using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class gameManagement : MonoBehaviour {
    public bool started;
    public int coinCount;
    public Text coinText;
    public Canvas canvas;
    public GameObject pauseScreen;
    public GameObject gameOverScreen;

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

        if (Input.GetButtonDown("Cancel"))
        {
            Time.timeScale = 0f;
            pauseScreen.SetActive(true);

        }

    }
    public void Death()
    {
        Debug.Log("Death");
        Time.timeScale = 0f;
        gameOverScreen.SetActive(true);

    }

    public void CoinUpdate()
    {
        coinText.text = "Coins: " + coinCount.ToString();
    }
}
