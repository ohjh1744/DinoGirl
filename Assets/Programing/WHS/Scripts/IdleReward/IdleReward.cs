using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleReward : MonoBehaviour
{
    private Dictionary<int, Dictionary<string, string>> housingData;

    private void Start()
    {
        StartCoroutine(WaitForPlayerData());
    }

    private IEnumerator WaitForPlayerData()
    {
        // PlayerDataManager가 초기화되고 PlayerData가 로드될 때까지 대기
        yield return new WaitUntil(() =>
            PlayerDataManager.Instance != null &&
            PlayerDataManager.Instance.PlayerData != null &&
            PlayerDataManager.Instance.PlayerData.UnitDatas != null &&
            PlayerDataManager.Instance.PlayerData.UnitDatas.Count > 0);

        housingData = CsvDataManager.Instance.DataLists[(int)E_CsvData.Housing];
        Debug.Log($"{housingData.Count}");
        foreach (var key in housingData.Keys)
        {
            Debug.Log($"Key: {key}, Value: {housingData[key]["PerHour"]}, {housingData[key]["0MaxStorage"]}");
        }
    }
    /*
    private void Awake()
    {
        housingData = CsvDataManager.Instance.DataLists[(int)E_CsvData.Housing];

        Debug.Log($"{housingData.Count}");
        foreach (var key in housingData.Keys)
        {
            Debug.Log($"Key: {key}, Value: {housingData[key]["PerHour"]}, {housingData[key]["0MaxStorage"]}");
        }
    }
    */

    // 시간 계산하기
    public void CalculateIdleReward()
    {
        string exitTimeStr = PlayerDataManager.Instance.PlayerData.RoomExitTime;
        DateTime exitTime = DateTime.Parse(exitTimeStr);
        TimeSpan idleTime = DateTime.Now - exitTime;

        int idleSeconds = (int)idleTime.TotalSeconds;
        Debug.Log(idleSeconds);

        int goldReward = CalculateReward(1, idleSeconds);
        int dinoBloodReward = CalculateReward(2, idleSeconds);
        int boneCrystalReward = CalculateReward(3, idleSeconds);

        PlayerDataManager.Instance.PlayerData.SetStoredItem((int)E_Item.Coin, goldReward);
        PlayerDataManager.Instance.PlayerData.SetStoredItem((int)E_Item.DinoBlood, dinoBloodReward);
        PlayerDataManager.Instance.PlayerData.SetStoredItem((int)E_Item.BoneCrystal, boneCrystalReward);

        Debug.Log($"Gold: {goldReward} DinoBlood: {dinoBloodReward} BoneCrystal: {boneCrystalReward}");

        UpdateStoredItemsInDatabase();
    }

    // 아이템 계산
    private int CalculateReward(int housingId, int seconds)
    {
        if (housingData.TryGetValue(housingId, out Dictionary<string, string> data))
        {
            float rewardPerHour = float.Parse(data["PerHour"]);
            int reward = Mathf.FloorToInt(rewardPerHour * seconds / 1f);
            return reward;
        }
        return 0;
    }

    // 데이터베이스에 방치형 아이템 저장
    private void UpdateStoredItemsInDatabase()
    {
        // string userId = BackendManager.Auth.CurrentUser.UserId;
        string userId = "sinEKs9IWRPuWNbboKov1fKgmab2";
        DatabaseReference userRef = BackendManager.Database.RootReference.Child("UserData").Child(userId).Child("_storedItems");

        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            ["0"] = PlayerDataManager.Instance.PlayerData.StoredItems[(int)E_Item.Coin],
            ["1"] = PlayerDataManager.Instance.PlayerData.StoredItems[(int)E_Item.DinoBlood],
            ["2"] = PlayerDataManager.Instance.PlayerData.StoredItems[(int)E_Item.BoneCrystal],
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
            Debug.Log("방치형 보상 저장됨");
        });
    }

    // 종료 시간 저장
    public void SaveExitTime()
    {
        string curTime = DateTime.Now.ToString();

        PlayerDataManager.Instance.PlayerData.RoomExitTime = curTime;

        // string userId = BackendManager.Auth.CurrentUser.UserId;
        string userId = "sinEKs9IWRPuWNbboKov1fKgmab2";
        DatabaseReference userRef = BackendManager.Database.RootReference.Child("UserData").Child(userId);

        userRef.Child("_roomExitTime").SetValueAsync(curTime).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError($"exittime 저장 실패 {task.Exception}");
            }
            if (task.IsCanceled)
            {
                Debug.LogError($"exittime 저장 중단됨 {task.Exception}");
            }

            Debug.Log($"exittime 저장됨 {curTime}");
        });
    }

    public bool HasIdleReward()
    {
        string exitTimeStr = PlayerDataManager.Instance.PlayerData.RoomExitTime;
        DateTime exitTime = DateTime.Parse(exitTimeStr);
        TimeSpan idleTime = DateTime.Now - exitTime;

        int idleSeconds = (int)idleTime.TotalSeconds;
        return idleSeconds >= 3600;
    }
}
