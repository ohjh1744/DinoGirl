using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterSlot : UIBInder
{
    private PlayerUnitData _unitData;
    private GameObject _characterPanel;

    private void Awake()
    {
        BindAll();
        AddEvent("Character(Clone)", EventType.Click, OnClick);

        Transform parent = GameObject.Find("MainPanel").transform;
        _characterPanel = parent.Find("CharacterPanel").gameObject;
    }

    // 인벤토리에 캐릭터 세팅 - 이미지, 이름, 레벨 ( 레어도, 포지션 등 )
    public void SetCharacter(PlayerUnitData newUnitData)
    {
        _unitData = newUnitData;

        Dictionary<int, Dictionary<string, string>> characterData = CsvDataManager.Instance.DataLists[(int)E_CsvData.Character];

        if (characterData.TryGetValue(_unitData.UnitId, out var data))
        {
            GetUI<TextMeshProUGUI>("NameText").text = data["Name"];
        }
        else
        {
            GetUI<TextMeshProUGUI>("NameText").text = _unitData.UnitId.ToString();
        }

        GetUI<TextMeshProUGUI>("LevelText").text = _unitData.UnitLevel.ToString();

        if (int.TryParse(data["Rarity"], out int rarity))
        {
            UpdateStar(rarity);
        }

        // 원소 속성 이미지 설정
        if (int.TryParse(data["ElementID"], out int elementId))
        {
            string elementPath = $"UI/element_{elementId}";
            Sprite elementSprite = Resources.Load<Sprite>(elementPath);
            if (elementSprite != null)
            {
                GetUI<Image>("ElementImage").sprite = elementSprite;
            }
            else
            {
                Debug.LogWarning($"이미지를 찾을 수 없음: {elementPath}");
            }
        }

        string portraitPath = $"Portrait/portrait_{_unitData.UnitId}";
        if (portraitPath != null)
        {
            GetUI<Image>("CharacterImage").sprite = Resources.Load<Sprite>(portraitPath);
        }
        else
        {
            Debug.Log($"이미지를 찾을 수 없음 {portraitPath}");
        }
    }

    // 클릭 시 ( 캐릭터 정보 출력, 추가 UI )
    private void OnClick(PointerEventData eventData)
    {
        _characterPanel.SetActive(true);
        BackButtonManager.Instance.AddBackAction(ClosePanel);

        _characterPanel.GetComponent<CharacterPanel>().UpdateCharacterInfo(_unitData);
    }

    public void ClosePanel()
    {
        _characterPanel.SetActive(false);
    }

    public PlayerUnitData GetCharacter()
    {
        return _unitData;
    }

    private void UpdateStar(int rarity)
    {
        // rarity에 따라 별 개수를 출력
        for (int i = 0; i < 5; i++)
        {
            Image starImage = GetUI<Image>($"Star_{i + 1}");
            if (starImage != null)
            {
                if (i < rarity)
                {
                    starImage.gameObject.SetActive(true);
                    starImage.sprite = Resources.Load<Sprite>("UI/icon_star");
                }
                else
                {
                    starImage.gameObject.SetActive(false);
                }
            }
        }
    }
}
