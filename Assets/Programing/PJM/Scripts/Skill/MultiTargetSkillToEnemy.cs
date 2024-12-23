using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MultiTargetSkillToEnemy", menuName = "Skills/MultiTargetSkillToEnemy")]
public class MultiTargetSkillToEnemy : Skill
{
    public int maxTargets; // 최대 타겟 수

    protected override bool SetTarget(Transform caster, LayerMask enemyLayer, bool isPriorityTargetFar)
    {
        ResetTargets();

        Collider2D[] detectedColliders = Physics2D.OverlapCircleAll(caster.position, skillRange, enemyLayer);
        if (detectedColliders.Length == 0)
        {
            return false;
        }

        // 거리를 기준으로 정렬, 선택사항, 일단 넣어둠
        List<Collider2D> sortedColliders = new List<Collider2D>(detectedColliders);
        sortedColliders.Sort((a, b) =>
        {
            float distanceA = Vector2.Distance(caster.position, a.transform.position);
            float distanceB = Vector2.Distance(caster.position, b.transform.position);
            return isPriorityTargetFar ? distanceB.CompareTo(distanceA) : distanceA.CompareTo(distanceB);
        });

        // 최대 타겟 수만큼 선택
        foreach (var collider in sortedColliders)
        {
            if (skillTargets.Count >= maxTargets)
                break;

            skillTargets.Add(collider.transform);
        }

        return skillTargets.Count > 0;
    }

    protected override void Perform(Transform caster)
    {
        if (skillTargets.Count == 0)
        {
            Debug.Log($"{skillName}: 타겟이 없습니다.");
            return;
        }

        foreach (var target in skillTargets)
        {
            Debug.Log($"{skillName}: {target.name}에게 {skillName} 사용!");
            // 멀티타겟 전용 로직 추가
        }
    }
}

