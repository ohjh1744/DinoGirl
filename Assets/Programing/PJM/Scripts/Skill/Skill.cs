using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Skill : ScriptableObject
{
    public string skillName;
    public float skillRange;
    public float cooldown;
    protected Transform skillTarget; 

    // 거리 체크
    public virtual bool CheckRange(Transform caster)
    {
        if (skillTarget == null)
            return false;
        float sqrDistance = (skillTarget.position - caster.position).sqrMagnitude;
        return sqrDistance <= skillRange * skillRange;
    }

    // 타겟 설정
    public abstract bool SetTarget(Transform caster, LayerMask enemyLayer, bool isPriorityTargetFar);

    // 스킬 실행
    public abstract void Perform(Transform caster);

    // 스킬 행동 트리를 반환하는 메서드
    public SequenceNode CreateSkillTree(Transform caster, LayerMask enemyLayer, bool isPriorityTargetFar)
    {
        return new SequenceNode
        (
            new List<BaseNode>()
            {
                new ConditionNode(() => SetTarget(caster, enemyLayer, isPriorityTargetFar)), // 타겟 설정 및 확인
                new ConditionNode(() => CheckRange(caster)),           // 거리 체크
                new ActionNode(() =>                               // 스킬 실행
                {
                    Perform(caster);
                    return BaseNode.ENodeState.Success;
                })
            }
        );
    }
}

