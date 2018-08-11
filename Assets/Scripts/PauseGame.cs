using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGame : MonoBehaviour {

    public void pauseUnpause(float timeScale)
    {
        Time.timeScale = timeScale;
    }
}
