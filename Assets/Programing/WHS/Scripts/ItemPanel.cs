using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ItemPanel : UIBInder
{
    private void Start()
    {
        BindAll();
        StartCoroutine(WaitForPlayerData());

        AddEvent("BackButton", EventType.Click, ItemTEST);
    }

    private IEnumerator WaitForPlayerData()
    {
        // PlayerDataManager가 초기화되고 PlayerData가 로드될 때까지 대기
        yield return new WaitUntil(() =>
            PlayerDataManager.Instance != null &&
            PlayerDataManager.Instance.PlayerData != null &&
            PlayerDataManager.Instance.PlayerData.UnitDatas != null &&
            PlayerDataManager.Instance.PlayerData.UnitDatas.Count > 0);

        Init();
    }

    private void OnEnable()
    {
        if (PlayerDataManager.Instance.PlayerData.OnItemChanged == null)
        {
            PlayerDataManager.Instance.PlayerData.OnItemChanged = new UnityAction<int>[System.Enum.GetValues(typeof(E_Item)).Length];
        }

        PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.Coin] += UpdateCoinText;
        PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.DinoBlood] += UpdateDinoBloodText;
        PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.BoneCrystal] += UpdateBoneCrystalText;
        PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.DinoStone] += UpdateDinoStoneText;
    }

    private void OnDisable()
    {
        if (PlayerDataManager.Instance != null && PlayerDataManager.Instance.PlayerData != null)
        {
            PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.Coin] -= UpdateCoinText;
            PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.DinoBlood] -= UpdateDinoBloodText;
            PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.BoneCrystal] -= UpdateBoneCrystalText;
            PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.DinoStone] -= UpdateDinoStoneText;
        }
    }

    private void Init()
    {
        if (PlayerDataManager.Instance != null && PlayerDataManager.Instance.PlayerData != null)
        {
            UpdateCoinText(PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Coin]);
            UpdateDinoBloodText(PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoBlood]);
            UpdateBoneCrystalText(PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.BoneCrystal]);
            UpdateDinoStoneText(PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoStone]);
        }
        else
        {
            Debug.LogWarning("PlayerDataManager or PlayerData is null. Unable to load initial data.");
        }
    }

    private void UpdateCoinText(int newValue)
    {
        GetUI<TextMeshProUGUI>("CoinText").text = newValue.ToString();
    }

    private void UpdateDinoBloodText(int newValue)
    {
        GetUI<TextMeshProUGUI>("DinoBloodText").text = newValue.ToString();
    }

    private void UpdateBoneCrystalText(int newValue)
    {
        GetUI<TextMeshProUGUI>("BoneCrystalText").text = newValue.ToString();
    }

    private void UpdateDinoStoneText(int newValue)
    {
        GetUI<TextMeshProUGUI>("DinoStoneText").text = newValue.ToString();
    }

    public void ItemTEST(PointerEventData eventData)
    {
        int currentCoinAmount = PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Coin];
        PlayerDataManager.Instance.PlayerData.SetItem((int)E_Item.Coin, currentCoinAmount + 100000);
        PlayerDataManager.Instance.PlayerData.SetItem((int)E_Item.DinoBlood, currentCoinAmount + 100000);
        PlayerDataManager.Instance.PlayerData.SetItem((int)E_Item.BoneCrystal, currentCoinAmount + 100000);
        PlayerDataManager.Instance.PlayerData.SetItem((int)E_Item.DinoStone, currentCoinAmount + 100000);
    }
}
