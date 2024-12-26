using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using Firebase.Auth;
using Firebase.Extensions;

public class LobbyPanel : UIBInder
{

    [SerializeField] private SceneChanger _sceneChanger;

    [SerializeField] private int _idLength;

    private static string[] _itemValueTexts = { "Item1Value", "Item2Value", "Item3Value", "Item4Value", "Item5Value" };
    private void Awake()
    {
        BindAll();
    }

    private void Start()
    {
        ShowName();
        ShowItems();
        //PlayerDataManager.Instance.LoadHousingIDs();
    }

    private void ShowName()
    {
        StringBuilder _nameSb = new StringBuilder();
        _nameSb.Clear();
        _nameSb.Append(PlayerDataManager.Instance.PlayerData.PlayerName);
        _nameSb.Append("#");
        _nameSb.Append(BackendManager.Auth.CurrentUser.UserId.Substring(0, 4));
        GetUI<TextMeshProUGUI>("LobbyPlayerNameText").SetText(_nameSb);
        GetUI<TextMeshProUGUI>("PlayerPanelPlayerNameText").SetText(_nameSb);
    }

    private void ShowItems()
    {
        StringBuilder _itemSb = new StringBuilder();

        for (int i = 0; i < (int)E_Item.Length; i++)
        {
            _itemSb.Clear();
            _itemSb.Append(PlayerDataManager.Instance.PlayerData.Items[i]);
            GetUI<TextMeshProUGUI>(_itemValueTexts[i]).SetText(_itemSb);
        }

    }




}
