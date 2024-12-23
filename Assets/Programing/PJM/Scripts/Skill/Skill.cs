using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Skill : ScriptableObject
{
    public string skillName;
    public float skillRange;
    public float cooldown;
    protected Transform skillTarget;
    public Transform SkillTarget { get => skillTarget; protected set => skillTarget = value; }
    protected List<Transform> skillTargets;
    public List<Transform> SkillTargets { get => skillTargets; protected set => skillTargets = value; }

    // 거리 체크
    protected virtual bool CheckRange(Transform caster)
    {
        if (SkillTarget == null)
            return false;
        float sqrDistance = (SkillTarget.position - caster.position).sqrMagnitude;
        return sqrDistance <= skillRange * skillRange;
    }

    // 타겟 설정
    protected abstract bool SetTarget(Transform caster, LayerMask enemyLayer, bool isPriorityTargetFar);

    // 스킬 실행
    protected abstract void Perform(Transform caster);

    protected virtual void ResetTargets()
    {
        SkillTargets.Clear();
    }

    // 스킬 행동 트리를 반환하는 메서드
    public SequenceNode CreateSkillBTree(Transform caster, LayerMask enemyLayer, bool isPriorityTargetFar)
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

