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
}
