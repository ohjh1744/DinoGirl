using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterPanel : UIBInder
{
    private PlayerUnitData curCharacter;
    private GameObject levelUpPanel;
    
    private void Awake()
    {
        BindAll();
        AddEvent("LevelUpButton", EventType.Click, OnLevelUpButtonClick);

        Transform parent = GameObject.Find("CharacterPanel").transform;
        levelUpPanel = parent.Find("LevelUpPanel").gameObject;
    }

    private void Start()
    {
        UpdateCharacterInfo(curCharacter);
    }
    
    // 캐릭터 정보 갱신
    public void UpdateCharacterInfo(PlayerUnitData character)
    {
        
        curCharacter = character;
        GetUI<TextMeshProUGUI>("unitid").text = character.UnitId.ToString();
        // GetUI<TextMeshProUGUI>("NameText").text = character.Name;
        GetUI<TextMeshProUGUI>("LevelText").text = character.UnitLevel.ToString();
        /*
        GetUI<TextMeshProUGUI>("HPText").text = "HP : " + character.Hp.ToString();
        GetUI<TextMeshProUGUI>("AttackText").text = "Atk : " + character.Atk.ToString();
        GetUI<TextMeshProUGUI>("DefText").text = "Def : " + character.Def.ToString();
        GetUI<TextMeshProUGUI>("ClassText").text = "Class : " + character.Type;
        GetUI<TextMeshProUGUI>("ElementText").text = "Element : " + character.ElementName;
        GetUI<TextMeshProUGUI>("GridText").text = "Grid : " + character.Grid;
        GetUI<TextMeshProUGUI>("StatIdText").text = "StatID : " + character.StatId.ToString();
        GetUI<TextMeshProUGUI>("PercentIncreaseText").text = "PI : " + character.PercentIncrease.ToString();
        */
        GetUI<Button>("LevelUpButton").interactable = (character.UnitLevel < 30);
        
    }
    
    private void OnLevelUpButtonClick(PointerEventData eventData)
    {
        if (curCharacter != null && curCharacter.UnitLevel < 30)
        {
            levelUpPanel.gameObject.SetActive(true);

            LevelUpPanel levelUp = levelUpPanel.GetComponent<LevelUpPanel>();
            levelUp.Initialize(curCharacter);
        }
    }
    
}
