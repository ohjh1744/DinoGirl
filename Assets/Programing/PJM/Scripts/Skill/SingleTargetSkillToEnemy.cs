using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SingleTargetSkillToEnemy", menuName = "Skills/SingleTargetSkillToEnemy")]
public class SingleTargetSkillToEnemy : Skill
{
    protected override bool SetTarget(Transform caster, LayerMask enemyLayer, bool isPriorityTargetFar)
    {
        Collider2D[] detectedColliders = Physics2D.OverlapCircleAll(caster.position, skillRange, enemyLayer);
        if (detectedColliders.Length == 0)
        {
            skillTarget = null;
            return false;
        }

        float minDistance = float.MaxValue;
        float maxDistance = float.MinValue;
        Transform closetEnemy = null;
        Transform farthestEnemy = null;
        
        foreach (Collider2D collider in detectedColliders)
        {
            float distance = Vector2.Distance(caster.transform.position, collider.transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                closetEnemy = collider.transform;
            }

            if (distance > maxDistance)
            {
                maxDistance = distance;
                farthestEnemy = collider.transform;
            }
        }
        if (isPriorityTargetFar)
        {
            SkillTarget = farthestEnemy;
        }
        else
        {
            SkillTarget = closetEnemy;
        }
        return SkillTarget != null;
    }
    
    protected override void Perform(Transform caster)
    {
        if (SkillTarget == null)
        {
            Debug.Log($"{skillName}: 타겟이 없습니다.");
            return;
        }

        Debug.Log($"{skillName}: {SkillTarget.gameObject.name}에게 {skillName} 사용!");
    }
}
