using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelUpPanel : UIBInder
{
    private PlayerUnitData _targetCharacter;
    private int _maxLevelUp;
    private int _curLevelUp;
    private const int MAXLEVEL = 30;

    private Dictionary<int, Dictionary<string, string>> _levelUpData;
    private Dictionary<int, Dictionary<string, string>> _characterData;

    private CharacterPanel _characterPanel;
    private InventoryPanel _inventoryPanel;

    private struct RequiredItems
    {
        public int Coin;
        public int DinoBlood;
        public int BoneCrystal;
    }

    private void Awake()
    {
        BindAll();
        AddEvent("LevelUpConfirm", EventType.Click, OnConfirmButtonClick);
        GetUI<Slider>("LevelUpSlider").onValueChanged.AddListener(OnSliderValueChanged);
        AddEvent("DecreaseButton", EventType.Click, OnDecreaseButtonClick);
        AddEvent("IncreaseButton", EventType.Click, OnIncreaseButtonClick);

        _levelUpData = CsvDataManager.Instance.DataLists[(int)E_CsvData.CharacterLevelUp];
        _characterData = CsvDataManager.Instance.DataLists[(int)E_CsvData.Character];

        _characterPanel = FindObjectOfType<CharacterPanel>();
        _inventoryPanel = FindObjectOfType<InventoryPanel>();

        Debug.Log($"levelUpData count: {_levelUpData.Count}");
        foreach (var key in _levelUpData.Keys)
        {
            Debug.Log($"Key: {key}, Value: {_levelUpData[key]["500"]}, {_levelUpData[key]["501"]}, {_levelUpData[key]["502"]}");
        }
    }

    private void OnEnable()
    {
        if (PlayerDataManager.Instance.PlayerData.OnItemChanged == null)
        {
            PlayerDataManager.Instance.PlayerData.OnItemChanged = new UnityAction<int>[Enum.GetValues(typeof(E_Item)).Length];
        }

        PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.Coin] += (value) => UpdateItemText(value, "Coin");
        PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.DinoBlood] += (value) => UpdateItemText(value, "DinoBlood");
        PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.BoneCrystal] += (value) => UpdateItemText(value, "BoneCrystal");
    }

    private void OnDisable()
    {
        if (PlayerDataManager.Instance != null && PlayerDataManager.Instance.PlayerData != null)
        {
            PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.Coin] -= (value) => UpdateItemText(value, "Coin");
            PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.DinoBlood] -= (value) => UpdateItemText(value, "DinoBlood");
            PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.BoneCrystal] -= (value) => UpdateItemText(value, "BoneCrystal");
        }
    }

    // 레벨업 패널이 열릴때 초기화
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

    // 슬라이더 값 갱신
    private void OnSliderValueChanged(float value)
    {
        _curLevelUp = Mathf.RoundToInt(value);
        UpdateUI();
    }

    // 재화소모량 갱신
    private void UpdateUI()
    {
        RequiredItems items = CalculateRequiredItems(_curLevelUp);

        int notEnoughCoin = Mathf.Max(0, items.Coin - PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Coin]);
        int notEnoughDinoBlood = Mathf.Max(0, items.DinoBlood - PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoBlood]);
        int notEnoughBoneCrystal = Mathf.Max(0, items.BoneCrystal - PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.BoneCrystal]);

        bool canLevelUp = (notEnoughCoin == 0 && notEnoughDinoBlood == 0 && notEnoughBoneCrystal == 0);

        GetUI<Button>("DecreaseButton").interactable = (_curLevelUp > 1);

        LoadItemImage("CoinImage", E_Item.Coin);
        LoadItemImage("DinoBloodImage", E_Item.DinoBlood);
        LoadItemImage("BoneCrystalImage", E_Item.BoneCrystal);

        if (canLevelUp)
        {
            GetUI<Button>("DecreaseButton").interactable = true;
            GetUI<Button>("IncreaseButton").interactable = true;
            GetUI<Slider>("LevelUpSlider").interactable = true;
            GetUI<RectTransform>("Handle Slide Area").gameObject.SetActive(true);

            GetUI<TextMeshProUGUI>("CoinText").text = $"Coin : {items.Coin}";
            GetUI<TextMeshProUGUI>("DinoBloodText").text = $"DinoBlood : {items.DinoBlood}";
            GetUI<TextMeshProUGUI>("BoneCrystalText").text = $"BoneCrystal : {items.BoneCrystal}";
        }
        else
        {
            GetUI<Button>("DecreaseButton").interactable = false;
            GetUI<Button>("IncreaseButton").interactable = false;
            GetUI<Slider>("LevelUpSlider").interactable = false;
            GetUI<RectTransform>("Handle Slide Area").gameObject.SetActive(false);

            if (notEnoughCoin > 0)
            {
                GetUI<TextMeshProUGUI>("CoinText").text = $"Coin {notEnoughCoin} 부족";
            }
            else
            {
                GetUI<TextMeshProUGUI>("CoinText").text = $"Coin 충분함";
            }
            if (notEnoughDinoBlood > 0)
            {
                GetUI<TextMeshProUGUI>("DinoBloodText").text = $"DinoBlood {notEnoughDinoBlood} 부족";
            }
            else
            {
                GetUI<TextMeshProUGUI>("DinoBloodText").text = $"DinoBlood 충분함";
            }
            if (notEnoughBoneCrystal > 0)
            {
                GetUI<TextMeshProUGUI>("BoneCrystalText").text = $"BoneCrystal {notEnoughBoneCrystal} 부족";
            }
            else
            {
                GetUI<TextMeshProUGUI>("BoneCrystalText").text = $"BoneCrystal 충분함";
            }
        }

        if (_targetCharacter.UnitLevel + _curLevelUp > MAXLEVEL)
        {
            GetUI<TextMeshProUGUI>("LevelText").text = $"Lv.{_targetCharacter.UnitLevel} -> Lv.{MAXLEVEL} (MAX)";
        }
        else
        {
            GetUI<TextMeshProUGUI>("LevelText").text = $"Lv.{_targetCharacter.UnitLevel} -> Lv.{_targetCharacter.UnitLevel + _curLevelUp}";
        }

        GetUI<Button>("LevelUpConfirm").interactable = canLevelUp;
    }

    // 레벨업 할 최대치
    private void CalculateMaxLevelUp()
    {
        _maxLevelUp = 0;
        while (CanLevelUp(_maxLevelUp + 1) && (_targetCharacter.UnitLevel + _maxLevelUp + 1) <= MAXLEVEL)
        {
            _maxLevelUp++;
        }
    }

    // 레벨업 가능 여부
    private bool CanLevelUp(int level)
    {
        if (_targetCharacter.UnitLevel + level > MAXLEVEL)
        {
            return false;
        }

        RequiredItems items = CalculateRequiredItems(level);

        // 레벨업에 충분한 아이템 보유중일때 true
        return PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Coin] >= items.Coin &&
               PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoBlood] >= items.DinoBlood &&
               PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.BoneCrystal] >= items.BoneCrystal;
    }

    // 요구 재화량 계산
    private RequiredItems CalculateRequiredItems(int level)
    {
        RequiredItems items = new RequiredItems();
        int rarity = GetRarity();
        int endLevel = Math.Min(_targetCharacter.UnitLevel + level, MAXLEVEL);

        for (int curLevel = _targetCharacter.UnitLevel + 1; curLevel <= endLevel; curLevel++)
        {
            int levelUpId = FindLevelUpId(rarity, curLevel);

            if (_levelUpData.TryGetValue(levelUpId, out Dictionary<string, string> data))
            {
                if (int.TryParse(data["500"], out int coin))
                {
                    items.Coin += coin;
                }
                if (int.TryParse(data["501"], out int dinoBlood))
                {
                    items.DinoBlood += dinoBlood;
                }
                if (data.ContainsKey("502") && int.TryParse(data["502"], out int boneCrystal))
                {
                    items.BoneCrystal += boneCrystal;
                }
            }
            else
            {
                Debug.LogError($"레벨업 데이터를 찾을 수 없습니다. LevelUpID: {levelUpId}");
                return new RequiredItems();
            }
        }
        return items;
    }

    // 레어도 가져오기
    private int GetRarity()
    {
        if (_characterData.TryGetValue(_targetCharacter.UnitId, out var data))
        {
            if (int.TryParse(data["Rarity"], out int rarity))
            {
                return rarity;
            }
        }
        else
        {
            Debug.LogError($"ID 찾을 수 없음 {_targetCharacter.UnitId}");
        }

        return -1;
    }

    // LevelUpID 가져오기
    private int FindLevelUpId(int rarity, int level)
    {
        if (level > MAXLEVEL)
        {
            return -1;
        }

        foreach (var entry in _levelUpData)
        {
            // 레어도와 레벨이 같을 때의 키 = LevelUpID 받아오기
            if (int.Parse(entry.Value["Rarity"]) == rarity &&
                int.Parse(entry.Value["Level"]) == level)
            {
                return entry.Key;
            }
        }

        Debug.Log($"LevelUpId 찾지 못함 Rarity {rarity}, Level {level}");

        return -1;
    }

    // 레벨업 확인 버튼
    private void OnConfirmButtonClick(PointerEventData eventData)
    {
        if (_targetCharacter.UnitLevel + _curLevelUp > MAXLEVEL)
        {
            return;
        }

        RequiredItems items = CalculateRequiredItems(_curLevelUp);

        // 아이템이 충분한지 확인하고 레벨업 진행
        if (PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Coin] >= items.Coin &&
            PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoBlood] >= items.DinoBlood &&
            PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.BoneCrystal] >= items.BoneCrystal)
        {
            for (int i = 0; i < _curLevelUp; i++)
            {
                LevelUp(_targetCharacter);
            }

            // UI 및 데이터베이스 업데이트
            UpdateItemsData(items);
            UpdateCharacters(_targetCharacter);
            UpdateLevelData(_targetCharacter);

            gameObject.SetActive(false);
        }
    }

    // 레벨업
    private bool LevelUp(PlayerUnitData character)
    {
        RequiredItems items = CalculateRequiredItems(1); // 단일 레벨업에 대한 아이템 계산

        if (items.Coin == 0 && items.DinoBlood == 0 && items.BoneCrystal == 0)
        {
            Debug.Log("요구 아이템 정보를 찾을 수 없습니다.");
            return false;
        }

        PlayerDataManager.Instance.PlayerData.SetItem((int)E_Item.Coin, PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Coin] - items.Coin);
        PlayerDataManager.Instance.PlayerData.SetItem((int)E_Item.DinoBlood, PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoBlood] - items.DinoBlood);
        PlayerDataManager.Instance.PlayerData.SetItem((int)E_Item.BoneCrystal, PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.BoneCrystal] - items.BoneCrystal);

        character.UnitLevel++;

        // 레벨업 후 PlayerDataManager 업데이트
        for (int i = 0; i < PlayerDataManager.Instance.PlayerData.UnitDatas.Count; i++)
        {
            if (PlayerDataManager.Instance.PlayerData.UnitDatas[i].UnitId == character.UnitId)
            {
                PlayerDataManager.Instance.PlayerData.UnitDatas[i] = character;
                break;
            }
        }

        Debug.Log($"{character.UnitId} 레벨업 {character.UnitLevel}");

        return true;
    }

    // db에 소모한 아이템 갱신
    private void UpdateItemsData(RequiredItems items)
    {
        string userId = BackendManager.Auth.CurrentUser.UserId;
        DatabaseReference userRef = BackendManager.Database.RootReference.Child("UserData").Child(userId);

        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            ["_items/0"] = PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Coin],
            ["_items/1"] = PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoBlood],
            ["_items/2"] = PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.BoneCrystal]
        };

        userRef.UpdateChildrenAsync(updates).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError($"아이템 업데이트 실패 {task.Exception}");
            }
            if (task.IsCanceled)
            {
                Debug.Log($"아이템 업데이트 중단됨 {task.Exception}");
            }

            Debug.Log("소모한 아이템 갱신");
        });
    }

    // db에 레벨업 데이터 갱신
    private void UpdateLevelData(PlayerUnitData character)
    {
        string userID = BackendManager.Auth.CurrentUser.UserId;
        DatabaseReference characterRef = BackendManager.Database.RootReference
            .Child("UserData").Child(userID).Child("_unitDatas");

        characterRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("캐릭터 데이터 로딩 중 오류 발생: " + task.Exception);
                return;
            }

            DataSnapshot snapshot = task.Result;
            foreach (var childSnapshot in snapshot.Children)
            {
                if (int.Parse(childSnapshot.Child("_unitId").Value.ToString()) == character.UnitId)
                {
                    Dictionary<string, object> updates = new Dictionary<string, object>
                    {
                        ["_unitLevel"] = character.UnitLevel
                    };

                    childSnapshot.Reference.UpdateChildrenAsync(updates).ContinueWithOnMainThread(updateTask =>
                    {
                        if (updateTask.IsCompleted)
                        {
                            Debug.Log($"캐릭터 ID {character.UnitId}의 레벨이 {character.UnitLevel}로 업데이트됨");
                        }
                        else
                        {
                            Debug.LogError($"캐릭터 ID {character.UnitId} 업데이트 실패: " + updateTask.Exception);
                        }
                    });
                    break;
                }
            }
        });
    }

    // 외부 UI 갱신
    private void UpdateCharacters(PlayerUnitData character)
    {
        // 캐릭터 정보에 보여지는 레벨 갱신
        _characterPanel?.UpdateCharacterInfo(character);

        // 인벤토리 슬롯에 보여지는 레벨 갱신
        _inventoryPanel?.UpdateCharacterUI(character);
    }

    // 감소 버튼
    private void OnDecreaseButtonClick(PointerEventData eventData)
    {
        if (GetUI<Button>("DecreaseButton").interactable == false)
        {
            return;

        }
        if (_curLevelUp > 0)
        {
            _curLevelUp--;
            UpdateUI();
            GetUI<Slider>("LevelUpSlider").value = _curLevelUp;
        }

    }

    // 증가 버튼
    private void OnIncreaseButtonClick(PointerEventData eventData)
    {
        if (GetUI<Button>("IncreaseButton").interactable == false)
        {
            return;

        }
        if (_curLevelUp < _maxLevelUp && (_targetCharacter.UnitLevel + _curLevelUp) + 1 <= MAXLEVEL)
        {
            _curLevelUp++;
            UpdateUI();
            GetUI<Slider>("LevelUpSlider").value = _curLevelUp;
        }
    }

    // UI 아이템 숫자 갱신
    private void UpdateItemText(int newValue, string itemName)
    {
        GetUI<TextMeshProUGUI>($"{itemName}Text").text = $"{itemName} : {newValue}";
        CalculateMaxLevelUp();
        UpdateUI();
    }

    private void LoadItemImage(string imageName, E_Item itemType)
    {
        string itemPath = $"UI/item_{(int)itemType}";
        Sprite itemSprite = Resources.Load<Sprite>(itemPath);
        if (itemSprite != null)
        {
            GetUI<Image>(imageName).sprite = itemSprite;
        }
        else
        {
            Debug.LogWarning($"이미지 찾을 수 없음 {itemPath}");
        }
    }
}
