using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataWriter : MonoBehaviour
{

    [SerializeField] private int[] _baseUnitIds;// 캐릭터 ID

    [SerializeField] private string _uID;

    [SerializeField] private string _name;

    [SerializeField] private int _mainUnitID;

    [ContextMenu("WriteTest")]
    public void CreateDataBase()
    {

        DatabaseReference root = BackendManager.Instance.Database.RootReference.Child("UserData").Child(_uID);
        PlayerDataManager.Instance.PlayerData.PlayerName = _name;
        PlayerDataManager.Instance.PlayerData.MainUnitID = _mainUnitID;
        PlayerDataManager.Instance.PlayerData.RoomExitTime = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
        PlayerDataManager.Instance.PlayerData.LastResetAddFriendTime = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
        //PlayerDataManager.Instance.PlayerData.Gift["ruru"] = 1000;

        for (int i = 0; i < _baseUnitIds.Length; i++)
        {
            PlayerUnitData unitData = new PlayerUnitData();

            unitData.UnitId = _baseUnitIds[i];
            unitData.UnitLevel = 1;

            PlayerDataManager.Instance.PlayerData.UnitDatas.Add(unitData);
        }

        //Json으로 변환후 저장.
        string json = JsonUtility.ToJson(PlayerDataManager.Instance.PlayerData);
        root.SetRawJsonValueAsync(json);

        //Dictionary는 JsonUtility.TOJson이 불가능해서 필요시 따로 해줘야함.
        //root.Child("_gift").SetValueAsync(PlayerDataManager.Instance.PlayerData.Gift);
    }


}
