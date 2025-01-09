using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageBossUnitController : EnemyBaseUnitController
{
    [SerializeField] private Skill _bossSkill;
    protected Skill BossSkill {get => _bossSkill; set => _bossSkill = value; }
    private List<BaseUnitController> _skillTargets;
    protected List<BaseUnitController> SkillTargets { get => _skillTargets; set => _skillTargets = value; }
    
    protected override void Awake()
    {
        base.Awake();
        SkillTargets = new List<BaseUnitController>();
        CoolTimeCounter = 5.0f; // 임시
    }
    
    
    protected override BaseNode SetBTree()
    {
        return new SelectorNode //Behaviour Selector
        (
            new List<BaseNode>
            {
                new SelectorNode
                (
                    new List<BaseNode>
                    {
                        new ActionNode(CheckUnitDying),
                        new ActionNode(CheckCrowdControl),
                    }
                ),

                new SelectorNode // skillable Dicision Selector
                (
                    new List<BaseNode>
                    {
                        new DecoratorNode
                        (
                            new ConditionNode(IsSkillAlreadyRunning),
                            BossSkill.CreatePerformNode(this, SkillTargets)
                        ),
                        new SequenceNode // skillable Dicision Sequence
                        (
                            new List<BaseNode>()
                            {
                                new ConditionNode(CheckSkillCooltimeBack),
                                BossSkill.CreateSkillBTree(this, SkillTargets)
                            }
                        ),
                    }
                ),

                new SequenceNode // Attack Dicision
                (
                    new List<BaseNode>
                    {
                        new ConditionNode(CheckAttackRange),
                        new ActionNode(SetTargetToAttack),
                        new ActionNode(PerformAttack)
                    }
                ),
                new SequenceNode
                (
                    new List<BaseNode>
                    {
                        new ActionNode(SetDetectedTarget),
                        // CheckMoveable ?
                        new ActionNode(ChaseTarget)
                    }
                ),
                new ActionNode(StayIdle)
            }
        );
    }
}
