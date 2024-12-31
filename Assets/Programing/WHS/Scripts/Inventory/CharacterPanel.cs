using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterPanel : UIBInder
{
    private PlayerUnitData curCharacter;
    private GameObject levelUpPanel;

    private Dictionary<int, Dictionary<string, string>> characterData;

    private void Awake()
    {
        BindAll();
        AddEvent("LevelUpButton", EventType.Click, OnLevelUpButtonClick);

        Transform parent = GameObject.Find("CharacterPanel").transform;
        levelUpPanel = parent.Find("LevelUpPanel").gameObject;

        characterData = CsvDataManager.Instance.DataLists[(int)E_CsvData.Character];
    }

    private void Start()
    {
        UpdateCharacterInfo(curCharacter);
    }

    // 캐릭터 정보 갱신
    public void UpdateCharacterInfo(PlayerUnitData character)
    {
        curCharacter = character;
        int level = character.UnitLevel;
        if (characterData.TryGetValue(character.UnitId, out var data))
        {
            // TODO : 캐릭터의 각종 스탯 정보 ( 이름, ID 외 나머지 스텟 가져와야함)
            GetUI<TextMeshProUGUI>("UnitIdText").text = character.UnitId.ToString();
            GetUI<TextMeshProUGUI>("LevelText").text = character.UnitLevel.ToString();

            GetUI<TextMeshProUGUI>("NameText").text = data["Name"];
            GetUI<TextMeshProUGUI>("HPText").text = "HP : " + CalculateStat(TypeCastManager.Instance.TryParseInt(data["BaseHp"]), level);
            GetUI<TextMeshProUGUI>("AttackText").text = "Atk : " + CalculateStat(int.Parse(data["BaseATK"]), level);
            GetUI<TextMeshProUGUI>("DefText").text = "Def : " + CalculateStat(int.Parse(data["BaseDef"]), level);
            GetUI<TextMeshProUGUI>("ClassText").text = "Class : " + data["Class"];
            GetUI<TextMeshProUGUI>("ElementText").text = "Element : " + data["ElementName"];
            GetUI<TextMeshProUGUI>("GridText").text = "Grid : " + data["Grid"];
            GetUI<TextMeshProUGUI>("StatIdText").text = "StatID : " + data["StatID"];
            GetUI<TextMeshProUGUI>("PercentIncreaseText").text = "PI : " + data["PercentIncrease"];

            GetUI<Button>("LevelUpButton").interactable = (character.UnitLevel < 30);

            // db에 캐릭터 정보 갱신
            UpdateCharacterData(character);
        }
    }

    private void UpdateCharacterData(PlayerUnitData character)
    {
        // string userID = BackendManager.Auth.CurrentUser.UserId;
        string userID = "sinEKs9IWRPuWNbboKov1fKgmab2";
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
                        // ["_hp"] = CalculateStat(int.Parse(characterData[character.UnitId]["BaseHp"]), character.UnitLevel),
                        // ["_atk"] = CalculateStat(int.Parse(characterData[character.UnitId]["BaseATK"]), character.UnitLevel),
                        // ["_def"] = CalculateStat(int.Parse(characterData[character.UnitId]["BaseDef"]), character.UnitLevel)
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
        // TODO : 레벨에 따른 스텟 계산 추가
        return stat;
    }

}
