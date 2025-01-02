using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine.UI;

public class LobbyPanel : UIBInder
{

    [SerializeField] private SceneChanger _sceneChanger;

    private static string[] _itemValueTexts = { "LobbyItem1Value", "LobbyItem2Value", "LobbyItem3Value", "LobbyItem4Value", "LobbyItem5Value" };

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
    }

    private void Start()
    {
        ShowName();
        ShowItems();
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
        _sceneChanger.ChangeScene("ChapterSelect_LJH");
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


}
