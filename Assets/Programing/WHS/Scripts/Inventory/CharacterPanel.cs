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
    private PlayerUnitData curCharacter;
    private GameObject levelUpPanel;

    private Dictionary<int, Dictionary<string, string>> characterData;

    private SceneChanger _sceneChanger;

    private int index;
    private List<PlayerUnitData> characterList;

    private void Awake()
    {
        BindAll();
        AddEvent("LevelUpButton", EventType.Click, OnLevelUpButtonClick);
        AddEvent("HomeButton", EventType.Click, GoLobby);
        AddEvent("PreviousCharacterButton", EventType.Click, PreviousButton);
        AddEvent("NextCharacterButton", EventType.Click, NextButton);

        Transform parent = GameObject.Find("CharacterPanel").transform;
        levelUpPanel = parent.Find("LevelUpPanel").gameObject;

        characterData = CsvDataManager.Instance.DataLists[(int)E_CsvData.Character];

        _sceneChanger = FindObjectOfType<SceneChanger>();

        characterList = PlayerDataManager.Instance.PlayerData.UnitDatas;
    }

    private void Start()
    {
        UpdateCharacterInfo(curCharacter);
    }

    // 캐릭터 정보 갱신
    public void UpdateCharacterInfo(PlayerUnitData character)
    {
        curCharacter = character;
        index = characterList.FindIndex(c => c.UnitId == character.UnitId);
        Debug.Log($"{index} 현재 인덱스");

        int level = character.UnitLevel;
        if (characterData.TryGetValue(character.UnitId, out var data))
        {
            // TODO : 캐릭터의 각종 스탯 정보 ( 레벨에 따른 스탯, 이미지 )

            // 캐릭터 이미지
            string path = $"Portrait/portrait_{character.UnitId}";
            if (path != null)
            {
                GetUI<Image>("CharacterImage").sprite = Resources.Load<Sprite>(path);
            }

            // 레벨, 이름
            // GetUI<TextMeshProUGUI>("LevelText").text = character.UnitLevel.ToString();
            GetUI<TextMeshProUGUI>("NameText").text = data["Name"];

            // TODO : 속성 아이콘 이미지 
            GetUI<Image>("ElementImage").sprite = null;

            // 레어도에 따라 별 개수 ~5개 출력
            if (int.TryParse(data["Rarity"], out int rarity))
            {
                UpdateStar(rarity);
            }

            // TODO : 스킬 정보 가져오기
            GetUI<TextMeshProUGUI>("SkillNameText").text = "스킬 이름";
            GetUI<TextMeshProUGUI>("CoolDownText").text = "쿨타임";
            GetUI<TextMeshProUGUI>("SkillDescriptionText").text = "적에게 창을 던져 물리 피해를 입힙니다";

            // TODO : 레벨에 따라 증가한 스탯
            GetUI<TextMeshProUGUI>("HPText").text = "HP : " + CalculateStat(TypeCastManager.Instance.TryParseInt(data["BaseHp"]), level);
            GetUI<TextMeshProUGUI>("AttackText").text = "Atk : " + CalculateStat(int.Parse(data["BaseATK"]), level);
            GetUI<TextMeshProUGUI>("DefText").text = "Def : " + CalculateStat(int.Parse(data["BaseDef"]), level);

            GetUI<Button>("LevelUpButton").interactable = (character.UnitLevel < 30);

            // db에 캐릭터 정보 갱신
            UpdateCharacterData(character);
        }
    }

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
            foreach (var childSnapshot in snapshot.Children)
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
        if (curCharacter != null && curCharacter.UnitLevel < 30)
        {
            levelUpPanel.gameObject.SetActive(true);

            LevelUpPanel levelUp = levelUpPanel.GetComponent<LevelUpPanel>();
            levelUp.Init(curCharacter);
        }
    }

    private int CalculateStat(int stat, int level)
    {
        // TODO : 레벨에 따른 스텟 계산
        return stat;
    }

    private void PreviousButton(PointerEventData eventData)
    {
        // 이전 캐릭터 정보로 이동
        if (index > 0)
        {
            index--;
        }
        else
        {
            index = characterList.Count - 1;
        }

        UpdateCharacterInfo(characterList[index]);
    }

    private void NextButton(PointerEventData eventData)
    {
        // 다음 캐릭터 정보로 이동
        if (index < characterList.Count - 1)
        {
            index++;
        }
        else
        {
            index = 0;
        }

        UpdateCharacterInfo(characterList[index]);
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
}
