#region includes
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
#endregion

public class Main_Menu_Script : MonoBehaviour
{
    #region LoadScenes
    /// <summary>
    /// Loads the main menu scene
    /// </summary>
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Loads the neural network scene
    /// </summary>
    public void Play()
    {
        SceneManager.LoadScene(1);
    }

    /// <summary>
    /// Loads the track generator scene
    /// </summary>
    public void LoadTrackGenerator()
    {
        SceneManager.LoadScene(2);
    }

    /// <summary>
    /// Loads the leaderboard scene
    /// </summary>
    public void LoadLeaderboard()
    {
        SceneManager.LoadScene(3);
    }

    /// <summary>
    /// Loads the time trial scene
    /// </summary>
    public void LoadTimeTrial()
    {
        SceneManager.LoadScene(4);
    }
    #endregion
}
