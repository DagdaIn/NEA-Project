using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;

public struct PlayerData
{
    public PlayerData(string[] values)
    {
        this.name = values[0];
    }

    public string name;
}

public class UserName_Menu_Script : MonoBehaviour
{
    public GameObject usernameMenu;
    public GameObject mainMenu;
    private toggleUsernameMenu ToggleUsernameMenu;
    public GameObject inputText;

    public void Start()
    {
        // Remember to enable usernamemenu for the build
        // EnableUsernameMenu();

        inputText.GetComponent<TMP_InputField>().onEndEdit.AddListener(SubmitField);
    }

    public void EnableUsernameMenu()
    {
        mainMenu.SetActive(false);
        usernameMenu.SetActive(true);
        this.ToggleUsernameMenu = DisableUsernameMenu;
    }

    public void DisableUsernameMenu()
    {
        mainMenu.SetActive(true);
        usernameMenu.SetActive(false);
        this.ToggleUsernameMenu = EnableUsernameMenu;
    }

    public void SubmitField(string input)
    {
        // Save the name to a file
        PlayerData dataToWrite = new PlayerData();
        dataToWrite.name = input;
        WritePlayerDataToFile(dataToWrite);
        DisableUsernameMenu();

        SetName();
    }

    private void WritePlayerDataToFile(PlayerData data)
    {
        string writeToFile = "";

        // https://stackoverflow.com/questions/7613782/iterating-through-struct-members
        #region StackOverflow
        foreach (var field in typeof(PlayerData).GetFields(BindingFlags.Instance | BindingFlags.Public))
        {
            writeToFile += $"{field.GetValue(data)}`";
        }
        #endregion

        string filename = "PlayerData.txt";
        using (StreamWriter sw = new StreamWriter(filename, false))
        {
            sw.WriteLine(writeToFile);
        }
    }

    private void SetName()
    {
        PlayerData data = ReadPlayerData();
        mainMenu.transform.Find("UsernameText").GetComponent<TMP_Text>().SetText(data.name);
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

    public delegate void toggleUsernameMenu();
}
