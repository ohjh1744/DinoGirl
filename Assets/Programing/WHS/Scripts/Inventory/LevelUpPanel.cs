using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelUpPanel : UIBInder
{
    private PlayerUnitData targetCharacter;
    private int maxLevelUp;
    private int curLevelUp;
    private const int MAXLEVEL = 30;

    private Dictionary<int, Dictionary<string, string>> levelUpData;

    private struct RequiredItems
    {
        public int coin;
        public int dinoBlood;
        public int boneCrystal;
    }

    private void Awake()
    {
        BindAll();
        AddEvent("LevelUpConfirm", EventType.Click, OnConfirmButtonClick);
        GetUI<Slider>("LevelUpSlider").onValueChanged.AddListener(OnSliderValueChanged);
        AddEvent("DecreaseButton", EventType.Click, OnDecreaseButtonClick);
        AddEvent("IncreaseButton", EventType.Click, OnIncreaseButtonClick);

        levelUpData = CsvDataManager.Instance.DataLists[(int)E_CsvData.CharacterLevelUp];
        Debug.Log($"levelUpData count: {levelUpData.Count}");
        foreach (var key in levelUpData.Keys)
        {
            Debug.Log($"Key: {key}, Value: {levelUpData[key]["500"]}, {levelUpData[key]["501"]}, {levelUpData[key]["502"]}");
        }
    }

    private void OnEnable()
    {
        if (PlayerDataManager.Instance.PlayerData.OnItemChanged == null)
        {
            PlayerDataManager.Instance.PlayerData.OnItemChanged = new UnityAction<int>[System.Enum.GetValues(typeof(E_Item)).Length];
        }

        PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.Coin] += UpdateCoinText;
        PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.DinoBlood] += UpdateDinoBloodText;
        PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.BoneCrystal] += UpdateBoneCrystalText;
    }

    private void OnDisable()
    {
        if (PlayerDataManager.Instance != null && PlayerDataManager.Instance.PlayerData != null)
        {
            PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.Coin] -= UpdateCoinText;
            PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.DinoBlood] -= UpdateDinoBloodText;
            PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.BoneCrystal] -= UpdateBoneCrystalText;
        }
    }


    public void Initialize(PlayerUnitData character)
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
        RequiredItems items = CalculateRequiredItems(curLevelUp);

        GetUI<TextMeshProUGUI>("CoinText").text = $"Coin : {items.coin}";
        GetUI<TextMeshProUGUI>("DinoBloodText").text = $"DinoBlood : {items.dinoBlood}";
        GetUI<TextMeshProUGUI>("BoneCrystalText").text = $"BoneCrystal : {items.boneCrystal}";

        if (targetCharacter.UnitLevel + curLevelUp >= MAXLEVEL)
        {
            GetUI<TextMeshProUGUI>("LevelText").text = $"Lv.{targetCharacter.UnitLevel} -> Lv.{MAXLEVEL} (MAX)";
        }
        else
        {
            GetUI<TextMeshProUGUI>("LevelText").text = $"Lv.{targetCharacter.UnitLevel} -> Lv.{targetCharacter.UnitLevel + curLevelUp}";
        }
    }

    // 레벨업 할 최대치
    private void CalculateMaxLevelUp()
    {
        maxLevelUp = 0;
        while (CanLevelUp(maxLevelUp + 1)
            && (targetCharacter.UnitLevel + maxLevelUp + 1) <= MAXLEVEL)
        {
            maxLevelUp++;
        }
    }

    // 레벨업 가능 여부
    private bool CanLevelUp(int level)
    {
        if (targetCharacter.UnitLevel + level > MAXLEVEL)
        {
            return false;
        }

        RequiredItems items = CalculateRequiredItems(level);

        // 레벨업에 충분한 양을 가지고 있으면 true
        return PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Coin] >= items.coin &&
               PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoBlood] >= items.dinoBlood &&
               PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.BoneCrystal] >= items.boneCrystal;
    }

    // 요구 재화량 계산
    private RequiredItems CalculateRequiredItems(int level)
    {
        RequiredItems items = new RequiredItems();  

        for (int i = 0; i < level; i++)
        {
            // TODO : LevelUpID나 레어도에 따라 수정 필요
            int levelUpId = 4000 + ((targetCharacter.UnitLevel + i) * 10);

            if (levelUpData.TryGetValue(levelUpId, out Dictionary<string, string> data))
            {
                items.coin += int.Parse(data["500"].Replace(",", ""));
                items.dinoBlood += int.Parse(data["501"].Replace(",", ""));
                if (data.ContainsKey("502") && !string.IsNullOrEmpty(data["502"]))
                {
                    items.boneCrystal += int.Parse(data["502"].Replace(",", ""));
                }
            }
            else
            {
                Debug.LogError($"레벨업 데이터를 찾을 수 없습니다. LevelUpID: {levelUpId}");
            }
        }
        return items;
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
    private bool LevelUp(PlayerUnitData character)
    {
        RequiredItems items = CalculateRequiredItems(1);

        if (PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Coin] >= items.coin &&
            PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoBlood] >= items.dinoBlood &&
            PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.BoneCrystal] >= items.boneCrystal)
        {
            PlayerDataManager.Instance.PlayerData.SetItem((int)E_Item.Coin, PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Coin] - items.coin);
            PlayerDataManager.Instance.PlayerData.SetItem((int)E_Item.DinoBlood, PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoBlood] - items.dinoBlood);
            PlayerDataManager.Instance.PlayerData.SetItem((int)E_Item.BoneCrystal, PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.BoneCrystal] - items.boneCrystal);

            // TODO : 데이터베이스에 소모한 아이템 업데이트

            character.UnitLevel++;
            Debug.Log($"{character.UnitId} 레벨업 {character.UnitLevel}");
            if (items.boneCrystal > 0)
            {
                Debug.Log($"본 크리스탈 {items.boneCrystal} 소모");
            }

            UpdateCharacters(character);
            UpdateLevelData(character);

            return true;
        }

        if (PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Coin] < items.coin)
        {
            Debug.Log($"코인이 {items.coin - PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Coin]} 부족합니다.");
        }
        if (PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoBlood] < items.dinoBlood)
        {
            Debug.Log($"다이노블러드가 {items.dinoBlood - PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoBlood]} 부족합니다");
        }
        if (PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.BoneCrystal] < items.boneCrystal)
        {
            Debug.Log($"본크리스탈이 {items.boneCrystal - PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.BoneCrystal]} 부족합니다");
        }

        return false;
    }

    // db 레벨업 데이터 갱신
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

    // UI 갱신
    private void UpdateCharacters(PlayerUnitData character)
    {
        CharacterPanel characterPanel = FindObjectOfType<CharacterPanel>();
        if (characterPanel != null)
        {
            characterPanel.UpdateCharacterInfo(character);
        }

        InventoryPanel inventoryPanel = FindObjectOfType<InventoryPanel>();
        if (inventoryPanel != null)
        {
            inventoryPanel.UpdateCharacterUI(character);
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
        if (curLevelUp < maxLevelUp && (targetCharacter.UnitLevel + curLevelUp) + 1 <= MAXLEVEL)
        {
            curLevelUp++;
            UpdateUI();
            GetUI<Slider>("LevelUpSlider").value = curLevelUp;
        }
    }

    private void UpdateCoinText(int newValue)
    {
        GetUI<TextMeshProUGUI>("CoinText").text = $"Coin : {newValue}";
        CalculateMaxLevelUp();
        UpdateUI();
    }

    private void UpdateDinoBloodText(int newValue)
    {
        GetUI<TextMeshProUGUI>("DinoBloodText").text = $"DinoBlood : {newValue}";
        CalculateMaxLevelUp();
        UpdateUI();
    }

    private void UpdateBoneCrystalText(int newValue)
    {
        GetUI<TextMeshProUGUI>("BoneCrystalText").text = $"BoneCrystal : {newValue}";
        CalculateMaxLevelUp();
        UpdateUI();
    }
}
