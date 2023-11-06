#region includes
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#endregion

public class Pause_Menu_Script : MonoBehaviour
{
    #region Public Variables
    [Header("References")]
    public GameObject PauseMenuUI;
    public GameObject NetworkOptionsUI;
    public GameObject TimeScaleUI;
    public GameObject player;

    public static bool GamePaused = false;
    #endregion

    #region Private Variables
    TogglePauseMenu togglePause;
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
            togglePause();
        }
    }
 
    /// <summary>
    /// Unpauses the game, and de-activates the pause menu
    /// </summary>
    public void Resume()
    {
        DisableOptionsMenu();
        PauseMenuUI.SetActive(false);
        TimeScaleUI.SetActive(true);
        Time.timeScale = 1f;
        GamePaused = false;
        togglePause = Pause;
    }

    /// <summary>
    /// Pauses the game, and activates the pause menu
    /// </summary>
    public void Pause()
    {
        DisablePlayer();
        PauseMenuUI.SetActive(true);
        TimeScaleUI.SetActive(false);
        Time.timeScale = 0f;
        GamePaused = true;
        togglePause = Resume;
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

    /// <summary>
    /// Activates the options menu
    /// </summary>
    public void EnableOptionsMenu()
    {
        NetworkOptionsUI.SetActive(true);
        PauseMenuUI.SetActive(false);
        togglePause = DisableOptionsMenu;
    }

    /// <summary>
    /// Deactivates the options menu
    /// </summary>
    public void DisableOptionsMenu()
    {
        NetworkOptionsUI.SetActive(false);
        PauseMenuUI.SetActive(true);
        togglePause = EnableOptionsMenu;
    }

    public void EnablePlayer()
    {
        player.SetActive(true);
        player.GetComponent<Vehicle_Control>().InitialisePlayer();
    }

    public void DisablePlayer()
    {
        player.SetActive(false);
    }
}
