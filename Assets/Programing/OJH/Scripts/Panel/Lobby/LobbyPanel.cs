using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine.UI;
using System;
using Firebase.Database;

public class LobbyPanel : UIBInder
{

    [SerializeField] private SceneChanger _sceneChanger;

    [SerializeField] private int _resetAddFriendNum;
  
    private static string[] _itemValueTexts = { "LobbyItem1Value", "LobbyItem2Value", "LobbyItem3Value", "LobbyItem4Value", "LobbyItem5Value" };

    private bool _isCheckResetAddFriend;

    private void Awake()
    {
        BindAll();
    }

    private void OnEnable()
    {
        PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.Coin] += UpdateCoin;
        GetUI<Button>("LobbyPlayerButton").onClick.AddListener(SetInteractableFalse);
        GetUI<Button>("LobbyMailButton").onClick.AddListener(SetInteractableFalse);
        GetUI<Button>("LobbyAddFriendButton").onClick.AddListener(SetInteractableFalse);
        GetUI<Button>("LobbyFriendsButton").onClick.AddListener(SetInteractableFalse);

        GetUI<Button>("PlayerExitButton").onClick.AddListener(SetInteractableTrue);
        GetUI<Button>("AddFriendExitButton").onClick.AddListener(SetInteractableTrue);
        GetUI<Button>("FriendsExitButton").onClick.AddListener(SetInteractableTrue);
        GetUI<Button>("MailExitButton").onClick.AddListener(SetInteractableTrue);

