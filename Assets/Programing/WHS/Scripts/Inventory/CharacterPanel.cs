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

    private SceneChanger _sceneChanger;

    private int _index;
    private List<PlayerUnitData> _characterList;

    [SerializeField] private AudioClip _buttonClip;

    private void Awake()
    {
        BindAll();
        AddEvent("LevelUpButton", EventType.Click, OnLevelUpButtonClick);
        AddEvent("HomeButton", EventType.Click, GoLobby);
        AddEvent("PreviousCharacterButton", EventType.Click, PreviousButton);
        AddEvent("NextCharacterButton", EventType.Click, NextButton);
        AddEvent("SetMainCharacterButton", EventType.Click, SetMainCharacter);

        _characterData = CsvDataManager.Instance.DataLists[(int)E_CsvData.Character];
        _skillData = CsvDataManager.Instance.DataLists[(int)E_CsvData.CharacterSkill];

        _sceneChanger = FindObjectOfType<SceneChanger>();

        _characterList = PlayerDataManager.Instance.PlayerData.UnitDatas;

        /*
        GetUI<Button>("LevelUpButton").onClick.AddListener(() => SoundManager.Instance.PlaySFX(_buttonClip));
        GetUI<Button>("QuitButton").onClick.AddListener(() => SoundManager.Instance.PlaySFX(_buttonClip));
        GetUI<Button>("PreviousCharacterButton").onClick.AddListener(() => SoundManager.Instance.PlaySFX(_buttonClip));
        GetUI<Button>("NextCharacterButton").onClick.AddListener(() => SoundManager.Instance.PlaySFX(_buttonClip));
        GetUI<Button>("SetMainCharacterButton").onClick.AddListener(() => SoundManager.Instance.PlaySFX(_buttonClip));
        GetUI<Button>("LevelUpConfirm").onClick.AddListener(() => SoundManager.Instance.PlaySFX(_buttonClip));
        */
    }

    private void Start()
    {
        UpdateCharacterInfo(_curCharacter);
    }

    // 캐릭터 정보 갱신
    public void UpdateCharacterInfo(PlayerUnitData character)
    {
        _curCharacter = character;
        _index = _characterList.FindIndex(c => c.UnitId == character.UnitId);
        Debug.Log($"{_index} 현재 인덱스");

        int level = character.UnitLevel;
        if (_characterData.TryGetValue(character.UnitId, out var data))
        {
            // 캐릭터 이미지
            string imagePath = $"UI/LobbyMainUnit/LobbyMainUnit_{character.UnitId}";
            
            if (imagePath != null)
            {
                GetUI<Image>("CharacterImage").sprite = Resources.Load<Sprite>(imagePath);
            }

            // 레벨, 이름
            GetUI<TextMeshProUGUI>("LevelText").text = character.UnitLevel.ToString();
            GetUI<TextMeshProUGUI>("NameText").text = data["Name"];

            // 속성 아이콘 이미지 
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

            // 레어도에 따라 별 개수 ~5개 출력
            if (int.TryParse(data["Rarity"], out int rarity))
            {
                UpdateStar(rarity);
            }

            // 스킬 정보 가져오기
            UpdateSkill(character.UnitId);

            // 레벨에 따라 증가한 스탯
            GetUI<TextMeshProUGUI>("HPText").text = "HP : " + CalculateStat(int.Parse(data["BaseHp"]), level);
            GetUI<TextMeshProUGUI>("AttackText").text = "Atk : " + CalculateStat(int.Parse(data["BaseATK"]), level);
            GetUI<TextMeshProUGUI>("DefText").text = "Def : " + CalculateStat(int.Parse(data["BaseDef"]), level);

            GetUI<Button>("LevelUpButton").interactable = (character.UnitLevel < 30);

            UpdateCharacterData(character);
        }

        GetUI<Button>("SetMainCharacterButton").interactable = (_curCharacter.UnitId != PlayerDataManager.Instance.PlayerData.MainUnitID);
    }

    // DB에 캐릭터 정보 갱신
    private void UpdateCharacterData(PlayerUnitData character)
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
                            Debug.Log($"캐릭터 ID {character.UnitId}의 데이터가 업데이트됨");
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

    private void OnLevelUpButtonClick(PointerEventData eventData)
    {
        if (_curCharacter != null && _curCharacter.UnitLevel < 30)
        {
            BackButtonManager.Instance.OpenPanel(_levelUpPanel);

            LevelUpPanel levelUp = _levelUpPanel.GetComponent<LevelUpPanel>();
            levelUp.Init(_curCharacter);
        }
    }

    // 레벨당 스탯 계산
    private int CalculateStat(int baseStat, int level)
    {
        // Character 시트에서 "Increase"에 따라 해당하는 배율만큼 레벨마다 합증가

        if (!_characterData.TryGetValue(_curCharacter.UnitId, out var data))
        {
            Debug.LogError($"캐릭터 데이터를 찾을 수 없음 {_curCharacter.UnitId}");
            return baseStat;
        }

        if (!int.TryParse(data["Increase"], out int increase))
        {
            Debug.LogError($"Increase 값 찾을 수 없음 {_curCharacter.UnitId}");
            return baseStat;
        }

        int levelIncrease = level - 1;
        float totalIncrease = 1 + (increase * levelIncrease / 100f); // 1.n배 

        return Mathf.FloorToInt(baseStat * totalIncrease);
    }

    private void UpdateSkill(int unitId)
    {
        foreach (var value in _skillData.Values)
        {
            if (int.Parse(value["CharID"]) == unitId)
            {
                GetUI<TextMeshProUGUI>("SkillNameText").text = value["SkillName"];
                GetUI<TextMeshProUGUI>("CoolDownText").text = $"쿨타임: {value["Cooldown"]}초";
                GetUI<TextMeshProUGUI>("SkillDescriptionText").text = value["SkillDescription"];
                return;
            }
        }

        Debug.LogWarning($"스킬 정보를 찾을 수 없음: CharID {unitId}");
    }

    private void PreviousButton(PointerEventData eventData)
    {
        // 이전 캐릭터 정보로 이동
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
        // 다음 캐릭터 정보로 이동
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

    private void GoLobby(PointerEventData eventData)
    {
        _sceneChanger.CanChangeSceen = true;
        _sceneChanger.ChangeScene("Lobby_OJH");
    }

    // 메인 캐릭터로 설정
    private void SetMainCharacter(PointerEventData eventData)
    {
        if (GetUI<Button>("SetMainCharacterButton").interactable == false) return;

        PlayerDataManager.Instance.PlayerData.MainUnitID = _curCharacter.UnitId;

        string userId = BackendManager.Auth.CurrentUser.UserId;
        DatabaseReference userRef = BackendManager.Database.RootReference.Child("UserData").Child(userId);

        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            ["_mainUnitID"] = PlayerDataManager.Instance.PlayerData.MainUnitID
        };

        userRef.UpdateChildrenAsync(updates).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("대표 캐릭터가 성공적으로 업데이트되었습니다.");
            }
            else
            {
                Debug.LogError("대표 캐릭터 업데이트 실패: " + task.Exception);
            }
        });

        GetUI<Button>("SetMainCharacterButton").interactable = (_curCharacter.UnitId != PlayerDataManager.Instance.PlayerData.MainUnitID);
    }
}
