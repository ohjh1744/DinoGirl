using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class UnitStat : MonoBehaviour
{
    [SerializeField] public float MaxHp;
    [SerializeField] public float Atk;
    [SerializeField] public float Def;
    [SerializeField] public int Level;
    [SerializeField] public int Id;
    [SerializeField] public string Elemente;
    [SerializeField] public List<Vector3Int> buffs;


    public void setStats(float maxhp, float atk, float def, int level, int id, string elemente)
    {
        MaxHp = maxhp;
        Atk = atk;
        Def = def;
        Level = level;
        Id = id;
        Elemente = elemente;
       
    }

}
