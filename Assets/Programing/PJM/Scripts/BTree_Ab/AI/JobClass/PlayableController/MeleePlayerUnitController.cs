using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleePlayerUnitController : PlayableUnitController
{
    //[SerializeField] private bool _isAssassin;
    protected void Awake()
    {
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
                    new List<BaseNode>()
                    {
                        new ConditionNode(CheckSkillCooltime),
                        new SelectorNode
                        (
                            new List<BaseNode>()
                            {
                                new ConditionNode(CheckAutoOn),
                                new ConditionNode(CheckUserInput)
                            }
                        ),
                        
                        new ActionNode(UseSkill)
                    }
                ),
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

    public override BaseNode.ENodeState UseSkill()
    {
        // 배틀 매니저에서 컷신 띄우기 및 타임스케일 조정
        // 애니메이션이 끝났을 때 Success 반환?
        
        Debug.Log($"전사 유닛  스킬 사용! "); //{UnitID}
        return BaseNode.ENodeState.Success;
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
