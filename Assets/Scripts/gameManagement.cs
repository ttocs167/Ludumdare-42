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
    public Text gameOverText;
    public GameObject startScreen;

    private AudioSource shittySong;
    private bool playing;
    private int dogSuprise;
    public AudioClip dogs;
    private bool paused;
    // Use this for initialization
    void Start () {
        started = false;
        coinCount = 0;
        CoinUpdate();
        canvas.gameObject.SetActive(true);
        startScreen.SetActive(true);
        shittySong = gameObject.GetComponent<AudioSource>();
        playing = true;
        paused = false;
        dogSuprise = 0;
    }
	
	// Update is called once per frame
	void Update () {
        if (!started && Input.GetKeyDown("space"))
        {
            started = true;
            startScreen.SetActive(false);

        }

        if (Input.GetButtonDown("Cancel"))
        {
            if (!paused)
            {
                Time.timeScale = 0f;
                pauseScreen.SetActive(true);
                paused = true;
            }
            else
            {
                Time.timeScale = 1f;
                pauseScreen.SetActive(false);
                paused = false;
            }

        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (dogSuprise > 10)
            {
                shittySong.clip = dogs;
            }
            if (playing)
            {
                shittySong.Stop();
            }
            else
            {
                shittySong.Play();
            }
            playing = !playing;
            dogSuprise++;
        }

    }
    public void Death()
    {
        Debug.Log("Death");
        Time.timeScale = 0f;
        gameOverScreen.SetActive(true);
        gameOverText.text = "Final Coin Count: \n" + coinCount.ToString();


    }

    public void CoinUpdate()
    {
        coinText.text = "Coins: " + coinCount.ToString();
    }
}
