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
        int requiredCoin = CalculateRequiredItem(10, 20, curLevelUp);
        int requiredDinoBlood = CalculateRequiredItem(100, 10, curLevelUp);
        int requiredBoneCrystal = CalculateRequiredBoneCrystal(curLevelUp);

        GetUI<TextMeshProUGUI>("CoinText").text = $"Coin : {requiredCoin}";
        GetUI<TextMeshProUGUI>("DinoBloodText").text = $"DinoBlood : {requiredDinoBlood}";
        GetUI<TextMeshProUGUI>("BoneCrystalText").text = $"BoneCrystal : {requiredBoneCrystal}";
        GetUI<TextMeshProUGUI>("LevelText").text = $"Lv.{targetCharacter.level} -> Lv.{targetCharacter.level + curLevelUp}";
    }

    // 레벨업 할 최대치
    private void CalculateMaxLevelUp()
    {
        maxLevelUp = 0;
        while (CanLevelUp(maxLevelUp + 1))
        {
            maxLevelUp++;
        }
    }

    // 레벨업 가능 여부
    private bool CanLevelUp(int level)
    {
        // 레벨에 따른 재화 요구량
        int requiredCoin = CalculateRequiredItem(10, 20, level);
        int requiredDinoBlood = CalculateRequiredItem(100, 10, level);
        int requiredBoneCrystal = CalculateRequiredBoneCrystal(level);

        // 레벨업에 충분한 양을 가지고 있으면 true
        return Inventory.instance.GetItemAmount(ItemID.Coin) >= requiredCoin &&
               Inventory.instance.GetItemAmount(ItemID.DinoBlood) >= requiredDinoBlood &&
               Inventory.instance.GetItemAmount(ItemID.BoneCrystal) >= requiredBoneCrystal;
    }

    // 레벨에 따른 재화 요구량 계산
    private int CalculateRequiredItem(int baseAmount, int increasePerLevel, int level)
    {
        int total = 0;
        
        // 레벨에 따른 요구량 계산
        for (int i = 0; i < level; i++)
        {
            total += baseAmount + ((targetCharacter.level + i) * increasePerLevel);
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
            if ((targetCharacter.level + i) % 5 == 0)
            {
                total += 50 + (((targetCharacter.level + i) / 5) * 50);
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
        int coin = 10;
        int dinoBlood = 100;
        int boneCrystal = 50;

        int requiredCoin = coin + (character.level * 20);
        int requiredDinoBlood = dinoBlood + (character.level * 10);
        int requiredBoneCrystal = 0;

        if (character.level % 5 == 0)
        {
            requiredBoneCrystal = boneCrystal + ((character.level / 5) * 50);
        }

        if (Inventory.instance.GetItemAmount(ItemID.Coin) >= requiredCoin &&
        Inventory.instance.GetItemAmount(ItemID.DinoBlood) >= requiredDinoBlood &&
        Inventory.instance.GetItemAmount(ItemID.BoneCrystal) >= requiredBoneCrystal)
        {
            if (Inventory.instance.SpendItem(ItemID.Coin, requiredCoin) &&
                Inventory.instance.SpendItem(ItemID.DinoBlood, requiredDinoBlood) &&
                (requiredBoneCrystal == 0 || Inventory.instance.SpendItem(ItemID.BoneCrystal, requiredBoneCrystal)))
            {
                character.level++;
                Debug.Log($"{character.Name} 레벨업 {character.level}");
                if (requiredBoneCrystal > 0)
                {
                    Debug.Log($"본 크리스탈 {requiredBoneCrystal} 소모");
                }

                UpdateCharacters(character);
                ItemUI.instance.UpdateCurrencyUI();

                return true;
            }
        }

        if (Inventory.instance.GetItemAmount(ItemID.Coin) < requiredCoin)
        {
            Debug.Log($"코인이 {Inventory.instance.GetItemAmount(ItemID.Coin) - requiredCoin} 부족합니다.");
        }
        if (Inventory.instance.GetItemAmount(ItemID.DinoBlood) < requiredDinoBlood)
        {
            Debug.Log($"다이노블러드가 {Inventory.instance.GetItemAmount(ItemID.DinoBlood) - requiredDinoBlood} 부족합니다");
        }
        if (Inventory.instance.GetItemAmount(ItemID.BoneCrystal) < requiredDinoBlood)
        {
            Debug.Log($"본크리스탈이 {Inventory.instance.GetItemAmount(ItemID.BoneCrystal) - requiredBoneCrystal} 부족합니다");
        }

        return false;
    }

    // UI 갱신
    private void UpdateCharacters(Character character)
    {
        CharacterPanel characterPanel = FindObjectOfType<CharacterPanel>();
        if(characterPanel != null)
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
        if (curLevelUp < maxLevelUp)
        {
            curLevelUp++;
            UpdateUI();
            GetUI<Slider>("LevelUpSlider").value = curLevelUp;
        }
    }
}
