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
        StageLocking();
    }
    public void StageLocking()
    {
        StartCoroutine(delayLocking());
    }

    IEnumerator delayLocking()
    {
        yield return new WaitForSeconds(0.05f);
        BattleSceneManager.Instance.curChapterNum = start;
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
                    Debug.Log(i);
                    chapterButtons[i + 1-start].interactable = true;
                }


            }
        }
      
    }
}
