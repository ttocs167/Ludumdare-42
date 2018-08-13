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
    private GameObject player;
    private float startHeight;
    private float endHeight;
    private float totalHeight;
    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        startHeight = player.transform.position.y;
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
        endHeight = player.transform.position.y;
        totalHeight = endHeight - startHeight;
        gameOverText.text = "Final Score: \n" + (coinCount * 100f).ToString() + "\n Total Height: \n" + Mathf.Round(endHeight).ToString() + "m";
    }

    public void CoinUpdate()
    {
        coinText.text = "Score: " + (coinCount * 100f).ToString();
    }
}
