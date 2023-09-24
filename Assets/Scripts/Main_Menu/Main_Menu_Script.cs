using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Menu_Script : MonoBehaviour
{
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void Play()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadTrackGenerator()
    {
        SceneManager.LoadScene(2);
    }

    public void LoadLeaderboard()
    {
        SceneManager.LoadScene(3);
    }

    public void LoadTimeTrial()
    {
        SceneManager.LoadScene(4);
    }
}
