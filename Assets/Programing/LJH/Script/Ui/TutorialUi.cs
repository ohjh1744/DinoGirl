using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUi : MonoBehaviour
{
    [SerializeField] Sprite[] tutoImages;
    [SerializeField] GameObject tutoPanel;
    [SerializeField] Image tutoIamge;
    [SerializeField] int curCount;

    private void OnEnable()
    {
        if (PlayerDataManager.Instance.PlayerData.IsStageClear[0] == false) 
        {
            Debug.Log("1스테이지 클리어x , 튜토 시작");

            tutoPanel.SetActive(true);
            tutoIamge.sprite = tutoImages[0];
            curCount = 0;
        }

    }

    public void nextPage() 
    {
        curCount++;
        if (curCount >= tutoImages.Length) 
        {
            tutoPanel.SetActive(false);
            return;
        }
        tutoIamge.sprite = tutoImages[curCount];

        
    }
    public void prevPage() 
    {   
        if (curCount == 0) 
            return;
        curCount--;
        tutoIamge.sprite = tutoImages[curCount];
    }
}
