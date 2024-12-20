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
        int requiredCoin = 100;

        if (Inventory.instance.SpendItem(ItemID.Coin, requiredCoin))
        {
            character.level++;
            Debug.Log($"{character.Name}의 레벨이 {character.level}로 올랐습니다.");
            return true;
        }
        else
        {
            Debug.Log("코인이 부족합니다");
            return false;
        }
    }
}
