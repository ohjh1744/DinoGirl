using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitsDatas
{ 
    [SerializeField] public int Pos;
    [SerializeField] public float MaxHp;
    [SerializeField] public float Atk;
    [SerializeField] public float Def;
    [SerializeField] public int Level;
    [SerializeField] public int Id;
    [SerializeField] public string Elemente;
    [SerializeField] public List<Vector3Int> buffs;

}
