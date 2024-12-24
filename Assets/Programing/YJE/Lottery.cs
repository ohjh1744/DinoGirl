using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ItemG와 CharaterG에 맞는 UI프리팹 용 오브젝트 풀 생성 필요 - 각 10개씩

[System.Serializable]
public class Lottery
{
    private int id; // 각 품목 ID
    private int probability; // 확률
}

[System.Serializable]
public class ItemG
{
    private int id;
    private string name;
    private Sprite sprite;
    private int count;
}

[System.Serializable]
public class CharaterG
{
    private int id;
    private string name;
    private Sprite sprite;
    private int rarity;
}
