#region includes
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#endregion

public class Failure_Menu_Script : MonoBehaviour
{
    #region Public Variables
    [Header("References")]
    public GameObject LossMenu;
    public GameObject Player;
    public GameObject TimeManager;

    public ToggleMenu toggleMenu;
    #endregion

    #region methods
    #region delegates
    public delegate void ToggleMenu();
    #endregion

    /// <summary>
    /// Starts with the failure menu disabled
    /// </summary>
    public void Start()
    {
        DisableMenu();
    }

    /// <summary>
    /// Pauses the game, and enables the menu
    /// </summary>
    public void EnableMenu()
    {
        Time.timeScale = 0f;
        this.LossMenu.SetActive(true);
        this.toggleMenu = DisableMenu;
    }

    /// <summary>
    /// Resumes the game, and disables the menu
    /// </summary>
    public void DisableMenu()
    {
        Time.timeScale = 1f;
        this.LossMenu.SetActive(false);
        this.toggleMenu = EnableMenu;
    }

    /// <summary>
    /// Resets the player and timer, then disables the menu
    /// </summary>
    public void Reset()
    {
        this.Player.SetActive(false);
        this.TimeManager.SetActive(false);

        this.Player.GetComponent<Vehicle_Control>().InitialisePlayer();
        this.TimeManager.GetComponent<ManageTimer>().ResetTimer();

        this.Player.SetActive(true);
        this.TimeManager.SetActive(true);

        DisableMenu();
    }
    #endregion
}
