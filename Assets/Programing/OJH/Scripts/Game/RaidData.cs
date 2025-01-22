using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RaidData
{
    [SerializeField] private string _name; // 이름+ID

    public string Name { get { return _name; } set { _name = value; } }

    [SerializeField] private int _totalDamage; // 입힌 총 데미지

    public int TotalDamage { get { return _totalDamage; } set { _totalDamage = value; } }

    [SerializeField] public int Rank; // 순위
}
