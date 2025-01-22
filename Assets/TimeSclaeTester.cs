using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeSclaeTester : MonoBehaviour
{
    public bool isSpeed = false;
    private void Update()
    {
        Debug.Log(Time.timeScale);
    }

    public void ToggleTimeSclae()
    {
        if (isSpeed)
        {
            Time.timeScale = 3.0f;
        }
        else
        {
            Time.timeScale = 1.0f;
        }

        
    }
}
