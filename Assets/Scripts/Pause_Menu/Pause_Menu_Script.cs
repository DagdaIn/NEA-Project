#region includes
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#endregion

public class Pause_Menu_Script : MonoBehaviour
{
    #region Public Variables
    [Header("References")]
    public GameObject PauseMenuUI;

    public static bool GamePaused = false;
    #endregion

    #region Private Variables
    TogglePauseMenu toggleMenu;
    #endregion

    #region Delegates
    public delegate void TogglePauseMenu();
    #endregion

    /// <summary>
    /// Before the first frame, check the game is not paused
    /// </summary>
    void Start()
    {
        Resume();
    }

    /// <summary>
    /// Every frame, checks if the escape key is pressed
    /// When pressed, the game is paused / Resumed
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            toggleMenu();
        }
    }
 
    /// <summary>
    /// Unpauses the game, and de-activates the pause menu
    /// </summary>
    public void Resume()
    {
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GamePaused = false;
        toggleMenu = Pause;
    }

    /// <summary>
    /// Pauses the game, and activates the pause menu
    /// </summary>
    public void Pause()
    {
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GamePaused = true;
        toggleMenu = Resume;
    }

    /// <summary>
    /// Resumes the game, then loads the main menu scene
    /// </summary>
    public void LoadMenu()
    {
        Resume();

        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Quits the application
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }
}
