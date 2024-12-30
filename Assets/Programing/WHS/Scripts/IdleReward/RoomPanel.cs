using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class RoomPanel : UIBInder
{
    private IdleReward idleReward;
    [SerializeField] private GameObject idleRewardPanel;

    private Coroutine updateIdleTimeCoroutine;

    private void Awake()
    {
        BindAll();

        AddEvent("ClaimButton", EventType.Click, ClaimIdleRewards);
        AddEvent("CheckRewardButton", EventType.Click, ShowIdleRewardPanel);
    }

    private void Start()
    {
        idleReward = GetComponent<IdleReward>();

        idleReward.CalculateIdleReward();
    }

    private void OnEnable()
    {
        /*
        // 1시간이 지나야 수령 버튼 활성화
        GetUI<UnityEngine.UI.Button>("ClaimButton").interactable = idleReward.HasIdleReward();

        if (updateIdleTimeCoroutine == null)
        {
            updateIdleTimeCoroutine = StartCoroutine(UpdateIdleTimeCoroutine());
        }
        */
    }

    // Room 떠날 때 RoomExitTime 서버에 저장
    private void OnDisable()
    {
        idleReward.SaveExitTime();

        if (updateIdleTimeCoroutine != null)
        {
            StopCoroutine(updateIdleTimeCoroutine);
            updateIdleTimeCoroutine = null;
        }
    }

    // 게임 종료 시 RoomExitTime 서버에 저장
    private void OnApplicationQuit()
    {
        idleReward.SaveExitTime();
    }

    public void TESTESTS()
    {
        if (updateIdleTimeCoroutine == null)
        {
            updateIdleTimeCoroutine = StartCoroutine(UpdateIdleTimeCoroutine());
        }
    }

    // 보상 수령
    public void ClaimIdleRewards(PointerEventData eventData)
    {
        if (GetUI<UnityEngine.UI.Button>("ClaimButton").interactable == false)
            return;

        int storedGold = PlayerDataManager.Instance.PlayerData.StoredItems[(int)E_Item.Coin];
        int storedDinoBlood = PlayerDataManager.Instance.PlayerData.StoredItems[(int)E_Item.DinoBlood];
        int storedBoneCrystal = PlayerDataManager.Instance.PlayerData.StoredItems[(int)E_Item.BoneCrystal];

        // 방치형 보상을 플레이어의 아이템에 추가
        PlayerDataManager.Instance.PlayerData.SetItem((int)E_Item.Coin, PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Coin] + storedGold);
        PlayerDataManager.Instance.PlayerData.SetItem((int)E_Item.DinoBlood, PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoBlood] + storedDinoBlood);
        PlayerDataManager.Instance.PlayerData.SetItem((int)E_Item.BoneCrystal, PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.BoneCrystal] + storedBoneCrystal);

        // 방치형 보상 비우기
        PlayerDataManager.Instance.PlayerData.SetStoredItem((int)E_Item.Coin, 0);
        PlayerDataManager.Instance.PlayerData.SetStoredItem((int)E_Item.DinoBlood, 0);
        PlayerDataManager.Instance.PlayerData.SetStoredItem((int)E_Item.BoneCrystal, 0);

        // Firebase에 업데이트된 아이템 정보 저장
        UpdateItemsInDatabase();

        // 방치형보상 수령 후 종료시간 저장
        idleReward.SaveExitTime();

        idleRewardPanel.SetActive(false);
    }

    // 데이터베이스에 아이템 저장
    private void UpdateItemsInDatabase()
    {
        // string userId = BackendManager.Auth.CurrentUser.UserId;
        string userId = "sinEKs9IWRPuWNbboKov1fKgmab2";
        DatabaseReference userRef = BackendManager.Database.RootReference.Child("UserData").Child(userId);

        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            ["_items/0"] = PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Coin],
            ["_items/1"] = PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoBlood],
            ["_items/2"] = PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.BoneCrystal],
            ["_storedItems/0"] = 0,
            ["_storedItems/1"] = 0,
            ["_storedItems/2"] = 0
        };

        userRef.UpdateChildrenAsync(updates).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log($"방치형 아이템 갱신 실패 {task.Exception}");
            }
            if (task.IsCanceled)
            {
                Debug.LogError($"방치형 아이템 갱신 중단됨 {task.Exception}");
            }
            Debug.Log("방치형 보상 수령 성공");
        });
    }

    // 방치시간 타이머 텍스트
    private IEnumerator UpdateIdleTimeCoroutine()
    {
        while (true)
        {
            idleReward.CalculateIdleReward();

            TimeSpan idleTime = idleReward.GetIdleTime();
            GetUI<TextMeshProUGUI>("IdleTimeText").text = $"{idleTime.Hours} : {idleTime.Minutes} : {idleTime.Seconds}";

            yield return new WaitForSeconds(1f);
        }
    }

    // 방치형보상 UI 패널
    private void ShowIdleRewardPanel(PointerEventData eventData)
    {
        idleReward.CalculateIdleReward();

        idleRewardPanel.SetActive(true);

        int goldReward = idleReward.CalculateReward(1, (int)idleReward.GetIdleTime().TotalSeconds);
        int dinoBloodReward = idleReward.CalculateReward(2, (int)idleReward.GetIdleTime().TotalSeconds);
        int boneCrystalReward = idleReward.CalculateReward(3, (int)idleReward.GetIdleTime().TotalSeconds);

        GetUI<TextMeshProUGUI>("GoldRewardText").text = $"Gold: {goldReward}";
        GetUI<TextMeshProUGUI>("DinoBloodRewardText").text = $"Dino Blood: {dinoBloodReward}";
        GetUI<TextMeshProUGUI>("BoneCrystalRewardText").text = $"Bone Crystal: {boneCrystalReward}";

        GetUI<UnityEngine.UI.Button>("ClaimButton").interactable = idleReward.HasIdleReward();
    }
}