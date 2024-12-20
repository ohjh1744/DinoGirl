using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterPanel : UIBInder
{
    private Character curCharacter;
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
    public void UpdateCharacterInfo(Character character)
    {
        curCharacter = character;
        GetUI<TextMeshProUGUI>("NameText").text = character.Name;
        GetUI<TextMeshProUGUI>("LevelText").text = character.level.ToString();
    }

    private void OnLevelUpButtonClick(PointerEventData eventData)
    {
        if (curCharacter != null)
        {
            levelUpPanel.gameObject.SetActive(true);

            LevelUpPanel levelUp = levelUpPanel.GetComponent<LevelUpPanel>();
            levelUp.Initialize(curCharacter);
        }
    }
}
