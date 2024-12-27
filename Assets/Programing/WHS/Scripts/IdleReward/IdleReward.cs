using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleReward : MonoBehaviour
{
    // TimeSpan 방치된 시간 = 현재 시간 - 종료 시간
    // TotalSeconds로 초단위로 변환
    // CSV Housing 시트에서 시간당 HousingID 1골드 2다이노블러드 3본크리스탈에 따라 보상

    private Dictionary<int, Dictionary<string, string>> housingData;

    private void Awake()
    {
        housingData = CsvDataManager.Instance.DataLists[(int)E_CsvData.Housing];
        if (housingData != null)
        {
            Debug.Log("housingData LOADED");
        }
    }

    // 시간 계산하기
    public void CalculateIdleReward()
    {
        string exitTimeStr = PlayerDataManager.Instance.PlayerData.ExitTime;
        DateTime exitTime = DateTime.Parse(exitTimeStr);
        TimeSpan idleTime = DateTime.Now - exitTime;

        int idleSeconds = (int)idleTime.TotalSeconds;

        int goldReward = CalculateReward(1, idleSeconds);
        int dinoBloodReward = CalculateReward(2, idleSeconds);
        int boneCrystalReward = CalculateReward(3, idleSeconds);

        PlayerDataManager.Instance.PlayerData.SetStoredItem((int)E_Item.Coin, goldReward);
        PlayerDataManager.Instance.PlayerData.SetStoredItem((int)E_Item.DinoBlood, dinoBloodReward);
        PlayerDataManager.Instance.PlayerData.SetStoredItem((int)E_Item.BoneCrystal, boneCrystalReward);

        Debug.Log($"Gold: {goldReward} DinoBlood: {dinoBloodReward} BoneCrystal: {boneCrystalReward}");
    }

    // 아이템 계산
    private int CalculateReward(int housingId, int seconds)
    {
        if (housingData.TryGetValue(housingId, out Dictionary<string, string> data))
        {
            float rewardPerHour = float.Parse(data["PerHour"]);
            return Mathf.FloorToInt(rewardPerHour * seconds / 3600f);
        }

        return 0;
    }

    // 종료 시간 저장
    public void SaveExitTime()
    {
        string curTime = DateTime.Now.ToString();

        PlayerDataManager.Instance.PlayerData.ExitTime = curTime;

        // string userId = BackendManager.Auth.CurrentUser.UserId;
        string userId = "poZb90DRTiczkoC5TpHOpaJ5AXR2";
        DatabaseReference userRef = BackendManager.Database.RootReference.Child("UserData").Child(userId);

        userRef.Child("_exitTime").SetValueAsync(curTime).ContinueWithOnMainThread(task =>
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
}
