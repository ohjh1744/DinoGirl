using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChar : MonoBehaviour
{
    [SerializeField] private UnitSO stats;

    private int charId;
    private int skillId;
    private string name;
    private float hp;
    private float ATK;
    private float DEF;
    private float coolTime;

    private void Awake()
    {
        charId = stats.charId;
        skillId = stats.skillId;
        name = stats.name;
        hp = stats.hp;
        ATK = stats.ATK;
        DEF = stats.DEF;
        coolTime = stats.coolTime;
    }
}
