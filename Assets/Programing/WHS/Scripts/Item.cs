using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 아이템들 ( 시트에서 받아와야 하나)
public class Item
{
    public int ItemID;          // ID 500~
    public string ItemName;     // 이름
    public int amount;          // 보유량
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