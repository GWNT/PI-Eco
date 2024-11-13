using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public AudioSource backgroundMusic;

    void Start()
    {
        
    }

    
    void Update()
    {
        if (PauseMenu.GameIsPaused)
        {
            backgroundMusic.Pause();
        } else if (!PauseMenu.GameIsPaused)
        {
            backgroundMusic.UnPause();
        }
    }
}