        GetUI<Button>("LobbyChapterButton").onClick.AddListener(ChangeChapterScene);
        GetUI<Button>("LobbyCharacterButton").onClick.AddListener(ChangeCharacterScene);
        GetUI<Button>("LobbyRoomButton").onClick.AddListener(ChangeRoomScene);
        GetUI<Button>("LobbyGachaButton").onClick.AddListener(ChangeGachaScene);
    }

    private void OnDisable()
    {
        PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.Coin] -= UpdateCoin;
        GetUI<Button>("LobbyPlayerButton").onClick.RemoveListener(SetInteractableFalse);
        GetUI<Button>("LobbyMailButton").onClick.RemoveListener(SetInteractableFalse);
        GetUI<Button>("LobbyAddFriendButton").onClick.RemoveListener(SetInteractableFalse);
        GetUI<Button>("LobbyFriendsButton").onClick.RemoveListener(SetInteractableFalse);

        GetUI<Button>("PlayerExitButton").onClick.RemoveListener(SetInteractableTrue);
        GetUI<Button>("AddFriendExitButton").onClick.RemoveListener(SetInteractableTrue);
        GetUI<Button>("FriendsExitButton").onClick.RemoveListener(SetInteractableTrue);
        GetUI<Button>("MailExitButton").onClick.RemoveListener(SetInteractableTrue);

        GetUI<Button>("LobbyChapterButton").onClick.RemoveListener(ChangeChapterScene);
        GetUI<Button>("LobbyCharacterButton").onClick.RemoveListener(ChangeCharacterScene);
        GetUI<Button>("LobbyRoomButton").onClick.RemoveListener(ChangeRoomScene);
        GetUI<Button>("LobbyGachaButton").onClick.RemoveListener(ChangeGachaScene);
    }

    private void Start()
    {
        ShowName();
        ShowItems();
        CheckResetAddFriend();
    }

    private void Update()
    {
        //하루종일 게임을 켜놨을때의 예외경우 생각.
        if(DateTime.Now.Hour >= 8 && _isCheckResetAddFriend == false)
        {
            _isCheckResetAddFriend = true;
            CheckResetAddFriend();
        }
    }

    // 친구 추가 초기화 확인
    private void CheckResetAddFriend()
    {
        string lastResetTime = PlayerDataManager.Instance.PlayerData.LastResetAddFriendTime;

        DateTime parsedDateTime = DateTime.ParseExact(lastResetTime, "yyyyMMdd_HHmmss_fff", null);

        Debug.Log(parsedDateTime.Year);
        Debug.Log(parsedDateTime.Month);
        Debug.Log(parsedDateTime.Day);
        Debug.Log(parsedDateTime.Hour);

        if (parsedDateTime.Year <= DateTime.Now.Year && parsedDateTime.Month <= DateTime.Now.Month)
        {
            //현재 날짜가 마지막으로 리셋한 날짜보다 최소한 다음날이여야함.
            //하루차이인경우
            if(parsedDateTime.Day+1 == DateTime.Now.Day)
            {
                if(DateTime.Now.Hour >= 8)
                {
                    ResetAddFriend();
                }

            }
            //이틀이상인경우
            else if(parsedDateTime.Day < DateTime.Now.Day)
            {
                ResetAddFriend();
            }
        }

    }

    private void ResetAddFriend()
    {
        DatabaseReference root = BackendManager.Database.RootReference.Child("UserData").Child(BackendManager.Auth.CurrentUser.UserId);
        Dictionary<string, object> dic = new Dictionary<string, object>();

        //canAddFriend 초기화
        PlayerDataManager.Instance.PlayerData.CanAddFriend = _resetAddFriendNum;
        dic[$"/_canAddFriend"] = _resetAddFriendNum;

        //_lastResetAddFriendTime초기화
        PlayerDataManager.Instance.PlayerData.LastResetAddFriendTime = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
        dic[$"/_lastResetAddFriendTime"] = PlayerDataManager.Instance.PlayerData.LastResetAddFriendTime;

        root.UpdateChildrenAsync(dic);

        Debug.Log("AddFRiend 초기화!");

    }

    private void ShowName()
    {
        StringBuilder nameSb = new StringBuilder();
        nameSb.Clear();
        nameSb.Append(PlayerDataManager.Instance.PlayerData.PlayerName);
        nameSb.Append("#");
        nameSb.Append(BackendManager.Auth.CurrentUser.UserId.Substring(0, 4));
        GetUI<TextMeshProUGUI>("LobbyPlayerNameText").SetText(nameSb);
        GetUI<TextMeshProUGUI>("PlayerPlayerNameText").SetText(nameSb);
    }

    private void ShowItems()
    {
        StringBuilder itemSb = new StringBuilder();

        for (int i = 0; i < (int)E_Item.Length; i++)
        {
            itemSb.Clear();
            itemSb.Append(PlayerDataManager.Instance.PlayerData.Items[i]);
            GetUI<TextMeshProUGUI>(_itemValueTexts[i]).SetText(itemSb);
        }

    }

    // 친구목록에 버튼을 누럿을시 Coin획득. 이를 위한 이벤트 연결함수 구현
    private void UpdateCoin(int count)
    {
        StringBuilder itemSb = new StringBuilder();
        itemSb.Clear();
        itemSb.Append(count);
        GetUI<TextMeshProUGUI>(_itemValueTexts[0]).SetText(itemSb);
    }

    private void SetInteractableFalse()
    {
        GetUI<Button>("LobbyPlayerButton").interactable = false;
        GetUI<Button>("LobbyChapterButton").interactable = false;
        GetUI<Button>("LobbyContentButton").interactable = false;
        GetUI<Button>("LobbyRoomButton").interactable = false;
        GetUI<Button>("LobbyCharacterButton").interactable = false;
        GetUI<Button>("LobbyGachaButton").interactable = false;
        GetUI<Button>("LobbyAddFriendButton").interactable = false;
        GetUI<Button>("LobbyMailButton").interactable = false;
        GetUI<Button>("LobbyFriendsButton").interactable = false;
    }

    private void SetInteractableTrue()
    {
        GetUI<Button>("LobbyPlayerButton").interactable = true;
        GetUI<Button>("LobbyChapterButton").interactable = true;
        GetUI<Button>("LobbyContentButton").interactable = true;
        GetUI<Button>("LobbyRoomButton").interactable = true;
        GetUI<Button>("LobbyCharacterButton").interactable = true;
        GetUI<Button>("LobbyGachaButton").interactable = true;
        GetUI<Button>("LobbyAddFriendButton").interactable = true;
        GetUI<Button>("LobbyMailButton").interactable = true;
        GetUI<Button>("LobbyFriendsButton").interactable = true;
    }

    private void ChangeChapterScene()
    {
        _sceneChanger.CanChangeSceen = true;
        _sceneChanger.ChangeScene("ChapterZero_LJH");
    }

    private void ChangeCharacterScene()
    {
        _sceneChanger.CanChangeSceen = true;
        _sceneChanger.ChangeScene("InventoryScene_WHS");
    }

    private void ChangeRoomScene()
    {
        _sceneChanger.CanChangeSceen = true;
        _sceneChanger.ChangeScene("RoomScene_WHS");
    }

    private void ChangeGachaScene()
    {
        _sceneChanger.CanChangeSceen = true;
        _sceneChanger.ChangeScene("GachaScene_YJE");
    }


}
