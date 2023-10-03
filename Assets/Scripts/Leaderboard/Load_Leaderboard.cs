using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Load_Leaderboard : MonoBehaviour
{
    public GameObject LeaderboardText;
    // Start is called before the first frame update
    void Start()
    {
        LeaderboardText.GetComponent<TextMeshProUGUI>().SetText("This is another test");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
<<<<<<< Updated upstream
=======

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
            text = $"{TimeSpan.FromSeconds(times[i].time)}";
            text = text.Substring(0, 12);
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
>>>>>>> Stashed changes
}
