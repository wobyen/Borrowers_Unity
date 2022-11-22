using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuHandler : InputManager
{

    public static bool GameIsPaused = false;
    public PlayerManager playerManager;

    public GameObject pauseMenuUI;


    private void Start()
    {

        ResumeGame();
    }


    private void Update()
    {

        if (pauseAction.triggered)
        {
            if (GameIsPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }



    private void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;

    }

    private void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }


}
