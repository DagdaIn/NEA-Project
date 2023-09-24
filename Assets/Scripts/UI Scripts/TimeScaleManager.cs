using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaleManager : MonoBehaviour
{
    public void ChangeTimeScale (float newTimeScale)
    {
        Time.timeScale = newTimeScale;
    }
}
