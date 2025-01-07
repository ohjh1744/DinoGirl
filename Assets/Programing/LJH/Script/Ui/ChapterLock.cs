using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChapterLock : MonoBehaviour
{
    [SerializeField] Button[] chapterButtons;
    [SerializeField] SceneChanger _sceneChanger;


    private void OnEnable()
    {
        if (PlayerDataManager.Instance.PlayerData.IsStageClear[6] == false)
        {
            chapterButtons[0].interactable = true;
            chapterButtons[1].interactable = false;
            chapterButtons[2].interactable = false;
        }
        else if (PlayerDataManager.Instance.PlayerData.IsStageClear[13] == false)
        {
            chapterButtons[0].interactable = true;
            chapterButtons[1].interactable = true;
            chapterButtons[2].interactable = false;
        }
    }
    public void GoLobby()
    {
        _sceneChanger.CanChangeSceen = true;
        _sceneChanger.ChangeScene("Lobby_OJH");
    }


    // 각각 스테이지로
    public void GoZero()
    {
        _sceneChanger.CanChangeSceen = true;
        _sceneChanger.ChangeScene("");
    }
    public void GoFirst()
    {
        _sceneChanger.CanChangeSceen = true;
        _sceneChanger.ChangeScene("");
    }
    public void GoSecond()
    {
        _sceneChanger.CanChangeSceen = true;
        _sceneChanger.ChangeScene("");
    }
}
