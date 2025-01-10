using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Backto : MonoBehaviour
{
    public void BacktoChapter() 
    {
        SceneManager.LoadScene("ChapterSelect_LJH");
        Destroy(BattleSceneManager.Instance.gameObject);
    }
    public void BacktoLobby()
    {   
        
        SceneManager.LoadScene("Lobby_OJH");
        Destroy(BattleSceneManager.Instance.gameObject);
    }


}
