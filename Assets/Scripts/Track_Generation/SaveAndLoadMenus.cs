#region includes
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System;
#endregion

public class SaveAndLoadMenus : MonoBehaviour
{
    #region public variables
    [Header("References")]
    public GameObject TrackManager;
    public GameObject LoadingDropDownField;
    public GameObject LoadMenu;
    public GameObject SaveMenu;

    public List<Track> allTracks;
    #endregion


    void Start()
    {
        this.allTracks = TrackManager.GetComponent<Track_Manager_Script>().tracks;

        // Guarantees the load and save menus are closed
        DisableLoadMenu();
        DisableSaveMenu();
    }

    /// <summary>
    /// Loads a track by its name as specified in a drop down list
    /// </summary>
    public void LoadTrackByName()
    {
        string name = GameObject.Find("TrackNamesDropdownLabel").GetComponent<TMP_Text>().text;
        this.TrackManager.GetComponent<Track_Manager_Script>().SetTrackByName(name);
        this.TrackManager.GetComponent<Track_Manager_Script>().RenderTrack();
    }

    /// <summary>
    /// Saves a track with the name specified in an input field
    /// </summary>
    public void SaveTrackWithName()
    {
        string name = GameObject.Find("EnterTrackNameText").GetComponent<TMP_Text>().text;

        if (NameAlreadyExists(name))
        {
            print("Name taken!!!");
            return;
        }

        this.TrackManager.GetComponent<Track_Manager_Script>().track.name = name;
        this.TrackManager.GetComponent<Track_Manager_Script>().SaveTrack();
    }

    /// <summary>
    /// Checks whether a track already exists with the given name
    /// </summary>
    /// <param name="name">The name to check if it exists</param>
    /// <returns>True if the name exists, false otherwise</returns>
    private bool NameAlreadyExists(string name)
    {
        // Some special character is added to the end when entered, so this strips that character
        name = name.Substring(0, name.Length - 1);
        
        foreach (Track track in allTracks)
        {
            if (string.Equals(track.name, name))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Writes the names of all tracks to a dropdown field
    /// </summary>
    public void SetDropDownNames()
    {
        TMP_Dropdown menu = LoadingDropDownField.GetComponent<TMP_Dropdown>();

        menu.options.Clear();

        foreach (Track track in allTracks)
        {
            menu.options.Add(new TMP_Dropdown.OptionData() {text = $"{track.name}"});
        }

        GameObject.Find("TrackNamesDropdownLabel").GetComponent<TMP_Text>().SetText(allTracks[0].name);
    }

    /// <summary>
    /// Opens the Load UI
    /// </summary>
    public void EnableLoadMenu()
    {
        DisableSaveMenu();
        this.LoadMenu.SetActive(true);

        this.allTracks = TrackManager.GetComponent<Track_Manager_Script>().tracks;
        SetDropDownNames();
    }

    /// <summary>
    /// Closes the Load UI
    /// </summary>
    public void DisableLoadMenu()
    {
        this.LoadMenu.SetActive(false);
    }

    /// <summary>
    /// Opens the Save UI
    /// </summary>
    public void EnableSaveMenu()
    {
        DisableLoadMenu();
        this.SaveMenu.SetActive(true);
    }

    /// <summary>
    /// Closes the save UI
    /// </summary>
    public void DisableSaveMenu()
    {
        this.SaveMenu.SetActive(false);
    }
}
