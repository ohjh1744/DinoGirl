using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MultiTargetSkillToEnemy", menuName = "Skills/MultiTargetSkillToEnemy")]
public class MultiTargetSkillToEnemy : Skill
{
    public int maxTargets; // 최대 타겟 수
    // 전체 타겟일 경우 배틀매니저에서 전체 리스트를 가져옴?

    protected override BaseNode.ENodeState SetTargets(BaseUnitController caster, List<Transform> targets)
    {
        ResetTargets(targets);
        Collider2D[] detectedColliders = Physics2D.OverlapCircleAll(caster.transform.position, SkillRange, caster.EnemyLayer);
        if (detectedColliders.Length == 0)
        {
            return BaseNode.ENodeState.Failure;
        }

        // 거리를 기준으로 정렬, 선택사항, 일단 넣어둠
        targets.Sort((a, b) =>
        {
            float distanceA = Vector2.Distance(caster.transform.position, a.transform.position);
            float distanceB = Vector2.Distance(caster.transform.position, b.transform.position);
            return caster.UnitModel.IsPriorityTargetFar ? distanceB.CompareTo(distanceA) : distanceA.CompareTo(distanceB);
        });

        // 최대 타겟 수만큼 선택
        foreach (var target in targets)
        {
            if (targets.Count >= maxTargets)
                break;

            targets.Add(target.transform);
        }

        if(targets.Count > 0)
            return BaseNode.ENodeState.Success;
        
        return BaseNode.ENodeState.Failure;
            
    }

    protected override BaseNode.ENodeState Perform(BaseUnitController caster, List<Transform> targets)
    {
        if (targets.Count == 0)
        {
            Debug.Log($"{SkillName}: 타겟이 없습니다.");
            return BaseNode.ENodeState.Failure;
        }

        foreach (var target in targets)
        {
            Debug.Log($"{target.name}에게 {SkillName} 사용!");
            return BaseNode.ENodeState.Success;
            // 멀티타겟 전용 로직 추가
        }

        return BaseNode.ENodeState.Running;
    }
}

