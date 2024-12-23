using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelUpPanel : UIBInder
{
    private Character targetCharacter;
    private int maxLevelUp;
    private int curLevelUp;
    private const int MAXLEVEL = 30;

    private struct RequiredItems
    {
        public int coin;
        public int dinoBlood;
        public int boneCrystal;
    }

    private void Awake()
    {
        BindAll();
        AddEvent("LevelUpConfirm", EventType.Click, OnConfirmButtonClick);
        GetUI<Slider>("LevelUpSlider").onValueChanged.AddListener(OnSliderValueChanged);
        AddEvent("DecreaseButton", EventType.Click, OnDecreaseButtonClick);
        AddEvent("IncreaseButton", EventType.Click, OnIncreaseButtonClick);
    }

    public void Initialize(Character character)
    {
        targetCharacter = character;
        CalculateMaxLevelUp();
        UpdateUI();

        Slider slider = GetUI<Slider>("LevelUpSlider");
        slider.minValue = 0;
        slider.maxValue = maxLevelUp;
        slider.value = 0;
    }

    // 슬라이더 값 갱신
    private void OnSliderValueChanged(float value)
    {
        curLevelUp = Mathf.RoundToInt(value);
        UpdateUI();
    }

    // 재화소모량 갱신
    private void UpdateUI()
    {
        RequiredItems items = CalculateRequiredItems(curLevelUp);

        GetUI<TextMeshProUGUI>("CoinText").text = $"Coin : {items.coin}";
        GetUI<TextMeshProUGUI>("DinoBloodText").text = $"DinoBlood : {items.dinoBlood}";
        GetUI<TextMeshProUGUI>("BoneCrystalText").text = $"BoneCrystal : {items.boneCrystal}";

        if (targetCharacter.level + curLevelUp >= MAXLEVEL)
        {
            GetUI<TextMeshProUGUI>("LevelText").text = $"Lv.{targetCharacter.level} -> Lv.{MAXLEVEL} (MAX)";
        }
        else
        {
            GetUI<TextMeshProUGUI>("LevelText").text = $"Lv.{targetCharacter.level} -> Lv.{targetCharacter.level + curLevelUp}";
        }
    }

    // 레벨업 할 최대치
    private void CalculateMaxLevelUp()
    {
        maxLevelUp = 0;
        while (CanLevelUp(maxLevelUp + 1)
            && (targetCharacter.level + maxLevelUp + 1) <= MAXLEVEL)
        {
            maxLevelUp++;
        }
    }

    // 레벨업 가능 여부
    private bool CanLevelUp(int level)
    {
        if (targetCharacter.level + level > MAXLEVEL)
        {
            return false;
        }

        RequiredItems items = CalculateRequiredItems(level);

        // 레벨업에 충분한 양을 가지고 있으면 true
        return Inventory.instance.GetItemAmount(ItemID.Coin) >= items.coin &&
               Inventory.instance.GetItemAmount(ItemID.DinoBlood) >= items.dinoBlood &&
               Inventory.instance.GetItemAmount(ItemID.BoneCrystal) >= items.boneCrystal;
    }

    // 요구 재화량 계산
    private RequiredItems CalculateRequiredItems(int level)
    {
        RequiredItems items = new RequiredItems();

        items.coin = CalculateRequiredCoin(level);
        items.dinoBlood = CalculateRequiredDinoBlood(level);
        items.boneCrystal = CalculateRequiredBoneCrystal(level);

        return items;
    }

    // 레벨에 따른 재화 요구량 계산
    private int CalculateRequiredCoin(int levels)
    {
        int total = 0;
        for (int i = 0; i < levels; i++)
        {
            total += 30 + (targetCharacter.level + i - 1) * 60;
        }
        return total;
    }

    // 다이노블러드 요구량 계산
    private int CalculateRequiredDinoBlood(int levels)
    {
        int total = 0;
        for (int i = 0; i < levels; i++)
        {
            total += 90 + (targetCharacter.level + i - 1) * 70;
        }
        return total;
    }

    // 레벨에 따른 본크리스탈 요구량 계산
    private int CalculateRequiredBoneCrystal(int levels)
    {
        int total = 0;

        for (int i = 0; i < levels; i++)
        {
            // 5레벨마다 본크리스탈 요구량
            if ((targetCharacter.level + i) % 5 == 4)
            {
                total += 10 + (((targetCharacter.level + i + 1) / 5) * 40);
            }
        }
        return total;
    }

    // 레벨업 버튼
    private void OnConfirmButtonClick(PointerEventData eventData)
    {
        for (int i = 0; i < curLevelUp; i++)
        {
            if (LevelUp(targetCharacter) == false)
            {
                break;
            }
        }
        gameObject.SetActive(false);
    }

    // 레벨업
    private bool LevelUp(Character character)
    {
        RequiredItems items = CalculateRequiredItems(1);

        if (Inventory.instance.GetItemAmount(ItemID.Coin) >= items.coin &&
            Inventory.instance.GetItemAmount(ItemID.DinoBlood) >= items.dinoBlood &&
            Inventory.instance.GetItemAmount(ItemID.BoneCrystal) >= items.boneCrystal)
        {
            if (Inventory.instance.SpendItem(ItemID.Coin, items.coin) &&
                Inventory.instance.SpendItem(ItemID.DinoBlood, items.dinoBlood) &&
                (items.boneCrystal == 0 || Inventory.instance.SpendItem(ItemID.BoneCrystal, items.boneCrystal)))
            {
                character.level++;
                Debug.Log($"{character.Name} 레벨업 {character.level}");
                if (items.boneCrystal > 0)
                {
                    Debug.Log($"본 크리스탈 {items.boneCrystal} 소모");
                }

                UpdateCharacters(character);
                ItemUI.instance.UpdateCurrencyUI();

                return true;
            }
        }

        if (Inventory.instance.GetItemAmount(ItemID.Coin) < items.coin)
        {
            Debug.Log($"코인이 {Inventory.instance.GetItemAmount(ItemID.Coin) - items.coin} 부족합니다.");
        }
        if (Inventory.instance.GetItemAmount(ItemID.DinoBlood) < items.dinoBlood)
        {
            Debug.Log($"다이노블러드가 {Inventory.instance.GetItemAmount(ItemID.DinoBlood) - items.dinoBlood} 부족합니다");
        }
        if (Inventory.instance.GetItemAmount(ItemID.BoneCrystal) < items.boneCrystal)
        {
            Debug.Log($"본크리스탈이 {Inventory.instance.GetItemAmount(ItemID.BoneCrystal) - items.boneCrystal} 부족합니다");
        }

        return false;
    }

    // UI 갱신
    private void UpdateCharacters(Character character)
    {
        CharacterPanel characterPanel = FindObjectOfType<CharacterPanel>();
        if (characterPanel != null)
        {
            characterPanel.UpdateCharacterInfo(character);
        }

        CharacterInventoryUI characterInventoryUI = FindObjectOfType<CharacterInventoryUI>();
        if (characterInventoryUI != null)
        {
            characterInventoryUI.UpdateCharacterUI(character);
        }
    }

    private void OnDecreaseButtonClick(PointerEventData eventData)
    {
        if (curLevelUp > 0)
        {
            curLevelUp--;
            UpdateUI();
            GetUI<Slider>("LevelUpSlider").value = curLevelUp;
        }
    }

    private void OnIncreaseButtonClick(PointerEventData eventData)
    {
        if (curLevelUp < maxLevelUp && (targetCharacter.level + curLevelUp) + 1 <= MAXLEVEL)
        {
            curLevelUp++;
            UpdateUI();
            GetUI<Slider>("LevelUpSlider").value = curLevelUp;
        }
    }
}
