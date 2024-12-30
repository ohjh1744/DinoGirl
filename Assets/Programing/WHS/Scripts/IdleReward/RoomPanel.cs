using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RoomPanel : UIBInder
{
    private IdleReward idleReward;

    private void Awake()
    {
        BindAll();

        AddEvent("ClaimButton", EventType.Click, ClaimIdleRewards);
    }

    private void Start()
    {
        idleReward = GetComponent<IdleReward>();

        UpdateClaimButtonState();
    }

    private void OnEnable()
    {
        // 1시간이 지나야 수령 버튼 활성화
        // GetUI<UnityEngine.UI.Button>("ClaimButton").interactable = idleReward.HasIdleReward();
    }

    // Room 떠날 때 RoomExitTime 서버에 저장
    private void OnDisable()
    {
        idleReward.SaveExitTime();
    }

    public void UpdateClaimButtonState()
    {
        // ClaimButton의 상호작용 가능 여부를 idleReward의 결과에 따라 설정
        GetUI<UnityEngine.UI.Button>("ClaimButton").interactable = idleReward.HasIdleReward();
    }

    private void OnApplicationQuit()
    {
        idleReward.SaveExitTime();
    }

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

        UpdateClaimButtonState();
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
}