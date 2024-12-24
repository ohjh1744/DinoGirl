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

    private void Awake()
    {
        BindAll();
    }

    private void Start()
    {
        GetUI<Button>("SetNameButton").onClick.AddListener(SetName);
    }

    private void SetName()
    {
        string nickName = GetUI<TMP_InputField>("NameInputField").text;
        
        // 이름이 없는 경우 팝업창 띄우기
        if(nickName == "")
        {
            SetTrueWarningPanel("Please Input Name");
            return;
        }

        FirebaseUser user = BackendManager.Auth.CurrentUser;
        if (user == null)
            return;

        UserProfile profile = new UserProfile();
        profile.DisplayName = nickName;


        user.UpdateUserProfileAsync(profile).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("UpdateUserProfileAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("UpdateUserProfileAsync encountered an error: " + task.Exception);
                return;
            }

            CreateDataBase();
        });
    }

    private void CreateDataBase()
    {
        // 성공시 비동기씬 진행.
        _sceneChanger.ChangeScene("LobbyOJH");

        DatabaseReference root = BackendManager.Database.RootReference.Child("UserData").Child(BackendManager.Auth.CurrentUser.UserId);
        PlayerDataManager.Instance.PlayerData.PlayerName = BackendManager.Auth.CurrentUser.DisplayName;
        PlayerDataManager.Instance.PlayerData.PlayerId = BackendManager.Auth.CurrentUser.UserId;
        PlayerDataManager.Instance.PlayerData.ExitTime = DateTime.Now.ToString("o");

        List<Dictionary<string, string>> stageDic = CsvDataManager.Instance.DataLists[(int)E_CsvData.Character];

        for (int i = 0; i < _baseUnitNum; i++)
        {
            PlayerUnitData unitData = new PlayerUnitData();

            foreach (Dictionary<string, string> field in stageDic)
            {
                if (int.Parse(field["CharID"]) == _baseUnitIds[i])
                {
                    unitData.Name = field["Name"];
                    unitData.UnitLevel = 1;
                    unitData.Type = field["Class"];
                    unitData.ElementName = field["ElementName"];
                    unitData.Hp = TypeCastManager.Instance.TryParseInt(field["BaseHp"]);
                    unitData.Atk = TypeCastManager.Instance.TryParseInt(field["BaseATK"]);
                    unitData.Def = TypeCastManager.Instance.TryParseInt(field["BaseDef"]);
                    unitData.Grid = field["Grid"];
                    unitData.StatId = TypeCastManager.Instance.TryParseInt(field["StatID"]);
                    unitData.PercentIncrease = TypeCastManager.Instance.TryParseInt(field["PercentIncrease"]);

                    PlayerDataManager.Instance.PlayerData.UnitDatas.Add(unitData);
                    break;
                }
            }
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
        GetUI<TextMeshProUGUI>("WarningText").SetText(_sb);
    }


}
