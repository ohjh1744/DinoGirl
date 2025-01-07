using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Backto : MonoBehaviour
{
    public void BacktoLobby() 
    {
        SceneManager.LoadScene("ChapterSelect_LJH");
        Destroy(BattleSceneManager.Instance.gameObject);
    }
}
