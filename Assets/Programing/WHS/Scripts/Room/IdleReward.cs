using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IdleReward : MonoBehaviour
{
    private Dictionary<int, Dictionary<string, string>> _housingData;

    /*
    private void Start()
    {
        StartCoroutine(WaitForPlayerData());
    }

    private IEnumerator WaitForPlayerData()
    {
        // PlayerDataManager가 초기화되고 PlayerData가 로드될 때까지 대기
        yield return new WaitUntil(() => PlayerDataManager.Instance.PlayerData.UnitDatas.Count > 0);

        housingData = CsvDataManager.Instance.DataLists[(int)E_CsvData.Housing];
        Debug.Log($"{housingData.Count}");
        foreach (var key in housingData.Keys)
        {
            Debug.Log($"Key: {key}, Value: {housingData[key]["PerHour"]}, {housingData[key]["0MaxStorage"]}");
        }
    }
    */

    private void Awake()
    {
        _housingData = CsvDataManager.Instance.DataLists[(int)E_CsvData.Housing];

        Debug.Log($"{_housingData.Count}");
        foreach (var key in _housingData.Keys)
        {
            Debug.Log($"Key: {key}, Value: {_housingData[key]["PerHour"]}, {_housingData[key]["0MaxStorage"]}, " +
                $"{_housingData[key]["1MaxStorage"]}, {_housingData[key]["2MaxStorage"]}, ");
        }
    }


    // 방치시간 계산하기
    public void CalculateIdleReward()
    {
        string exitTimeStr = PlayerDataManager.Instance.PlayerData.RoomExitTime;
        DateTime exitTime = DateTime.ParseExact(exitTimeStr, "yyyyMMdd_HHmmss_fff", null);
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

        // 데이터베이스에 방치형 아이템 저장
        UpdateStoredItemsInDatabase();
    }

    // 아이템 계산
    public int CalculateReward(int housingId, int seconds)
    {
        // 최대 누적 시간(24시간 86400초)
        int maxIdleTime = Mathf.Min(seconds, 86400);
        float idleHours = maxIdleTime / 3600f;

        if (_housingData.TryGetValue(housingId, out Dictionary<string, string> data))
        {
            // 스테이지 진행도에 따른 시간당 보상
            int rewardPerHour = GetRewardPerHour(housingId);
            int reward = Mathf.FloorToInt(rewardPerHour * idleHours);

            return reward;
        }
        return 0;
    }

    // 데이터베이스에 방치형 아이템 저장
    private void UpdateStoredItemsInDatabase()
    {
        string userId = BackendManager.Auth.CurrentUser.UserId;
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
        string curTime = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");

        PlayerDataManager.Instance.PlayerData.RoomExitTime = curTime;

        string userId = BackendManager.Auth.CurrentUser.UserId;
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

    // 수령할 보상이 있을 때 (idleTime 1분부터) 버튼 활성화 
    public bool HasIdleReward()
    {
        string exitTimeStr = PlayerDataManager.Instance.PlayerData.RoomExitTime;
        DateTime exitTime = DateTime.ParseExact(exitTimeStr, "yyyyMMdd_HHmmss_fff", null);
        TimeSpan idleTime = DateTime.Now - exitTime;

        int idleSeconds = (int)idleTime.TotalSeconds;
        return idleSeconds >= 60;
    }

    // 방치한 시간
    public TimeSpan GetIdleTime()
    {
        string exitTimeStr = PlayerDataManager.Instance.PlayerData.RoomExitTime;
        DateTime exitTime = DateTime.ParseExact(exitTimeStr, "yyyyMMdd_HHmmss_fff", null);

        if (exitTime > DateTime.Now)
        {
            Debug.LogWarning("exitTime이 현재보다 큼");
            exitTime = DateTime.Now;
        }

        return DateTime.Now - exitTime;
    }

    // 스테이지 진행에 따른 보상
    private int GetRewardPerHour(int housingId)
    {
        if (_housingData.TryGetValue(housingId, out Dictionary<string, string> data))
        {
            int baseReward = int.Parse(data["PerHour"]);

            // 클리어한 스테이지의 수
            int clearedStages = PlayerDataManager.Instance.PlayerData.IsStageClear.Count(x => x);

            if (clearedStages >= 21 && data.TryGetValue("2MaxStorage", out string storage2))
            {
                Debug.Log("storage2");
                return int.Parse(storage2);
            }
            else if (clearedStages >= 14 && data.TryGetValue("1MaxStorage", out string storage1))
            {
                Debug.Log("storage1");
                return int.Parse(storage1);
            }
            else if (clearedStages >= 7 && data.TryGetValue("0MaxStorage", out string storage0))
            {
                Debug.Log("storage0");
                return int.Parse(storage0);
            }
            else
            {
                Debug.Log("basereward");
                return baseReward;
            }
        }
        return 0;
    }
}
