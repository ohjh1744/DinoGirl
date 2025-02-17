using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterPanel : UIBInder
{
    private PlayerUnitData _curCharacter;
    [SerializeField] private GameObject _levelUpPanel;

    private Dictionary<int, Dictionary<string, string>> _characterData;
    private Dictionary<int, Dictionary<string, string>> _skillData;

    private int _index;
    private List<PlayerUnitData> _characterList;

    private void Awake()
    {
        BindAll();
        AddEvent("LevelUpButton", EventType.Click, OnLevelUpButtonClick);
        AddEvent("PreviousCharacterButton", EventType.Click, PreviousButton);
        AddEvent("NextCharacterButton", EventType.Click, NextButton);
        AddEvent("SetMainCharacterButton", EventType.Click, SetMainCharacter);

        _characterData = CsvDataManager.Instance.DataLists[(int)E_CsvData.Character];
        _skillData = CsvDataManager.Instance.DataLists[(int)E_CsvData.CharacterSkill];

        _characterList = PlayerDataManager.Instance.PlayerData.UnitDatas;
    }

    private void Start()
    {
        UpdateCharacterInfo(_curCharacter);
    }

    // ĳ���� ���� ����
    public void UpdateCharacterInfo(PlayerUnitData character)
    {
        _curCharacter = character;
        _index = _characterList.FindIndex(c => c.UnitId == character.UnitId);
        Debug.Log($"{_index} ���� �ε���");

        // ���� ĳ������ ����
        int level = character.UnitLevel;

        if (_characterData.TryGetValue(character.UnitId, out var data))
        {
            // ĳ���� �̹���
            string imagePath = $"LobbyMainUnit/LobbyMainUnit_{character.UnitId}";            
            if (imagePath != null)
            {
                GetUI<Image>("CharacterImage").sprite = Resources.Load<Sprite>(imagePath);
            }
            else
            {
                Debug.LogWarning($"�̹����� ã�� �� ����: {imagePath}");
            }


            // ����, �̸�
            GetUI<TextMeshProUGUI>("LevelText").text = character.UnitLevel.ToString();
            GetUI<TextMeshProUGUI>("NameText").text = data["Name"];

            // �Ӽ� ������ �̹��� 
            if (int.TryParse(data["ElementID"], out int elementId))
            {
                string elementPath = $"Element/element_{elementId}";
                Sprite elementSprite = Resources.Load<Sprite>(elementPath);
                if (elementSprite != null)
                {
                    GetUI<Image>("ElementImage").sprite = elementSprite;
                }
                else
                {
                    Debug.LogWarning($"�̹����� ã�� �� ����: {elementPath}");
                }
            }

            // ����� ���� �� ���� ~5�� ���
            if (int.TryParse(data["Rarity"], out int rarity))
            {
                UpdateStar(rarity);
            }

            // ��ų ���� ��������
            UpdateSkill(character.UnitId);

            // ������ ���� ������ ����
            GetUI<TextMeshProUGUI>("HPText").text = "HP : " + CalculateStat(int.Parse(data["BaseHp"]), level);
            GetUI<TextMeshProUGUI>("AttackText").text = "Atk : " + CalculateStat(int.Parse(data["BaseATK"]), level);
            GetUI<TextMeshProUGUI>("DefText").text = "Def : " + CalculateStat(int.Parse(data["BaseDef"]), level);

            GetUI<Button>("LevelUpButton").interactable = (character.UnitLevel < 30);

            UpdateCharacterData(character);
        }

        GetUI<Button>("SetMainCharacterButton").interactable = (_curCharacter.UnitId != PlayerDataManager.Instance.PlayerData.MainUnitID);
    }

    // DB�� ĳ���� ���� ����
    private void UpdateCharacterData(PlayerUnitData character)
    {
        string userID = BackendManager.Instance.Auth.CurrentUser.UserId;
        DatabaseReference characterRef = BackendManager.Instance.Database.RootReference
            .Child("UserData").Child(userID).Child("_unitDatas");

        characterRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("ĳ���� ������ �ε� �� ���� �߻�: " + task.Exception);
                return;
            }

            DataSnapshot snapshot = task.Result;
            foreach (DataSnapshot childSnapshot in snapshot.Children)
            {
                if (int.Parse(childSnapshot.Child("_unitId").Value.ToString()) == character.UnitId)
                {
                    Dictionary<string, object> updates = new Dictionary<string, object>
                    {
                        ["_unitLevel"] = character.UnitLevel,
                    };

                    childSnapshot.Reference.UpdateChildrenAsync(updates).ContinueWithOnMainThread(updateTask =>
                    {
                        if (updateTask.IsCompleted)
                        {
                            Debug.Log($"ĳ���� ID {character.UnitId}�� �����Ͱ� ������Ʈ��");
                        }
                        else
                        {
                            Debug.LogError($"ĳ���� ID {character.UnitId} ������Ʈ ����: " + updateTask.Exception);
                        }
                    });

                    break;
                }
            }
        });
    }

    private void OnLevelUpButtonClick(PointerEventData eventData)
    {
        if (_curCharacter != null && _curCharacter.UnitLevel < 30)
        {
            BackButtonManager.Instance.OpenPanel(_levelUpPanel);

            LevelUpPanel levelUp = _levelUpPanel.GetComponent<LevelUpPanel>();
            levelUp.Init(_curCharacter);
        }
    }

    // ������ ���� ���
    private int CalculateStat(int baseStat, int level)
    {
        // Character ��Ʈ���� "Increase"�� ���� �ش��ϴ� ������ŭ �������� ������

        if (!_characterData.TryGetValue(_curCharacter.UnitId, out var data))
        {
            Debug.LogError($"ĳ���� �����͸� ã�� �� ���� {_curCharacter.UnitId}");
            return baseStat;
        }

        if (!int.TryParse(data["Increase"], out int increase))
        {
            Debug.LogError($"Increase �� ã�� �� ���� {_curCharacter.UnitId}");
            return baseStat;
        }

        int levelIncrease = level - 1;
        float totalIncrease = 1 + (increase * levelIncrease / 100f); // 1.n�� 

        return Mathf.FloorToInt(baseStat * totalIncrease);
    }

    // ��ų ���� �ؽ�Ʈ
    private void UpdateSkill(int unitId)
    {
        foreach (var value in _skillData.Values)
        {
            if (int.Parse(value["CharID"]) == unitId)
            {
                GetUI<TextMeshProUGUI>("SkillNameText").text = value["SkillName"];
                GetUI<TextMeshProUGUI>("CoolDownText").text = $"��Ÿ�� : {value["Cooldown"]}��";
                GetUI<TextMeshProUGUI>("SkillDescriptionText").text = value["SkillDescription"];
                return;
            }
        }

        Debug.LogWarning($"��ų ������ ã�� �� ����: CharID {unitId}");
    }

    private void PreviousButton(PointerEventData eventData)
    {
        // ���� ĳ���� ������ �̵�
        if (_index > 0)
        {
            _index--;
        }
        else
        {
            _index = _characterList.Count - 1;
        }

        UpdateCharacterInfo(_characterList[_index]);
    }

    private void NextButton(PointerEventData eventData)
    {
        // ���� ĳ���� ������ �̵�
        if (_index < _characterList.Count - 1)
        {
            _index++;
        }
        else
        {
            _index = 0;
        }

        UpdateCharacterInfo(_characterList[_index]);
    }

    private void UpdateStar(int rarity)
    {
        // rarity�� ���� �� ������ ���
        for (int i = 0; i < 5; i++)
        {
            Image starImage = GetUI<Image>($"Star_{i + 1}");
            if (starImage != null)
            {
                if (i < rarity)
                {
                    starImage.gameObject.SetActive(true);
                    starImage.sprite = Resources.Load<Sprite>("Element/icon_star");
                }
                else
                {
                    starImage.gameObject.SetActive(false);
                }
            }
        }
    }

    // ���� ĳ���ͷ� ����
    private void SetMainCharacter(PointerEventData eventData)
    {
        if (GetUI<Button>("SetMainCharacterButton").interactable == false) return;

        PlayerDataManager.Instance.PlayerData.MainUnitID = _curCharacter.UnitId;

        string userId = BackendManager.Instance.Auth.CurrentUser.UserId;
        DatabaseReference userRef = BackendManager.Instance.Database.RootReference.Child("UserData").Child(userId);

        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            ["_mainUnitID"] = PlayerDataManager.Instance.PlayerData.MainUnitID
        };

        userRef.UpdateChildrenAsync(updates).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("��ǥ ĳ���Ͱ� ���������� ������Ʈ�Ǿ����ϴ�.");
            }
            else
            {
                Debug.LogError("��ǥ ĳ���� ������Ʈ ����: " + task.Exception);
            }
        });

        GetUI<Button>("SetMainCharacterButton").interactable = (_curCharacter.UnitId != PlayerDataManager.Instance.PlayerData.MainUnitID);
    }
}
