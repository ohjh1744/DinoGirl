using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class UnitStat : MonoBehaviour   
{
    [SerializeField] public int Pos;
    [SerializeField] public int MaxHp;
    [SerializeField] public int Atk;
    [SerializeField] public int Def;
    [SerializeField] public int Level;
    [SerializeField] public int Id;
    [SerializeField] public string Elemente;
    [SerializeField] public List<Vector3Int> buffs;


    
    public void setStats(int pos , int maxhp, int atk, int def, int level, int id, string elemente)
    {
        Pos = pos;
        MaxHp = maxhp;
        Atk = atk;
        Def = def;
        Level = level;
        Id = id;
        Elemente = elemente;
       
    }

}
