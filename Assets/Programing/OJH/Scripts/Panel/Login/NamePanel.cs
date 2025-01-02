using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NamePanel : UIBInder
{
    [SerializeField] private int _baseUnitNum; //계정 생성후 갖고있는 캐릭터 개수.

    [SerializeField] private int[] _baseUnitIds;// 캐릭터 ID

    private StringBuilder _sb = new StringBuilder();

    [SerializeField] private SceneChanger _sceneChanger;

    [SerializeField] private int _nameLen; // 이름 제한 길이

    private void Awake()
    {
        BindAll();
    }

    private void Start()
    {
        GetUI<Button>("SetNameButton").onClick.AddListener(SetName);
        GetUI<Button>("NameExitButton").onClick.AddListener(ResetInputField);
    }

    private void SetName()
    {
        string nickName = GetUI<TMP_InputField>("NameInputField").text;
        
        // 이름이 없는 경우 팝업창 띄우기
        if(nickName == "" || nickName.Length > _nameLen)
        {
            SetTrueWarningPanel("No Name or Too Long Name");
            ResetInputField();
            return;
        }

        FirebaseUser user = BackendManager.Auth.CurrentUser;
        if (user == null)
        {
            ResetInputField();
            return;
        }

        UserProfile profile = new UserProfile();
        profile.DisplayName = nickName;


        user.UpdateUserProfileAsync(profile).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("UpdateUserProfileAsync was canceled.");
                ResetInputField();
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("UpdateUserProfileAsync encountered an error: " + task.Exception);
                ResetInputField();
                return;
            }


            ResetInputField();
            CreateDataBase();
        });
    }

    private void CreateDataBase()
    {
        // 성공시 비동기씬 진행.
        _sceneChanger.ChangeScene("LobbyOJH");

        DatabaseReference root = BackendManager.Database.RootReference.Child("UserData").Child(BackendManager.Auth.CurrentUser.UserId);
        PlayerDataManager.Instance.PlayerData.PlayerName = BackendManager.Auth.CurrentUser.DisplayName;
        PlayerDataManager.Instance.PlayerData.RoomExitTime = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
        PlayerDataManager.Instance.PlayerData.LastResetFollowTime = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
        

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


        // 정상적으로 함수 동작시 씬 전환.
        _sceneChanger.CanChangeSceen = true;
    }


    private void SetTrueWarningPanel(string textName)
    {
        GetUI("NameWarningPanel").SetActive(true);
        _sb.Clear();
        _sb.Append(textName);
        GetUI<TextMeshProUGUI>("NameWarningText").SetText(_sb);
    }

    private void ResetInputField()
    {
        GetUI<TMP_InputField>("NameInputField").text = "";
    }


}
