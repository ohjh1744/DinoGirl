using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public int id;          // ID 500~
    public string name;     // 이름
    public int amount;      // 보유량
    public Sprite icon;     // UI에 표시될 아이콘?
}

public static class ItemID
{
    public const int Coin = 500;
    public const int DinoBlood = 501;
    public const int BoneCrystal = 502;
    public const int DinoStone = 503;
    public const int LeapStone = 504;
    public const int Gear = 505;
}