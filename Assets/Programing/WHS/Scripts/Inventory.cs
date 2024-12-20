using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

[System.Serializable] // 캐릭터 정보 ( 데이터베이스에서 받아올것)
public class Character
{
    public int ID;
    public string Name;
    public string Dinosaur;
    public string Job;
    public string Element;
    public string Skills;
    public string SkillsDiscription;
    public int Rare;

    public int level;
    public Sprite image;
}

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    private void Awake()
    {
        if (instance != null)
        {
            return;
        }

        instance = this;

        InitInventory();
    }

    // 아이템 딕셔너리
    public Dictionary<int, Item> items = new Dictionary<int, Item>();

    // 캐릭터 리스트
    public List<Character> characters = new List<Character>();


    // 재화 초기화 ( 서버에서 받아와야 할 것)
    private void InitInventory()
    {
        items = new Dictionary<int, Item>();

        AddItem(ItemID.Coin, 100000);
        AddItem(ItemID.DinoBlood, 50000);
        AddItem(ItemID.BoneCrystal, 2500);
        AddItem(ItemID.DinoStone, 30000);
        AddItem(ItemID.LeapStone, 300);
        AddItem(ItemID.Gear, 100);
    }

    // 재화 추가하기, 얻기
    public void AddItem(int itemID, int amount)
    {
        if (items.TryGetValue(itemID, out Item item))
        {
            item.amount += amount;
        }
        else
        {
            Item newItem = new Item { id = itemID, amount = amount };
            items[itemID] = newItem;
        }
        Debug.Log($"{itemID}가 {amount} 추가, {items[itemID].amount}");
    }

    // 재화 소모하기, 잃기
    public bool SpendItem(int itemId, int amount)
    {
        if(items == null || !items.ContainsKey(itemId))
        {
            Debug.Log("아이템이 없다");
            return false;
        }

        if (items.TryGetValue(itemId, out Item item) && item.amount >= amount)
        {
            item.amount -= amount;
            Debug.Log($"{itemId} 소모 : {amount}, 잔여 {item.amount}");

            return true;
        }

        Debug.Log($"아이템 ID {itemId}가 {amount - item.amount} 부족");
        return false;
    }

    // 현재 보유량 출력
    public int GetItemAmount(int itemId)
    {
        return items.TryGetValue(itemId, out Item item) ? item.amount : 0;
    }

    // 재화를 데이터베이스에 어떻게 저장해야 하나
}
