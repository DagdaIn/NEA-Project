#region includes
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
#endregion

#region structures
/// <summary>
/// Stores information about all leaderboard times
/// </summary>
public struct LeaderboardTime
{
    /// <summary>
    /// Provides a helpful way of constructing a structure using the raw data read in from a file
    /// </summary>
    /// <param name="fields">All the fields of the structure represented as strings</param>
    public LeaderboardTime(string[] fields)
    {
        time = float.Parse(fields[0]);
        dateSet = DateTime.Parse(fields[1]);
        userNameSet = fields[2];
        trackNameSet = fields[3];
    }

    public float time;
    public DateTime dateSet;
    public string userNameSet;
    public string trackNameSet;
}
#endregion

public class Load_Leaderboard : MonoBehaviour
{
    #region Public Variables
    [Header("References")]
    public GameObject LeaderboardText;

    [Header("Save/Load Location")]
    public string path = "./Leaderboard.txt";
    #endregion

    #region Private Variables
    #endregion

    #region Methods

    #region Unity Methods
    void Start()
    {
        PlayerData thisPlayer = ReadPlayerData();
        List<LeaderboardTime> times = ParseTimesByUser(thisPlayer.name);

        WriteTopTimesToLeaderboard(times);
    }
    #endregion

    /// <summary>
    /// Loads all the times stored in the file defined as path
    /// </summary>
    /// <returns>A list of LeaderboardTime structures about all times stored</returns>
    public List<LeaderboardTime> LoadAllTimes()
    {
        // Create a list of all leaderboard times
        List<LeaderboardTime> times = new List<LeaderboardTime>();

        // Read from the file
        using (FileStream stream = new FileStream(path, FileMode.Open))
        {
            using (StreamReader sr = new StreamReader(stream))
            {
                while (!sr.EndOfStream)
                {
                    // - Assume structure is in following format
                    // time`dateset`userNameSet`trackNameSet
                    string[] timeAsString = sr.ReadLine().Split('`');

                    LeaderboardTime temp = new LeaderboardTime(timeAsString);

                    times.Add(temp);
                }
            }
        }

        return times;
    }

    /// <summary>
    /// Finds all times set by any user with the username <paramref name="name"/>
    /// </summary>
    /// <param name="name">The name of the user whos times should be searched for</param>
    /// <returns>A list of LeaderboardTime structures with the usernameSet equal to the name</returns>
    public List<LeaderboardTime> ParseTimesByUser(string name)
    {
        // Defines a list for specifically the userTimes, and loads all times stored
        List<LeaderboardTime> userTimes = new List<LeaderboardTime>();
        List<LeaderboardTime> allTimes = LoadAllTimes();

        // Adds each time set by any user with the name provided
        foreach (LeaderboardTime time in allTimes)
        {
            if (time.userNameSet == name)
            {
                userTimes.Add(time);
            }
        }

        SortTimes(userTimes);

        return userTimes;
    }

    public void SortTimes(List<LeaderboardTime> times)
    {
        times.Sort((x, y) => x.time.CompareTo(y.time));
    }

    public void WriteTopTimesToLeaderboard(List<LeaderboardTime> times)
    {
        string text = "";

        for (int i = 0; i < times.Count; i++)
        {
            text = $"{TimeSpan.FromSeconds(times[i].time)}".Substring(0, 12);
            text = $"{i + 1}) {text}\n";
        }

        LeaderboardText.GetComponent<TextMeshProUGUI>().SetText(text);
    }

    private PlayerData ReadPlayerData()
    {
        PlayerData data = new PlayerData();
        string playerDataPath = "./PlayerData.txt";

        string[] dataString;

        using (FileStream stream = new FileStream(playerDataPath, FileMode.Open))
        {
            using (StreamReader sr = new StreamReader(stream))
            {
                dataString = sr.ReadLine().Split('`');
            }
        }

        // Assumes all values are strings, may need to be reworked
        data = new PlayerData(dataString);

        return data;
    }

    #endregion
}
