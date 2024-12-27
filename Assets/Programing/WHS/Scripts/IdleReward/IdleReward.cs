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
    }

    public void CalculateIdleReward()
    {
        string exitTimeStr = PlayerDataManager.Instance.PlayerData.ExitTime;

        DateTime exitTime = DateTime.Parse(exitTimeStr);
        TimeSpan idleTime = DateTime.Now - exitTime;

        int idleSeconds = (int)idleTime.TotalSeconds; // 방치된 시간 초
    }
}
