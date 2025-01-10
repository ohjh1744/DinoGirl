using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using Firebase.Database;
using Firebase.Extensions;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
//using static UnityEditor.Progress;

public class BattleSceneUi : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private GameObject winUi;
    [SerializeField] private GameObject loseUi;
    [SerializeField] private GameObject RewardUi;

    [SerializeField] Button Lobbybtn;
    [SerializeField] Button Stagebtn;


    [SerializeField] private float time;
    [SerializeField] private float curTime;

    private int minute;
    private int second;

    private void OnEnable()
    {
        StartCoroutine(ableDelaying());

    }
    IEnumerator ableDelaying() 
    {   
        yield return new WaitForSeconds(0.3f);
        time = BattleSceneManager.Instance._timeLimit;

        Spawner.OnSpawnCompleted += startTimerTriger;
    }
    private void OnDestroy()
    {
        StopAllCoroutines();

        for (int i = 0; i < BattleSceneManager.Instance.myUnits.Count; i++)
        {
            BattleSceneManager.Instance.myUnits[i].GetComponent<UnitModel>().OnDeath -= WinorLose;
        }
        for (int i = 0; i < BattleSceneManager.Instance.enemyUnits.Count; i++)
        {
            BattleSceneManager.Instance.enemyUnits[i].GetComponent<UnitModel>().OnDeath -= WinorLose;
        }
        Spawner.OnSpawnCompleted -= startTimerTriger;
        StopCoroutine("startTimer");
        StopCoroutine("Subscriber");
    }
    IEnumerator Subscriber()
    {
        Debug.Log("이벤트 구독");

        yield return new WaitForSeconds(2f);
        for (int i = 0; i < BattleSceneManager.Instance.myUnits.Count; i++)
        {
            BattleSceneManager.Instance.myUnits[i].GetComponent<UnitModel>().OnDeath += WinorLose;
        }
        for (int i = 0; i < BattleSceneManager.Instance.enemyUnits.Count; i++)
        {
            BattleSceneManager.Instance.enemyUnits[i].GetComponent<UnitModel>().OnDeath += WinorLose;
        }

    }
    Coroutine coroutine;
    public void startTimerTriger()
    {
        coroutine =  StartCoroutine(startTimer());
        coroutine =  StartCoroutine(Subscriber());
    }
    IEnumerator startTimer()
    {
        curTime = time;
        while (curTime > 0)
        {
            curTime -= Time.deltaTime;
            minute = (int)curTime / 60;
            second = (int)curTime % 60;
            timerText.text = minute.ToString("00") + ":" + second.ToString("00");
            yield return null;

            if (curTime <= 0)
            {
                Debug.Log("시간 종료");
                curTime = 0;
                openResultPanel();
                yield break;
            }
        }
    }
    public void WinorLose()
    {
        if (BattleSceneManager.Instance.myUnits.All(item => item.UnitModel.Hp <= 0)) // 전부 피가 0이면 
        {
            // 패배
            Debug.Log("패배");
            BattleSceneManager.Instance.curBattleState = BattleSceneManager.BattleState.Lose;

            openResultPanel();


        }
        else if (BattleSceneManager.Instance.enemyUnits.All(item => item.UnitModel.Hp <= 0))
        {
            // 승리
            Debug.Log("승리");
            BattleSceneManager.Instance.curBattleState = BattleSceneManager.BattleState.Win;
            openResultPanel();
        }
        else if (curTime <= 0)
        {
            // 시간제한 패배
            Debug.Log("시간제한 패배");
            BattleSceneManager.Instance.curBattleState = BattleSceneManager.BattleState.Lose;
            openResultPanel();
        }
        else
        {
            Debug.Log("결과조건 미 도달");
        }

    }
    public void openResultPanel()
    {   
        for (int i = 0; i < BattleSceneManager.Instance.myUnits.Count; i++)
        {
            BattleSceneManager.Instance.myUnits[i].GetComponent<UnitModel>().OnDeath -= WinorLose;
        }
        for (int i = 0; i < BattleSceneManager.Instance.enemyUnits.Count; i++)
        {
            BattleSceneManager.Instance.enemyUnits[i].GetComponent<UnitModel>().OnDeath -= WinorLose;
        }
        Spawner.OnSpawnCompleted -= startTimerTriger;

        StopCoroutine("startTimer");
        StopCoroutine("Subscriber");
        Time.timeScale = 0;
        resultPanel.SetActive(true);
        if (BattleSceneManager.Instance.curBattleState == BattleSceneManager.BattleState.Win)
        {
            winUi.SetActive(true);
            loseUi.SetActive(false);
            if (PlayerDataManager.Instance.PlayerData.IsStageClear[BattleSceneManager.Instance.curStageNum] == false)
            {
                UpdateItems();
                updateClear();
            } // 승리시만 업데이트 하면 됨 , 최초클리어만 보상
        }
        else if (BattleSceneManager.Instance.curBattleState == BattleSceneManager.BattleState.Lose)
        {
            loseUi.SetActive(true);
            winUi.SetActive(false);
        }

    }
    private void updateClear()
    {
        string userId = BackendManager.Auth.CurrentUser.UserId;
        DatabaseReference stageRef = BackendManager.Database.RootReference.Child("UserData").Child(userId);
        Debug.Log(BattleSceneManager.Instance.curStageNum);
        PlayerDataManager.Instance.PlayerData.IsStageClear[BattleSceneManager.Instance.curStageNum] = true;
        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            ["_isStageClear/" + BattleSceneManager.Instance.curStageNum.ToString()] = true
        };

        stageRef.UpdateChildrenAsync(updates).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError($"클리어 업데이트 실패 {task.Exception}");
            }
            if (task.IsCanceled)
            {
                Debug.Log($"클리어 업데이트 중단됨 {task.Exception}");
            }

            Debug.Log("클리어  갱신");
        });
    }
    private void UpdateItems()
    {   // Coin , DinoBlood ,BoneCrystal, DinoStone, Stone
        string userId = BackendManager.Auth.CurrentUser.UserId;
        DatabaseReference userRef = BackendManager.Database.RootReference.Child("UserData").Child(userId);


        foreach (int i in BattleSceneManager.Instance.curItemValues.Keys)
        {
            Debug.Log($"{i}{BattleSceneManager.Instance.curItemValues[i]}");
            switch (i)
            {
                case 500:
                    PlayerDataManager.Instance.PlayerData.SetItem((int)E_Item.Coin, PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Coin] + BattleSceneManager.Instance.curItemValues[i]);
                    break;
                case 501:
                    PlayerDataManager.Instance.PlayerData.SetItem((int)E_Item.DinoBlood, PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoBlood] + BattleSceneManager.Instance.curItemValues[i]);
                    break;
                case 502:
                    PlayerDataManager.Instance.PlayerData.SetItem((int)E_Item.BoneCrystal, PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.BoneCrystal] + BattleSceneManager.Instance.curItemValues[i]);
                    break;
                case 503:
                    PlayerDataManager.Instance.PlayerData.SetItem((int)E_Item.DinoStone, PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoStone] + BattleSceneManager.Instance.curItemValues[i]);
                    break;
                case 504:
                    PlayerDataManager.Instance.PlayerData.SetItem((int)E_Item.Stone, PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Stone] + BattleSceneManager.Instance.curItemValues[i]);
                    break;

            }
        }
        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            ["_items/0"] = PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Coin],
            ["_items/1"] = PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoBlood],
            ["_items/2"] = PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.BoneCrystal],
            ["_items/3"] = PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoStone],
            ["_items/4"] = PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Stone]
        };

        userRef.UpdateChildrenAsync(updates).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError($"아이템 업데이트 실패 {task.Exception}");
            }
            if (task.IsCanceled)
            {
                Debug.Log($"아이템 업데이트 중단됨 {task.Exception}");
            }

            Debug.Log("획득한 아이템 갱신");
        });
    }


    public void goLobby()
    {
        BattleSceneManager.Instance.GoLobby();
    }
    public void goChapter()
    {
        BattleSceneManager.Instance.GoChapter();
    }
    public void goLobby2()
    {
        Time.timeScale = 0;
        for (int i = 0; i < BattleSceneManager.Instance.myUnits.Count; i++)
        {
            BattleSceneManager.Instance.myUnits[i].GetComponent<UnitModel>().OnDeath -= WinorLose;
        }
        for (int i = 0; i < BattleSceneManager.Instance.enemyUnits.Count; i++)
        {
            BattleSceneManager.Instance.enemyUnits[i].GetComponent<UnitModel>().OnDeath -= WinorLose;
        }
        Spawner.OnSpawnCompleted -= startTimerTriger;
        Destroy(gameObject);
        StopCoroutine("startTimer");
        StopCoroutine("Subscriber");
        //Time.timeScale = 0;
        BattleSceneManager.Instance.GoLobby();
    }

}
