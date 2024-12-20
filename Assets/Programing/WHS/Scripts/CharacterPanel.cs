using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterPanel : UIBInder
{
    private Character curCharacter;

    private void Awake()
    {
        BindAll();
        AddEvent("LevelUpButton", EventType.Click, OnLevelUpButtonClick);
        
    }

    private void Start()
    {
        UpdateCharacterInfo(curCharacter);
    }

    public void UpdateCharacterInfo(Character character)
    {
        curCharacter = character;
        GetUI<TextMeshProUGUI>("NameText").text = character.Name;
        GetUI<TextMeshProUGUI>("LevelText").text = character.level.ToString();
    }

    // 레벨업 버튼 클릭
    private void OnLevelUpButtonClick(PointerEventData eventData)
    {
        if (curCharacter != null)
        {
            if (LevelUp(curCharacter))
            {
                // UI 갱신
                UpdateCharacterInfo(curCharacter);
                FindObjectOfType<CharacterInventoryUI>().UpdateCharacterUI(curCharacter);
                ItemUI.instance.UpdateCurrencyUI();
            }
        }
    }

    private bool LevelUp(Character character)
    {
        int coin = 10;
        int dinoBlood = 100;
        int boneCrystal = 50;

        int requiredCoin = coin + (character.level * 20);
        int requiredDinoBlood = dinoBlood + (character.level * 10);
        int requiredBoneCrystal = 0;

        if(character.level % 5 == 0)
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
                if(requiredBoneCrystal > 0)
                {
                    Debug.Log($"본 크리스탈 {requiredBoneCrystal} 소모");
                }
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
}
