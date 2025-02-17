using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using Firebase.Database;


public class LobbyPanel : UIBInder
{

    [SerializeField] private SceneChanger _sceneChanger;

    [SerializeField] private int _resetAddFriendNum; // AddFriend 초기화 시 횟수
      
    [SerializeField] private int _resetAddFriendTime; // AddFriend를 초기화할 시간 ex)8시

    [SerializeField] private Sprite[] _mainUnitImages;// 로비씬 전체이미지
     
    [SerializeField] private Sprite[] _mainUnitPortraitImages; //Player정보창에서의 메인캐릭터 얼굴이미지

    //ButtonSound
    [SerializeField] private AudioClip _buttonClip;

    //Bgm
    [SerializeField] private AudioClip _bgmClip;

    private static string[] _itemValueTexts = { "LobbyItem1Value", "LobbyItem2Value", "LobbyItem3Value", "LobbyItem4Value", "LobbyItem5Value" };

    private bool _isCheckResetAddFriend;

    private string _lastResetTime;

    private DateTime _parsedDateTime;

    private void Awake()
    {
        BindAll();
    }

    private void OnEnable()
    {

        PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.Coin] += UpdateCoin;
        //검정LayOutOn
        GetUI<Button>("LobbyPlayerButton").onClick.AddListener(SetBlackPanelTrue);
        GetUI<Button>("LobbyMailButton").onClick.AddListener(SetBlackPanelTrue);
        GetUI<Button>("LobbyAddFriendButton").onClick.AddListener(SetBlackPanelTrue);
        GetUI<Button>("LobbyFriendsButton").onClick.AddListener(SetBlackPanelTrue);
        GetUI<Button>("LobbySettingButton").onClick.AddListener(SetBlackPanelTrue);

        //검정LayOutOff
        GetUI<Button>("PlayerExitButton").onClick.AddListener(SetBlackPanelFalse);
        GetUI<Button>("AddFriendExitButton").onClick.AddListener(SetBlackPanelFalse);
        GetUI<Button>("FriendsExitButton").onClick.AddListener(SetBlackPanelFalse);
        GetUI<Button>("MailExitButton").onClick.AddListener(SetBlackPanelFalse);
        GetUI<Button>("SettingExitButton").onClick.AddListener(SetBlackPanelFalse);

        //씬전환
        GetUI<Button>("LobbyChapterButton").onClick.AddListener(() => ChangeScene("ChapterSelect_LJH"));
        GetUI<Button>("LobbyRaidButton").onClick.AddListener(() => ChangeScene("RaidReady_LJH"));
        GetUI<Button>("LobbyCharacterButton").onClick.AddListener(() => ChangeScene("InventoryScene_WHS"));
        GetUI<Button>("LobbyRoomButton").onClick.AddListener(() => ChangeScene("RoomScene_WHS"));
        GetUI<Button>("LobbyGachaButton").onClick.AddListener(() => ChangeScene("ShopScene_YJE"));

