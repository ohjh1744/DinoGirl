using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageLock : MonoBehaviour
{
    [SerializeField] bool[] stagesCleared;
    [SerializeField] Button[] chapterButtons;


     
    private void OnEnable()
    {
        //for (int i = 0; i < PlayerDataManager.Instance.PlayerData.IsStageClear.Length; i++)
        //{
        //    if (PlayerDataManager.Instance.PlayerData.IsStageClear[i] == false)
        //    {
        //        if (i+1 < PlayerDataManager.Instance.PlayerData.IsStageClear.Length) 
        //        {
        //           chapterButtons[i + 1].interactable = false;
        //        }
        //        else
        //            break;
                
        //    }
        //}
    }
    public void StageLocking() 
    {
        StartCoroutine(delayLocking());
    }

    IEnumerator delayLocking() // 인덱스 벗어나는거 해결해야 함 
    {
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < PlayerDataManager.Instance.PlayerData.IsStageClear.Length; i++)
        {
            if (PlayerDataManager.Instance.PlayerData.IsStageClear[i] == false)
            {
                if (i + 1 < PlayerDataManager.Instance.PlayerData.IsStageClear.Length)
                {
                    chapterButtons[i + 1].interactable = false;
                }
                else
                    break;

            }
        }
    }
}
