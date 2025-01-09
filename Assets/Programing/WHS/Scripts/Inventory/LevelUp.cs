using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelUp
{
    public const int MAXLEVEL = 30;
    private Dictionary<int, Dictionary<string, string>> _levelUpData;
    private Dictionary<int, Dictionary<string, string>> _characterData;


    public LevelUp(Dictionary<int, Dictionary<string, string>> levelUpData, Dictionary<int, Dictionary<string, string>> characterData)
    {
        _levelUpData = levelUpData;
        _characterData = characterData;
    }

    // 레벨업 가능 유무
    public bool CanLevelUp(PlayerUnitData character, int levels)
    {
        if (character.UnitLevel + levels > MAXLEVEL)
        {
            return false;
        }

        RequiredItems items = CalculateRequiredItems(character, levels);

        return PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Coin] >= items.Coin &&
               PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoBlood] >= items.DinoBlood &&
               PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.BoneCrystal] >= items.BoneCrystal;
    }

    // 레벨업에 필요한 아이템 요구량 계산
    public RequiredItems CalculateRequiredItems(PlayerUnitData character, int levels)
    {
        RequiredItems items = new RequiredItems();
        int rarity = GetRarity(character.UnitId);

        for (int i = 0; i < levels; i++)
        {
            int curLevel = character.UnitLevel + i + 1;
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

    // 레벨업 실행
    public void PerformLevelUp(PlayerUnitData character, int levels)
    {
        RequiredItems items = CalculateRequiredItems(character, levels);

        // PlayerDataManager에서 아이템 차감
        PlayerDataManager.Instance.PlayerData.SetItem((int)E_Item.Coin, PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Coin] - items.Coin);
        PlayerDataManager.Instance.PlayerData.SetItem((int)E_Item.DinoBlood, PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoBlood] - items.DinoBlood);
        PlayerDataManager.Instance.PlayerData.SetItem((int)E_Item.BoneCrystal, PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.BoneCrystal] - items.BoneCrystal);

        // 캐릭터의 레벨 증가
        character.UnitLevel += levels;

        // DB에 레벨, 아이템 갱신
        UpdateDatabase(character, items);
    }

    // 캐릭터 csv데이터에서 레어도 반환
    private int GetRarity(int unitId)
    {
        return int.Parse(_characterData[unitId]["Rarity"]);
    }

    // 레벨업 csv데이터에서 레어도와 레벨에 따른 ID 반환
    private int FindLevelUpId(int rarity, int level)
    {
        return _levelUpData.First(entry =>
            int.Parse(entry.Value["Rarity"]) == rarity &&
            int.Parse(entry.Value["Level"]) == level).Key;
    }

    // DB에 레벨과 아이템 갱신
    private void UpdateDatabase(PlayerUnitData character, RequiredItems items)
    {
        string userId = BackendManager.Auth.CurrentUser.UserId;
        DatabaseReference userRef = BackendManager.Database.RootReference.Child("UserData").Child(userId);

        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            ["_items/0"] = PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Coin],
            ["_items/1"] = PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoBlood],
            ["_items/2"] = PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.BoneCrystal],
            [$"_unitDatas/{character.UnitId}/_unitLevel"] = character.UnitLevel
        };

        userRef.UpdateChildrenAsync(updates).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError($"데이터베이스 업데이트 실패: {task.Exception}");
            }
            else
            {
                Debug.Log("데이터베이스 업데이트 성공");
            }
        });
    }
}