using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public struct RequiredItems
{
    public int Coin;
    public int DinoBlood;
    public int BoneCrystal;
}

public class LevelUpPanel : UIBInder
{
    private PlayerUnitData _targetCharacter;
    private int _maxLevelUp;
    private int _curLevelUp;
    private LevelUp _levelUpSystem;

    private void Awake()
    {
        BindAll();
        AddEvent("LevelUpConfirm", EventType.Click, OnConfirmButtonClick);
        GetUI<Slider>("LevelUpSlider").onValueChanged.AddListener(OnSliderValueChanged);
        AddEvent("DecreaseButton", EventType.Click, OnDecreaseButtonClick);
        AddEvent("IncreaseButton", EventType.Click, OnIncreaseButtonClick);

        _levelUpSystem = new LevelUp(
            CsvDataManager.Instance.DataLists[(int)E_CsvData.CharacterLevelUp],
            CsvDataManager.Instance.DataLists[(int)E_CsvData.Character]
        );
    }

    // 레벨업 패널 초기화
    public void Init(PlayerUnitData character)
    {
        _targetCharacter = character;
        CalculateMaxLevelUp();

        Slider slider = GetUI<Slider>("LevelUpSlider");
        slider.minValue = 1;
        slider.maxValue = _maxLevelUp;
        slider.value = 1;

        _curLevelUp = 1;
        UpdateUI();
    }

    private void OnSliderValueChanged(float value)
    {
        _curLevelUp = Mathf.RoundToInt(value);
        UpdateUI();
    }

    // 아이템, 레벨 정보 갱신
    private void UpdateUI()
    {
        RequiredItems items = _levelUpSystem.CalculateRequiredItems(_targetCharacter, _curLevelUp);
        bool canLevelUp = _levelUpSystem.CanLevelUp(_targetCharacter, _curLevelUp);

        UpdateItemTexts(items, canLevelUp);
        UpdateLevelText();
        UpdateButtonStates(canLevelUp);
    }

    // 아이템 요구량 텍스트 갱신
    private void UpdateItemTexts(RequiredItems items, bool canLevelUp)
    {
        CheckItemAmount("Coin", items.Coin, PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Coin]);
        CheckItemAmount("DinoBlood", items.DinoBlood, PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoBlood]);
        CheckItemAmount("BoneCrystal", items.BoneCrystal, PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.BoneCrystal]);
    }

    // 부족한 아이템 출력
    private void CheckItemAmount(string itemName, int requiredAmount, int currentAmount)
    {
        int shortage = requiredAmount - currentAmount;
        if (shortage > 0)
        {
            GetUI<TextMeshProUGUI>($"{itemName}Text").text = $"{itemName} {shortage} 부족";
        }
        else
        {
            GetUI<TextMeshProUGUI>($"{itemName}Text").text = $"{itemName} 충분함";
        }
    }

    // 레벨 갱신
    private void UpdateLevelText()
    {
        int newLevel = Mathf.Min(_targetCharacter.UnitLevel + _curLevelUp, LevelUp.MAXLEVEL);
        GetUI<TextMeshProUGUI>("LevelText").text = $"Lv.{_targetCharacter.UnitLevel} -> Lv.{newLevel}";
    }

    // 레벨업 버튼 상태 갱신
    private void UpdateButtonStates(bool canLevelUp)
    {
        GetUI<Button>("DecreaseButton").interactable = (_curLevelUp > 1);
        GetUI<Button>("IncreaseButton").interactable = (_curLevelUp < _maxLevelUp);
        GetUI<Button>("LevelUpConfirm").interactable = canLevelUp;
        GetUI<Slider>("CharacterImage").interactable = canLevelUp;
        GetUI<RectTransform>("Handle Slide Area").gameObject.SetActive(canLevelUp);
    }

    // 레벨업 할 수 있는 최대치 계산
    private void CalculateMaxLevelUp()
    {
        _maxLevelUp = 0;
        while (_levelUpSystem.CanLevelUp(_targetCharacter, _maxLevelUp + 1))
        {
            _maxLevelUp++;
        }
    }

    // 레벨업 버튼 실행
    private void OnConfirmButtonClick(PointerEventData eventData)
    {
        if (_levelUpSystem.CanLevelUp(_targetCharacter, _curLevelUp))
        {
            _levelUpSystem.PerformLevelUp(_targetCharacter, _curLevelUp);
            UpdateCharacters(_targetCharacter);
            gameObject.SetActive(false);
        }
    }

    // CharacterPanel, InventoryPanel에 보여지는 레벨 갱신
    private void UpdateCharacters(PlayerUnitData character)
    {
        FindObjectOfType<CharacterPanel>()?.UpdateCharacterInfo(character);
        FindObjectOfType<InventoryPanel>()?.UpdateCharacterUI(character);
    }

    // 감소버튼
    private void OnDecreaseButtonClick(PointerEventData eventData)
    {
        if (_curLevelUp > 1)
        {
            _curLevelUp--;
            GetUI<Slider>("LevelUpSlider").value = _curLevelUp;
        }
    }

    // 증가버튼
    private void OnIncreaseButtonClick(PointerEventData eventData)
    {
        if (_curLevelUp < _maxLevelUp)
        {
            _curLevelUp++;
            GetUI<Slider>("LevelUpSlider").value = _curLevelUp;
        }
    }
}
