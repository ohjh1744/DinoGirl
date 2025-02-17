using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NamePanel : UIBInder
{

    [SerializeField] private int[] _baseUnitIds;// ĳ���� ID

    private StringBuilder _sb = new StringBuilder();

    [SerializeField] private SceneChanger _sceneChanger;

    [SerializeField] private int _nameLen; // �̸� ���� ����

    //ButtonSound
    [SerializeField] private AudioClip _buttonClip;

    private void Awake()
    {
        BindAll();
    }

    private void Start()
    {
        GetUI<Button>("SetNameButton").onClick.AddListener(SetName);
        GetUI<Button>("NameExitButton").onClick.AddListener(ResetInputField);

        GetUI<Button>("NameExitButton").onClick.AddListener(() => SoundManager.Instance.PlaySFX(_buttonClip));
        GetUI<Button>("SetNameButton").onClick.AddListener(() => SoundManager.Instance.PlaySFX(_buttonClip));
        GetUI<Button>("NameWarningExitButton").onClick.AddListener(() => SoundManager.Instance.PlaySFX(_buttonClip));
    }

    private void SetName()
    {
        string nickName = GetUI<TMP_InputField>("NameInputField").text;
        
        // �̸��� ���� ��� �˾�â ����
        if(nickName == "" || nickName.Length > _nameLen)
        {
            SetTrueWarningPanel($"�̸��� �����̰ų� {_nameLen}�ڸ� �ѽ��ϴ�.");
            ResetInputField();
            return;
        }

        FirebaseUser user = BackendManager.Instance.Auth.CurrentUser;
        if (user == null)
        {
            ResetInputField();
            return;
        }

        UserProfile profile = new UserProfile();
        profile.DisplayName = nickName;
        string name = nickName;

        // ���� user���� ��ɵ��� ����Ͽ��� ��α����� ���ϸ� Update�� �ȵǴ°ŷ� �����.
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
            CreateDataBase(nickName);
        });
    }

    private void CreateDataBase(string name)
    {
        // ������ �񵿱�� ����.
        _sceneChanger.ChangeScene("Lobby_OJH");

        DatabaseReference root = BackendManager.Instance.Database.RootReference.Child("UserData").Child(BackendManager.Instance.Auth.CurrentUser.UserId);
        PlayerDataManager.Instance.PlayerData.PlayerName = name;
        PlayerDataManager.Instance.PlayerData.RoomExitTime = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
        PlayerDataManager.Instance.PlayerData.LastResetAddFriendTime = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
        

        for (int i = 0; i < _baseUnitIds.Length; i++)
        {
            PlayerUnitData unitData = new PlayerUnitData();

            unitData.UnitId = _baseUnitIds[i];
            unitData.UnitLevel = 1;

            PlayerDataManager.Instance.PlayerData.UnitDatas.Add(unitData);
        }

        //Json���� ��ȯ�� ����.
        string json = JsonUtility.ToJson(PlayerDataManager.Instance.PlayerData);
        root.SetRawJsonValueAsync(json);


        // ���������� �Լ� ���۽� �� ��ȯ.
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
