using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Failure_Menu_Script : MonoBehaviour
{
    [Header("References")]
    public GameObject LossMenu;
    public GameObject Player;
    public GameObject TimeManager;

    public ToggleMenu toggleMenu;

    #region methods
    #region delegates
    public delegate void ToggleMenu();
    #endregion

    public void Start()
    {
        DisableMenu();
    }

    public void EnableMenu()
    {
        Time.timeScale = 0f;
        this.LossMenu.SetActive(true);
        this.toggleMenu = DisableMenu;
    }

    public void DisableMenu()
    {
        Time.timeScale = 1f;
        this.LossMenu.SetActive(false);
        this.toggleMenu = EnableMenu;
    }

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
