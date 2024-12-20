using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeController : UnitController
{
    private bool _isCooltimeReturn;

    private bool _isAutoOn = true; // 우선 기본적으로 켜져있음
    
    
    //[SerializeField] private bool _isAssassin;
    protected override void Awake()
    {
        base.Awake();
        //DetectRange = 20.0f;
        AttackRange = 2.0f;
        MoveSpeed = 2.0f;
    }

    protected override void Start()
    {
        base.Start();
        // 추가로 해줄 동작 설정
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

    // skill

    public override void UseSkill()
    {
        Debug.Log($"전사 유닛  스킬 사용! "); //{UnitID}
    }


    private BaseNode.ENodeState PerformAttack(string animationName)
    {
        if (_currentTarget == null) return BaseNode.ENodeState.Failure;
        if (!_attackTriggered)
        {
            _attackTriggered = true;
            _animator.SetTrigger("Attack");
            Debug.Log($"{_currentTarget.gameObject.name}에 워리어 공격!");
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
