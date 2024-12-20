using Photon.Pun.Demo.Procedural;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterSlotUI : UIBInder
{
    private Character character;
    private GameObject characterPanel;

    private void Awake()
    {
        BindAll();
        AddEvent("Character(Clone)", EventType.Click, OnClick);

        Transform parent = GameObject.Find("MainPanel").transform;
        characterPanel = parent.Find("CharacterPanel").gameObject;
    }

    // 인벤토리에 캐릭터 세팅 - 이미지, 이름, 레벨 ( 레어도, 포지션 등 )
    public void SetCharacter(Character newCharacter)
    {
        character = newCharacter;

        GetUI<TextMeshProUGUI>("NameText").text = character.Name;
        GetUI<TextMeshProUGUI>("LevelText").text = character.level.ToString();
        //GetUI<Image>("Character").sprite = character.image;
    }

    // 클릭 시 ( 캐릭터 정보 출력, 추가 UI )
    private void OnClick(PointerEventData eventData)
    {
        characterPanel.SetActive(true);
        characterPanel.GetComponent<CharacterPanel>().UpdateCharacterInfo(character);
    }

    public Character GetCharacter()
    {
        return character;
    }

}
