using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RaidBossUnitController : EnemyBaseUnitController
{
    [SerializeField] private Skill[] _bossSkills;
    protected Skill[] BossSkills {get => _bossSkills; set => _bossSkills = value; }
    private int _skillIndex;
    public event Action<Skill> OnNextSkillSelected;
    
    private List<BaseUnitController> _skillTargets;
    protected List<BaseUnitController> SkillTargets { get => _skillTargets; set => _skillTargets = value; }
    [HideInInspector] private GameObject laserObejct;
    public GameObject LaserObejct {get => laserObejct; set => laserObejct = value; }
    
    private BossSkillRuntimeData _skillRuntimeData;
    public BossSkillRuntimeData SkillRuntimeData { get => _skillRuntimeData; set => _skillRuntimeData = value; }

    public Skill nextSkill { get; private set; }
    
    protected override void Awake()
    {
        base.Awake();
        SkillTargets = new List<BaseUnitController>();
        CurSkill = BossSkills[_skillIndex];
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

                new SelectorNode 
                (
                    new List<BaseNode>
                    {
                        new DecoratorNode
                        (
                            new ConditionNode(() => IsSkillRunning),
                            new ActionNode(PerformChosenSkill)
                            
                        ),
                        new SequenceNode // skillable Dicision Sequence
                        (
                            new List<BaseNode>()
                            {
                                new ConditionNode(CheckSkillCooltimeBack),
                                new ActionNode(ChooseNextSkill),
                                new ActionNode(SetTargetsForChosenSkill),
                                new ActionNode(PerformChosenSkill)
                                
                            }
                        ),
                    }
                ),

                /*new SequenceNode // Attack Dicision // 평타로직 필요할 경우를 대비해 주석처리
                (
                    new List<BaseNode>
                    {
                        new ConditionNode(CheckAttackRange),
                        new ActionNode(SetTargetToAttack),
                        new ActionNode(PerformAttack)
                    }
                ),*/
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

    private BaseNode.ENodeState ChooseNextSkill()
    {
        if (BossSkills == null || BossSkills.Length == 0)
        {
            Debug.LogWarning("보스 스킬 배열 공란");
            return BaseNode.ENodeState.Failure;
        }
        _skillIndex = (_skillIndex + 1) % BossSkills.Length; // bossSkills의 배열을 순회하고 다시 0으로
        CurSkill = BossSkills[_skillIndex];
        nextSkill = BossSkills[(_skillIndex + 1) % BossSkills.Length];
        Debug.Log($"현재 스킬 인덱스 : {_skillIndex}");
        OnNextSkillSelected?.Invoke(nextSkill);
        return BaseNode.ENodeState.Success;
    }

    private BaseNode.ENodeState PerformChosenSkill()
    {
        return BossSkills[_skillIndex].Perform(this, SkillTargets);
    }

    private BaseNode.ENodeState SetTargetsForChosenSkill()
    {
        return BossSkills[_skillIndex].SetTargets(this, SkillTargets);
    }
    
    
    protected override bool IsSkillAlreadyRunning()
    {
        return IsSkillRunning;
    }

}
