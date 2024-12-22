using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(menuName = "WarriorSkill")]
public class RndWarriorSkill : RndSkillData
{
    [SerializeField] private int damage;
    public override void DoSkill()
    {
        // TODO : 전사 스킬의 사용 내용 
        Debug.Log($"전사의 스킬 사용 {damage}");
    }
}
