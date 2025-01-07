using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageLock : MonoBehaviour
{

    [SerializeField] Button[] chapterButtons;
    [SerializeField] int start;
    [SerializeField] int end;



    private void OnEnable()
    {
        for (int i = 1; i < chapterButtons.Length; i++)
        {
            chapterButtons[i].interactable = false;
        }
        for (int i = start; i < end; i++) 
        {
            if (PlayerDataManager.Instance.PlayerData.IsStageClear[i] == true)
            {
                if (i != end) 
                {
                    chapterButtons[i + 1].interactable = true;
                }
                

            }
        }
    }
    //public void StageLocking()
    //{
    //    StartCoroutine(delayLocking());
    //}

    //IEnumerator delayLocking()
    //{
    //    for (int i = 0; i < chapterButtons.Length; i++)
    //    {
    //        chapterButtons[i].interactable = false;
    //    }

    //    yield return new WaitForSeconds(1f);
    //    for (int i = 0; i < PlayerDataManager.Instance.PlayerData.IsStageClear.Length; i++)
    //    {
    //        if (PlayerDataManager.Instance.PlayerData.IsStageClear[i] == false)
    //        {
    //            if (i + 1 < PlayerDataManager.Instance.PlayerData.IsStageClear.Length)
    //            {
    //                chapterButtons[i + 1].interactable = false;
    //            }
    //            else
    //                break;

    //        }
    //    }
    //}
}
