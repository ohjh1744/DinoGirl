using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ItemG와 CharaterG에 맞는 UI프리팹 용 오브젝트 풀 생성 필요 - 각 10개씩
/// <summary>
/// 가챠 데이터 테이블 수정 시 요청할 사항
/// 1. Gacha 시트에서 Count 제거 후 GachaReturn 시트에서 Count 사용 가능할지 확인
/// 2. GachaReturn 시트에서 CharID와 ItemID를 통일해서 GachaID로 사용하되 내용은 CharID와 ItemID로 설정
///    이후 Check(?)로 캐릭터인 경우 0 / 아이템인 경우 1 로 표현이 가능할지 확인
/// </summary>

[System.Serializable]
public class Lottery
{
    private int id; // 각 품목 ID
    public int Id { get { return id; } set { id = value; } }
    private int probability; // 확률
    public int Probability { get { return probability; } set { probability = value; } }

}

[System.Serializable]
public class ItemG
{
    private int id;
    public int ID { get { return id; } set { id = value; }}
    private string name;
    public string Name { get { return name; } set { name = value; } }
    private Sprite sprite;
    public Sprite Sprite { get { return sprite; } set { sprite = value; } }
    private int count;
    public int Count { get { return count; } set { count = value; } }
}

[System.Serializable]
public class CharaterG
{
    private int id;
    public int ID { get { return id; } set { id = value; } }
    private string name;
    public string Name { get { return name; } set { name = value; } }
    private Sprite sprite;
    public Sprite Sprite { get { return sprite; } set { sprite = value; } }
    private int rarity;
    public int Rarity { get { return rarity; } set { rarity = value; } }
}
