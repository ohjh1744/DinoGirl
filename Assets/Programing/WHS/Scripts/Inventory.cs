using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public enum CurrencyType
{
    DinoStone,      // [영혼석]   가챠 재화 ( 젬, 청휘석 )
    Coin,           // [골드]     공통 재화 ( 골드, 크레딧 )
    DinoBlood,      // [경험치]   캐릭터 레벨업 메인 재료 ( 경험치, 보고서, 혼 )
    BoneCrystal,    // [아이템]   캐릭터 혹은 장비 강화 ( 강화석, 망치 )
}

[System.Serializable]
public class Currency
{
    public CurrencyType type;   // 재화 타입
    public int amount;          // 보유량
    public Sprite icon;         // UI에 표시될 아이콘
}

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
    }

    // 재화 리스트
    public List<Currency> currencies = new List<Currency>();

    // 캐릭터 리스트
    public List<Character> characters = new List<Character>();

    // 재화 추가하기, 얻기
    public void AddCurrency(CurrencyType type, int amount)
    {
        Currency currency = currencies.Find(x => x.type == type);
        if (currency != null)
        {
            // 재화 보유량 증가
            currency.amount += amount;
            Debug.Log($"{type} {amount} 추가됨. 보유 {currency.amount}");
        }
    }

    // 재화 소모하기, 잃기
    public bool SpendCurrency(CurrencyType type, int amount)
    {
        Currency currency = currencies.Find(x => x.type == type);

        if (currency != null && currency.amount >= amount)
        {
            currency.amount -= amount;
            Debug.Log($"{type} 재화 소모 : {amount}, 잔여 {currency.amount}");
            return true;
        }
        Debug.Log($"재화 부족 {currency.amount - amount}");
        return false;
    }

    // 현재 보유량 출력
    public int GetCurrencyAmount(CurrencyType type)
    {
        Currency currency = currencies.Find(x => x.type == type);
        return currency != null ? currency.amount : 0;
    }

    // 재화를 데이터베이스에 어떻게 저장해야 하나
}
