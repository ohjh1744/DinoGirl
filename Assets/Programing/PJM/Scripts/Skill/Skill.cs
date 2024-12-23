using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Skill : ScriptableObject
{
    [SerializeField] private string _skillName;
    public string SkillName {get => _skillName; set => _skillName = value; }
    
    [SerializeField] private float _skillRange;
    public float SkillRange {get => _skillRange; protected set => _skillRange = value; }
    
    [SerializeField] private int skillRatio;
    public int SkillRatio {get => skillRatio; protected set => skillRatio = value; }
    [SerializeField] private float cooltime;
    public float Cooltime {get => cooltime; protected set => cooltime = value; }
    
    /*protected Transform skillTarget; // 여기 있어도 괜찮나? 계속 바뀔텐데 데이터 컨테이너에 있을 얘가 아닌가?
    public Transform SkillTarget { get => skillTarget; protected set => skillTarget = value; }*/
    //protected List<Transform> skillTargets;
    //public List<Transform> SkillTargets { get => skillTargets; protected set => skillTargets = value; }

    // 거리 체크
    /*protected virtual bool CheckRange(Transform caster, Transform target)
    {
        if (target == null)
            return false;
        float sqrDistance = (target.position - caster.position).sqrMagnitude;
        return sqrDistance <= skillRange * skillRange;
    }*/

    // 타겟 설정
    protected abstract BaseNode.ENodeState SetTargets(Transform caster, List<Transform> targets, LayerMask enemyLayer,
        bool isPriorityTargetFar);

    // 스킬 실행
    protected abstract BaseNode.ENodeState Perform(Transform caster, List<Transform> targets, Animator unitAnimator);

    /*protected virtual void ResetTargets()
    {
        SkillTargets.Clear();
    }*/

    // 스킬 행동 트리를 반환하는 메서드
    //public abstract SequenceNode CreateSkillBTree(Transform caster, LayerMask enemyLayer, bool isPriorityTargetFar, Animator unitAnimator);
    
    public SequenceNode CreateSkillBTree(Transform caster,List<Transform> targets, LayerMask enemyLayer, bool isPriorityTargetFar, Animator unitAnimator)
    {
        return new SequenceNode
        (
            new List<BaseNode>()
            {
                //new ConditionNode(() => CheckRange(caster)),
                new ActionNode(() => SetTargets(caster,targets ,enemyLayer, isPriorityTargetFar)), 
                new ActionNode(() => Perform(caster, targets, unitAnimator))
            }
        );
    }

    protected void ResetTargets(List<Transform> targets)
    {
        targets.Clear();
    }
}

