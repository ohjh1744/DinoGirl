using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TimeScaleTester : MonoBehaviour
{
    public void ToggleTimeScale()
    {
        
            if (Time.timeScale == 0)
            {
                Debug.Log("타임 스케일 1");
                Time.timeScale = 1;
                
            }
                
            else if (Time.timeScale != 0)
            {
                Debug.Log("타임 스케일 0");
                Time.timeScale = 0;
            }
                
        
    }

}