        //Sound
        SoundManager.Instance.StopBGM();
        SoundManager.Instance.PlayeBGM(_bgmClip);
        GetUI<Button>("LobbyPlayerButton").onClick.AddListener(() => SoundManager.Instance.PlaySFX(_buttonClip));
        GetUI<Button>("PlayerExitButton").onClick.AddListener(() => SoundManager.Instance.PlaySFX(_buttonClip));
        GetUI<Button>("LobbyMailButton").onClick.AddListener(() => SoundManager.Instance.PlaySFX(_buttonClip));
        GetUI<Button>("LobbyAddFriendButton").onClick.AddListener(() => SoundManager.Instance.PlaySFX(_buttonClip));
        GetUI<Button>("LobbyFriendsButton").onClick.AddListener(() => SoundManager.Instance.PlaySFX(_buttonClip));
        GetUI<Button>("LobbySettingButton").onClick.AddListener(() => SoundManager.Instance.PlaySFX(_buttonClip));
        GetUI<Button>("LobbyChapterButton").onClick.AddListener(() => SoundManager.Instance.PlaySFX(_buttonClip));
        GetUI<Button>("LobbyRaidButton").onClick.AddListener(() => SoundManager.Instance.PlaySFX(_buttonClip));
        GetUI<Button>("LobbyRoomButton").onClick.AddListener(() => SoundManager.Instance.PlaySFX(_buttonClip));
        GetUI<Button>("LobbyCharacterButton").onClick.AddListener(() => SoundManager.Instance.PlaySFX(_buttonClip));
        GetUI<Button>("LobbyGachaButton").onClick.AddListener(() => SoundManager.Instance.PlaySFX(_buttonClip));
    }

    private void Start()
    {
        _lastResetTime = PlayerDataManager.Instance.PlayerData.LastResetAddFriendTime;
        _parsedDateTime = DateTime.ParseExact(_lastResetTime, "yyyyMMdd_HHmmss_fff", null);
        ShowName();
        ShowMainUnit();
        ShowItems();
        CheckResetAddFriend();
    }

    private void OnDisable()
    {
        PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.Coin] -= UpdateCoin;
    }

    private void Update()
    {
        //하루이상 계속 로비상태에서 게임을 켜놨을때의 예외경우 생각.
        if(DateTime.Now.Hour >= _resetAddFriendTime && _parsedDateTime.Day != DateTime.Now.Day)
        {
            CheckResetAddFriend();
        }
    }

    // 친구 추가 초기화 확인
    private void CheckResetAddFriend()
    {
        Debug.Log(_parsedDateTime.Date);
        Debug.Log(_parsedDateTime.Year);
        Debug.Log(_parsedDateTime.Month);
        Debug.Log(_parsedDateTime.Day);
        Debug.Log(_parsedDateTime.Hour);

        //현재 날짜가 마지막으로 리셋한 날짜보다 최소한 다음날이여야함.
        //하루차이인경우
        if (_parsedDateTime.Day + 1 == DateTime.Now.Day)
        {
            if (DateTime.Now.Hour >= _resetAddFriendTime)
            {
                ResetAddFriend();
            }

        }
        else
        {
            // 같은 날인 경우 무시 
            if(DateTime.Now.Date == _parsedDateTime.Date)
            {
                return;
            }
            //이틀이상인경우
            ResetAddFriend();
        }

    }

    private void ResetAddFriend()
    {
        DatabaseReference root = BackendManager.Instance.Database.RootReference.Child("UserData").Child(BackendManager.Instance.Auth.CurrentUser.UserId);
        Dictionary<string, object> dic = new Dictionary<string, object>();

        //canAddFriend 초기화
        PlayerDataManager.Instance.PlayerData.CanAddFriend = _resetAddFriendNum;
        dic[$"/_canAddFriend"] = _resetAddFriendNum;

        //_lastResetAddFriendTime초기화
        PlayerDataManager.Instance.PlayerData.LastResetAddFriendTime = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
        dic[$"/_lastResetAddFriendTime"] = PlayerDataManager.Instance.PlayerData.LastResetAddFriendTime;
        _lastResetTime = PlayerDataManager.Instance.PlayerData.LastResetAddFriendTime;
        _parsedDateTime = DateTime.ParseExact(_lastResetTime, "yyyyMMdd_HHmmss_fff", null);

        root.UpdateChildrenAsync(dic);

        Debug.Log("AddFRiend 초기화!");

    }

    private void ShowName()
    {
        StringBuilder nameSb = new StringBuilder();
        nameSb.Clear();
        nameSb.Append(PlayerDataManager.Instance.PlayerData.PlayerName);
        nameSb.Append("#");
        nameSb.Append(BackendManager.Instance.Auth.CurrentUser.UserId.Substring(0, 4));
        GetUI<TextMeshProUGUI>("LobbyPlayerNameText").SetText(nameSb);
        GetUI<TextMeshProUGUI>("PlayerPlayerNameText").SetText(nameSb);
    }

    private void ShowMainUnit()
    {
        GetUI<Image>("LobbyMainUnitImage").sprite = _mainUnitImages[PlayerDataManager.Instance.PlayerData.MainUnitID - 1];
        GetUI<Image>("PlayerMainUnitImage").sprite = _mainUnitPortraitImages[PlayerDataManager.Instance.PlayerData.MainUnitID - 1];
        Dictionary<int, Dictionary<string, string>> charDic = CsvDataManager.Instance.DataLists[(int)E_CsvData.Character];
        GetUI<TextMeshProUGUI>("PlayerMainUnitNameText").text = charDic[PlayerDataManager.Instance.PlayerData.MainUnitID]["Name"];
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

    private void SetBlackPanelTrue()
    {
        GetUI("BlackPanel").gameObject.SetActive(true);
    }

    private void SetBlackPanelFalse()
    {
        GetUI("BlackPanel").gameObject.SetActive(false);
    }

    private void ChangeScene(string sceneName)
    {
        _sceneChanger.CanChangeSceen = true;
        _sceneChanger.ChangeScene(sceneName);
    }

  

}
