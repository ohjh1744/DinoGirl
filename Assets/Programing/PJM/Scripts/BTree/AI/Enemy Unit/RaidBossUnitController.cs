using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaidBossUnitController : EnemyBaseUnitController
{
    [SerializeField] private Skill _bossSkill1;
    protected Skill BossSkill1 => _bossSkill1;
    [SerializeField] private Skill _bossSkill2;
    protected Skill BossSkill2 => _bossSkill1;
    [SerializeField] private Skill _bossSkill3;
    protected Skill BossSkill3 => _bossSkill1;

    private List<BaseUnitController> _skillTargets;
    protected List<BaseUnitController> SkillTargets { get => _skillTargets; set => _skillTargets = value; }
    [SerializeField] private GameObject laserObejct;
    public GameObject LaserObejct {get => laserObejct; set => laserObejct = value; }
    
    private BossSkillRuntimeData _skillRuntimeData;
    public BossSkillRuntimeData SkillRuntimeData { get => _skillRuntimeData; set => _skillRuntimeData = value; }
    protected override void Awake()
    {
        base.Awake();
        SkillTargets = new List<BaseUnitController>();
        CoolTimeCounter = 10.0f;
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
                            BossSkill1.CreatePerformNode(this, SkillTargets) // 사실상 스킬체크노드를 따로 만들어야함
                        ),
                        new SequenceNode // skillable Dicision Sequence
                        (
                            new List<BaseNode>()
                            {
                                new ConditionNode(CheckSkillCooltimeBack),
                                // 타임체크로 무슨 스킬을 쓸지 정해야함
                                BossSkill1.CreateSkillBTree(this, SkillTargets)
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
