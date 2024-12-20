using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherUnitController : UnitController
{
    protected void Awake()
    {
        //DetectRange = 20.0f;
        AttackRange = 15.0f;
        MoveSpeed = 1.0f;
    }
    
    protected override BaseNode SetBTree()
    {
        return new SelectorNode
        (
            new List<BaseNode>
            {
                new SequenceNode
                (
                    new List<BaseNode>
                    {
                        new ConditionNode(CheckAttackRange),
                        new ActionNode(SetTargetToAttack),
                        new ActionNode(() => PerformAttack("Attack"))
                    }
                ),
                new SequenceNode
                (
                    new List<BaseNode>
                    {
                        new ActionNode(SetDetectedTarget),
                        new ActionNode(ChaseTarget)
                    }
                ),
                new ActionNode(StayIdle)
            }
        );
    }
    
    private BaseNode.ENodeState PerformAttack(string animationName)
    {
        if (_currentTarget == null) return BaseNode.ENodeState.Failure;
        if (!_attackTriggered)
        {
            _attackTriggered = true;
            _animator.SetTrigger("Attack");
            Debug.Log($"{_currentTarget.gameObject.name}에 아처 공격!");
            StartCoroutine(ResetAttackTrigger(animationName));
            return BaseNode.ENodeState.Running;
        }
        if (IsAnimationRunning(animationName))
        {
            return BaseNode.ENodeState.Running;
        }
        return BaseNode.ENodeState.Success;
    }
}
