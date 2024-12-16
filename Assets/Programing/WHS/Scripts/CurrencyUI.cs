using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrencyUI : MonoBehaviour
{
    public TextMeshProUGUI dinoStoneText;
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI dinoBloodText;
    public TextMeshProUGUI boneCrystalText;

    private void Start()
    {
        UpdateCurrencyUI();
    }

    public void UpdateCurrencyUI()
    {
        dinoStoneText.text = Inventory.instance.GetCurrencyAmount(CurrencyType.DinoStone).ToString();
        coinText.text = Inventory.instance.GetCurrencyAmount(CurrencyType.Coin).ToString();
        dinoBloodText.text = Inventory.instance.GetCurrencyAmount(CurrencyType.DinoBlood).ToString();
        boneCrystalText.text = Inventory.instance.GetCurrencyAmount(CurrencyType.BoneCrystal).ToString();
    }

    // 숫자 길어지면 k, m처럼 끊기?
}
