using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill
{
    private string _skillName;
    public string SkillName { get => _skillName; private set => _skillName = value; }

    public Skill(string skillName)
    {
        SkillName = skillName;
    }
    
    public abstract bool CheckRange();

    // 타겟 설정
    public abstract BaseNode.ENodeState SetTarget();

    // 스킬 실행
    public abstract BaseNode.ENodeState Perform();

    // 스킬 행동 트리를 반환 메서드
    public SequenceNode CreateSkillTree()
    {
        return new SequenceNode
        (
            new List<BaseNode>()
            {
                new ConditionNode(CheckRange),
                new ActionNode(SetTarget),
                new ActionNode(Perform)
            }
        );
    }
    


}
