#region includes
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
#endregion

public class ManageTimer : MonoBehaviour
{
    #region public variables
    [Header("References")]
    public GameObject timerText;
    public GameObject Player;
    public GameObject trackManager;

    #endregion

    #region private variables
    private float raceTimer;
    private UpdateTimer timerManager;

    #endregion

    #region delegates
    private delegate void UpdateTimer();
    #endregion

    #region MonoBehaviourSubroutines
    void Start()
    {
        timerManager = InitialiseTimer;
    }

    void Update()
    {
        timerManager();

        if (Player.GetComponent<Vehicle_Control>().lastCheckpoint == 2)
        {
            StartTimer();
        }

        if (Player.GetComponent<Vehicle_Control>().completedLap)
        {
            SaveTime(raceTimer);
            Player.GetComponent<Vehicle_Control>().completedLap = false;
            InitialiseTimer();
        }
    }

    /// <summary>
    /// Restarts the timer
    /// </summary>
    public void ResetTimer()
    {
        InitialiseTimer();
    }

    /// <summary>
    /// Starts increasing the timer
    /// </summary>
    private void StartTimer()
    {
        timerManager = IncreaseTimer;
    }

    /// <summary>
    /// Increases the timer by time.deltaTime every frame, and writes the new time to the screen
    /// </summary>
    private void IncreaseTimer()
    {
        raceTimer += Time.deltaTime;
        timerText.GetComponent<TextMeshProUGUI>().SetText($"<mspace=0.7em>{TimeSpan.FromSeconds(raceTimer):g}".PadRight(22,'.').PadRight(25, '0'));
    }

    /// <summary>
    /// Sets the value of the timer to 0, and writes to the screen
    /// </summary>
    private void InitialiseTimer()
    {
        raceTimer = 0f;
        timerText.GetComponent<TextMeshProUGUI>().SetText($"<mspace=0.7em>{TimeSpan.FromSeconds(raceTimer):g}".PadRight(22, '.').PadRight(25, '0'));
    }

    /// <summary>
    /// Saves the current timer value to "Leaderboard.txt"
    /// </summary>
    /// <param name="time">The float representing the lap time, in seconds</param>
    private void SaveTime(float time)
    {
        PlayerData thisPlayer = ReadPlayerData();
        LeaderboardTime thisTime = new LeaderboardTime();

        string path = "./Leaderboard.txt";

        thisTime.time = time;
        thisTime.dateSet = DateTime.Today;
        thisTime.userNameSet = thisPlayer.name;
        thisTime.trackNameSet = this.trackManager.GetComponent<Track_Manager_Script>().track.name;

        using (FileStream stream = new FileStream(path, FileMode.Append))
        {
            using (StreamWriter sw = new StreamWriter(stream))
            {
                sw.WriteLine($"{thisTime.time}`{thisTime.dateSet}`{thisTime.userNameSet}`{thisTime.trackNameSet}");
            }
        }
    }

    /// <summary>
    /// Reads in the current player data from "PlayerData.txt"
    /// </summary>
    /// <returns>A PlayerData object containing all the player data</returns>
    private PlayerData ReadPlayerData()
    {
        PlayerData data = new PlayerData();
        string playerDataPath = "./PlayerData.txt";

        string[] dataString;

        using (FileStream stream = new FileStream(playerDataPath, FileMode.Open))
        {
            using (StreamReader sr = new StreamReader(stream))
            {
                dataString = sr.ReadLine().Split('\'');
            }
        }

        // Assumes all values are strings, may need to be reworked
        data = new PlayerData(dataString);

        return data;
    }
    #endregion

    
}
