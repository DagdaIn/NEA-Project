using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using TMPro;

public class SaveAndLoadMenus : MonoBehaviour
{
    public List<Track> allTracks;

    [Header("References")]
    public GameObject TrackManager;
    public GameObject LoadingDropDownField;

    void Start()
    {
        this.allTracks = TrackManager.GetComponent<Track_Manager_Script>().tracks;
    }
}
