using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SingleTargetSkillToEnemy", menuName = "Skills/SingleTargetSkillToEnemy")]
public class SingleTargetSkillToEnemy : Skill
{
    protected override BaseNode.ENodeState SetTargets(Transform caster, List<Transform> targets, LayerMask enemyLayer, bool isPriorityTargetFar)
    {
        ResetTargets(targets);
        Collider2D[] detectedColliders = Physics2D.OverlapCircleAll(caster.position, SkillRange, enemyLayer);
        if (detectedColliders.Length == 0)
        {
            return BaseNode.ENodeState.Failure;
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
            targets.Add(farthestEnemy);
        }
        else
        {
            targets.Add(closetEnemy);
        }
        
        if(targets.Count > 0)
            return BaseNode.ENodeState.Success;
        
        return BaseNode.ENodeState.Failure;
    }
    
    protected override BaseNode.ENodeState Perform(Transform caster, List<Transform> targets, Animator unitAnimator)
    {
        if (targets[0] == null)
        {
            Debug.Log($"{SkillName}: 타겟이 없습니다.");
            return BaseNode.ENodeState.Failure;
        }
        if (!unitAnimator.GetCurrentAnimatorStateInfo(0).IsName("UsingSkill"))
        {
            unitAnimator.SetTrigger("Skill");
            Debug.Log($"{SkillName}: {targets[0].name}에게 스킬 사용 시작.");
            return BaseNode.ENodeState.Running;
        }

        if (unitAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            Debug.Log($"{SkillName}: {targets[0].name}에게 스킬 완료.");
            return BaseNode.ENodeState.Success;
        }

        return BaseNode.ENodeState.Running;
    }
}
