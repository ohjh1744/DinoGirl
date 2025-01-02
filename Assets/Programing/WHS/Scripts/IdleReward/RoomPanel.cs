using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoomPanel : UIBInder
{
    private IdleReward idleReward;
    [SerializeField] private GameObject idleRewardPanel;
    [SerializeField] private ItemPanel itemPanel;

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
        GetUI<UnityEngine.UI.Button>("ClaimButton").interactable = idleReward.HasIdleReward();

      
        if (updateIdleTimeCoroutine == null)
        {
            updateIdleTimeCoroutine = StartCoroutine(UpdateIdleTimeCoroutine());
        }
    }
    private void OnDisable()
    {
        if (updateIdleTimeCoroutine != null)
        {
            StopCoroutine(updateIdleTimeCoroutine);
            updateIdleTimeCoroutine = null;
        }
    }

    /*
    public void TESTESTS()
    {
        if (updateIdleTimeCoroutine == null)
        {
            updateIdleTimeCoroutine = StartCoroutine(UpdateIdleTimeCoroutine());
        }
    }
    */

    // 보상 수령
    public void ClaimIdleRewards(PointerEventData eventData)
    {
        if (GetUI<Button>("ClaimButton").interactable == false)
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

        ShowPopup(storedGold, storedDinoBlood, storedBoneCrystal);

        // Firebase에 업데이트된 아이템 정보 저장
        UpdateItemsInDatabase();

        // 방치형보상 수령 후 종료시간 저장
        idleReward.SaveExitTime();

        idleRewardPanel.SetActive(false);

        itemPanel.UpdateItems();
    }

    // 데이터베이스에 아이템 저장
    private void UpdateItemsInDatabase()
    {
        string userId = BackendManager.Auth.CurrentUser.UserId;
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
        DateTime lastTime = DateTime.Now;

        while (true)
        {
            TimeSpan idleTime = idleReward.GetIdleTime();

            GetUI<TextMeshProUGUI>("IdleTimeText").text = $"{idleTime.Hours} : {idleTime.Minutes} : {idleTime.Seconds}";
            GetUI<Button>("ClaimButton").interactable = idleReward.HasIdleReward();

            TimeSpan elapsedTime = DateTime.Now - lastTime;

            if (elapsedTime.TotalSeconds >= 10)
            {
                idleReward.CalculateIdleReward();
                lastTime = DateTime.Now;
            }

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

        GetUI<Button>("ClaimButton").interactable = idleReward.HasIdleReward();
    }

    private void ShowPopup(int gold, int dinoBlood, int boneCrystal)
    {
        GetUI<TextMeshProUGUI>("GoldClaimText").text = $"Gold : {gold}";
        GetUI<TextMeshProUGUI>("DinoBloodClaimText").text = $"Gold : {dinoBlood}";
        GetUI<TextMeshProUGUI>("BoneCrystalClaimText").text = $"Gold : {boneCrystal}";
    }
}