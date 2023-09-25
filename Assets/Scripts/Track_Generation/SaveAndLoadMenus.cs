using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System;

public class SaveAndLoadMenus : MonoBehaviour
{
    public List<Track> allTracks;

    [Header("References")]
    public GameObject TrackManager;
    public GameObject LoadingDropDownField;
    public GameObject LoadMenu;
    public GameObject SaveMenu;


    void Start()
    {
        this.allTracks = TrackManager.GetComponent<Track_Manager_Script>().tracks;

        // Guarantees the load and save menus are closed
        DisableLoadMenu();
        DisableSaveMenu();
    }

    public void LoadTrackByName()
    {
        string name = GameObject.Find("TrackNamesDropdownLabel").GetComponent<TMP_Text>().text;
        this.TrackManager.GetComponent<Track_Manager_Script>().SetTrackByName(name);
        this.TrackManager.GetComponent<Track_Manager_Script>().RenderTrack();
    }

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

    public void EnableLoadMenu()
    {
        DisableSaveMenu();
        this.LoadMenu.SetActive(true);

        this.allTracks = TrackManager.GetComponent<Track_Manager_Script>().tracks;
        SetDropDownNames();
    }

    public void DisableLoadMenu()
    {
        this.LoadMenu.SetActive(false);
    }

    public void EnableSaveMenu()
    {
        DisableLoadMenu();
        this.SaveMenu.SetActive(true);
    }

    public void DisableSaveMenu()
    {
        this.SaveMenu.SetActive(false);
    }
}
