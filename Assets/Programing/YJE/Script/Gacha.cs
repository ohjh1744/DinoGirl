using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 뽑기 리스트를 제작하기 위한 타입 선언
public class Gacha : MonoBehaviour
{
    private int check;
    public int Check { get { return check; } set { check = value; } }
    private int charId;
    public int CharId { get { return charId; } set { charId = value; } }
    private int itemId;
    public int ItemId { get { return itemId; } set { itemId = value; } }
    private int probability;
    public int Probability { get { return probability; } set { probability = value; } }
    private int count;
    public int Count { get { return count; } set { count = value; } }
}
