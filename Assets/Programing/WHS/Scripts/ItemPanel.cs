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
        UpdateCurrencyUI();

        AddEvent("BackButton", EventType.Click, ItemTEST);
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

    // 재화 UI 갱신
    public void UpdateCurrencyUI()
    {        
        GetUI<TextMeshProUGUI>("CoinText").text = PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Coin].ToString();
        GetUI<TextMeshProUGUI>("DinoBloodText").text = PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoBlood].ToString();
        GetUI<TextMeshProUGUI>("BoneCrystalText").text = PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.BoneCrystal].ToString();
        GetUI<TextMeshProUGUI>("DinoStoneText").text = PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoStone].ToString();
    }

    public void ItemTEST(PointerEventData eventData)
    {
        int currentCoinAmount = PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Coin];
        PlayerDataManager.Instance.PlayerData.SetItem((int)E_Item.Coin, currentCoinAmount + 100000);
        PlayerDataManager.Instance.PlayerData.SetItem((int)E_Item.DinoBlood, currentCoinAmount + 100000);
        PlayerDataManager.Instance.PlayerData.SetItem((int)E_Item.BoneCrystal, currentCoinAmount + 100000);
        PlayerDataManager.Instance.PlayerData.SetItem((int)E_Item.DinoStone, currentCoinAmount + 100000);
        UpdateCurrencyUI();
    }
}
