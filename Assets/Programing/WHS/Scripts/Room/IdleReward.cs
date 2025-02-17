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

    // ��ġ�ð� ����ϱ�
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

        // �����ͺ��̽��� ��ġ�� ������ ����
        UpdateStoredItemsInDatabase();
    }

    // ������ ���
    public int CalculateReward(int housingId, int seconds)
    {
        // �ִ� ���� �ð�(12�ð� 43200��)
        int maxIdleTime = Mathf.Min(seconds, 43200);
        int idleTime = maxIdleTime / 60;

        if (_housingData.TryGetValue(housingId, out Dictionary<string, string> data))
        {
            // �������� ���൵�� ���� �ð��� ����
            float rewardPerMinute = GetRewardPerHour(housingId) / 60f;
            int reward = Mathf.FloorToInt(rewardPerMinute * idleTime);

            return reward;
        }
        return 0;
    }

    // �����ͺ��̽��� ��ġ�� ������ ����
    private void UpdateStoredItemsInDatabase()
    {
        string userId = BackendManager.Instance.Auth.CurrentUser.UserId;
        DatabaseReference userRef = BackendManager.Instance.Database.RootReference.Child("UserData").Child(userId).Child("_storedItems");

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
                Debug.Log($"��ġ�� ������ ���� ���� {task.Exception}");
            }
            if (task.IsCanceled)
            {
                Debug.LogError($"��ġ�� ������ ���� �ߴܵ� {task.Exception}");
            }
            Debug.Log("��ġ�� ���� �����");
        });
    }

    // ���� �ð� ����
    public void SaveExitTime()
    {
        string curTime = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");

        PlayerDataManager.Instance.PlayerData.RoomExitTime = curTime;

        string userId = BackendManager.Instance.Auth.CurrentUser.UserId;
        DatabaseReference userRef = BackendManager.Instance.Database.RootReference.Child("UserData").Child(userId);

        userRef.Child("_roomExitTime").SetValueAsync(curTime).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError($"exittime ���� ���� {task.Exception}");
            }
            if (task.IsCanceled)
            {
                Debug.LogError($"exittime ���� �ߴܵ� {task.Exception}");
            }

            Debug.Log($"exittime ����� {curTime}");
        });
    }

    // ������ ������ ���� �� (idleTime 1�к���) ��ư Ȱ��ȭ 
    public bool HasIdleReward()
    {
        string exitTimeStr = PlayerDataManager.Instance.PlayerData.RoomExitTime;
        DateTime exitTime = DateTime.ParseExact(exitTimeStr, "yyyyMMdd_HHmmss_fff", null);
        TimeSpan idleTime = DateTime.Now - exitTime;

        int idleSeconds = (int)idleTime.TotalSeconds;
        return idleSeconds >= 60;
    }

    // ��ġ�� �ð�
    public TimeSpan GetIdleTime()
    {
        string exitTimeStr = PlayerDataManager.Instance.PlayerData.RoomExitTime;
        DateTime exitTime = DateTime.ParseExact(exitTimeStr, "yyyyMMdd_HHmmss_fff", null);

        if (exitTime > DateTime.Now)
        {
            Debug.LogWarning("exitTime�� ���纸�� ŭ");
            exitTime = DateTime.Now;
        }

        return DateTime.Now - exitTime;
    }

    // �������� ���࿡ ���� ����
    private int GetRewardPerHour(int housingId)
    {
        if (_housingData.TryGetValue(housingId, out Dictionary<string, string> data))
        {
            int baseReward = int.Parse(data["PerHour"]);

            // Ŭ������ ���������� ��
            int clearedStages = PlayerDataManager.Instance.PlayerData.IsStageClear.Count(x => x);

            // 21��������(3é��) �� Ŭ���� �� 
            if (clearedStages >= 21 && data.TryGetValue("2MaxStorage", out string storage2))
            {
                Debug.Log("storage2");
                return int.Parse(storage2);
            }
            // 14��������(2é��) �� Ŭ���� ��
            else if (clearedStages >= 14 && data.TryGetValue("1MaxStorage", out string storage1))
            {
                Debug.Log("storage1");
                return int.Parse(storage1);
            }
            // 7��������(1é��) �� Ŭ���� ��
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
