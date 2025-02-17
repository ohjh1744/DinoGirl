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

    // ������ ���� ����
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

    // �������� �ʿ��� ������ �䱸�� ���
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
                Debug.LogError($"������ �����͸� ã�� �� �����ϴ�. LevelUpID: {levelUpId}");
                return new RequiredItems();
            }
        }

        return items;
    }

    // ������ ����
    public void PerformLevelUp(PlayerUnitData character, int levels)
    {
        RequiredItems items = CalculateRequiredItems(character, levels);

        // PlayerDataManager���� ������ ����
        PlayerDataManager.Instance.PlayerData.SetItem((int)E_Item.Coin, PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Coin] - items.Coin);
        PlayerDataManager.Instance.PlayerData.SetItem((int)E_Item.DinoBlood, PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoBlood] - items.DinoBlood);
        PlayerDataManager.Instance.PlayerData.SetItem((int)E_Item.BoneCrystal, PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.BoneCrystal] - items.BoneCrystal);

        // ĳ������ ���� ����
        character.UnitLevel += levels;

        // PlayerDataManager�� UnitDatas ������Ʈ
        for (int i = 0; i < PlayerDataManager.Instance.PlayerData.UnitDatas.Count; i++)
        {
            if (PlayerDataManager.Instance.PlayerData.UnitDatas[i].UnitId == character.UnitId)
            {
                PlayerDataManager.Instance.PlayerData.UnitDatas[i] = character;
                break;
            }
        }

        // DB�� ����, ������ ����
        UpdateDatabase(character, items);
    }

    // ĳ���� csv�����Ϳ��� ��� ��ȯ
    private int GetRarity(int unitId)
    {
        return int.Parse(_characterData[unitId]["Rarity"]);
    }

    // ������ csv�����Ϳ��� ����� ������ ���� ID ��ȯ
    private int FindLevelUpId(int rarity, int level)
    {
        return _levelUpData.First(entry =>
            int.Parse(entry.Value["Rarity"]) == rarity &&
            int.Parse(entry.Value["Level"]) == level).Key;
    }

    // DB�� ������ ������ ����
    private void UpdateDatabase(PlayerUnitData character, RequiredItems items)
    {
        string userId = BackendManager.Instance.Auth.CurrentUser.UserId;
        DatabaseReference userRef = BackendManager.Instance.Database.RootReference.Child("UserData").Child(userId);

        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            ["_items/0"] = PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Coin],
            ["_items/1"] = PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoBlood],
            ["_items/2"] = PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.BoneCrystal]
        };

        DatabaseReference unitDatasRef = userRef.Child("_unitDatas");
        unitDatasRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                int index = 0;
                foreach (var childSnapshot in snapshot.Children)
                {
                    if (int.Parse(childSnapshot.Child("_unitId").Value.ToString()) == character.UnitId)
                    {
                        updates[$"_unitDatas/{index}/_unitLevel"] = character.UnitLevel;
                        break;
                    }
                    index++;
                }

                userRef.UpdateChildrenAsync(updates).ContinueWithOnMainThread(updateTask =>
                {
                    if (updateTask.IsFaulted)
                    {
                        Debug.LogError($"�����ͺ��̽� ������Ʈ ����: {updateTask.Exception}");
                    }
                    else
                    {
                        Debug.Log("�����ͺ��̽� ������Ʈ ����");
                    }
                });
            }
        });
    }
}