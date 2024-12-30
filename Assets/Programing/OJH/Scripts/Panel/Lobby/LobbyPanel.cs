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

    private static string[] _itemValueTexts = { "Item1Value", "Item2Value", "Item3Value", "Item4Value", "Item5Value" };

    private void Awake()
    {
        BindAll();
    }

    private void OnEnable()
    {
        PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.Coin] += UpdateCoin;
    }

    private void OnDisable()
    {
        PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.Coin] -= UpdateCoin;
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
        GetUI<TextMeshProUGUI>("PlayerPanelPlayerNameText").SetText(nameSb);
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
        GetUI<TextMeshProUGUI>("Item1Value").SetText(itemSb);
    }


}
