using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 기본적으로 스킬을 실행할 때 모두가 필요로 할 함수를 가상함수로 제작
/// 각 스킬 분류(단일딜/광역딜/단일힐/광역힐)에 따라 새로 제작하는 Scriptable Object에 
/// 상속하여 사용
/// </summary>
public class SkillBaseSO : ScriptableObject
{
    public virtual void DoSkill(int damage, List<GameObject> target, GameObject unit)
    {

    }
    public virtual void DoAnimationSkill()
    {

    }
    public virtual void DoSoundSkill()
    {

    }
}
