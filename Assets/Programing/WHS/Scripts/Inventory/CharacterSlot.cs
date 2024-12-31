using Photon.Pun.Demo.Procedural;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterSlot : UIBInder
{
    private PlayerUnitData unitData;
    private GameObject characterPanel;

    private void Awake()
    {
        BindAll();
        AddEvent("Character(Clone)", EventType.Click, OnClick);

        Transform parent = GameObject.Find("MainPanel").transform;
        characterPanel = parent.Find("CharacterPanel").gameObject;
    }

    // 인벤토리에 캐릭터 세팅 - 이미지, 이름, 레벨 ( 레어도, 포지션 등 )
    public void SetCharacter(PlayerUnitData newUnitData)
    {
        unitData = newUnitData;

        Dictionary<int, Dictionary<string, string>> characterData = CsvDataManager.Instance.DataLists[(int)E_CsvData.Character];
        if (characterData.TryGetValue(unitData.UnitId, out var data))
        {
            GetUI<TextMeshProUGUI>("NameText").text = data["Name"];
        }
        else
        {
            GetUI<TextMeshProUGUI>("NameText").text = unitData.UnitId.ToString();
        }
        GetUI<TextMeshProUGUI>("LevelText").text = unitData.UnitLevel.ToString();
        // TODO : 캐릭터 스프라이트 이미지
    }

    // 클릭 시 ( 캐릭터 정보 출력, 추가 UI )
    private void OnClick(PointerEventData eventData)
    {
        characterPanel.SetActive(true);

       characterPanel.GetComponent<CharacterPanel>().UpdateCharacterInfo(unitData);
    }

    public PlayerUnitData GetCharacter()
    {
        return unitData;
    }

}
