using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataWriter : MonoBehaviour
{
    [SerializeField] private int _baseUnitNum; //계정 생성후 갖고있는 캐릭터 개수.


    [SerializeField] private int[] _baseUnitIds;// 캐릭터 ID

    [SerializeField] private string _uID;

    [SerializeField] private string _name;

    [ContextMenu("WriteTest")]
    public void CreateDataBase()
    {

        DatabaseReference root = BackendManager.Database.RootReference.Child("UserData").Child(_uID);
        PlayerDataManager.Instance.PlayerData.PlayerName = _name;
        PlayerDataManager.Instance.PlayerData.RoomExitTime = DateTime.Now.ToString("o");
        PlayerDataManager.Instance.PlayerData.LastResetFollowTime = DateTime.Now.ToString("o");
        PlayerDataManager.Instance.PlayerData.Gift.Add("Player", 0); // Dictionary를 Db에 저장하기 위해 임시 초기값 설정

        for (int i = 0; i < _baseUnitNum; i++)
        {
            PlayerUnitData unitData = new PlayerUnitData();

            unitData.UnitId = _baseUnitIds[i];
            unitData.UnitLevel = 1;

            PlayerDataManager.Instance.PlayerData.UnitDatas.Add(unitData);
        }

        //Json으로 변환후 저장.
        string json = JsonUtility.ToJson(PlayerDataManager.Instance.PlayerData);
        root.SetRawJsonValueAsync(json);
        //Dictionary는 List처럼 json 불가?
        root.Child("_gift").SetValueAsync(PlayerDataManager.Instance.PlayerData.Gift);
    }


}
