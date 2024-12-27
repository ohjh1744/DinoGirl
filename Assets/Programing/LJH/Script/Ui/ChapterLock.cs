using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChapterLock : MonoBehaviour
{
    [SerializeField] bool[] stagesCleared;
    [SerializeField] Button[] chapterButtons;



    private void OnEnable()
    {
        for (int i = 0; i < stagesCleared.Length; i++) 
        {
            if (stagesCleared[6] == false) 
            {
                chapterButtons[1].interactable = false;
                chapterButtons[2].interactable = false;
            }   
        }
    }
}
