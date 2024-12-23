using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SingleTargetSkillToEnemy", menuName = "Skills/SingleTargetSkillToEnemy")]
public class SingleTargetSkillToEnemy : Skill
{
    protected override BaseNode.ENodeState SetTargets(UnitController caster, List<Transform> targets)
    {
        ResetTargets(targets);
        Collider2D[] detectedColliders = Physics2D.OverlapCircleAll(caster.transform.position, SkillRange, caster.EnemyLayer);
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
        if (caster.IsPriorityTargetFar)
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
    
    protected override BaseNode.ENodeState Perform(UnitController caster, List<Transform> targets)
    {
        if (targets[0] == null)
        {
            Debug.Log($"{SkillName}: 타겟이 없습니다.");
            return BaseNode.ENodeState.Failure;
        }

        //if (!unitAnimator.GetBool("Skill"))
        if (caster.UnitAnimator == null)
        {
            Debug.LogWarning("애니메이터 없음;");
            return BaseNode.ENodeState.Failure;
        }
            
        
        if(caster.UnitAnimator.GetBool("Skill"))
        {
            caster.UnitAnimator.SetBool("Skill",true);
            Debug.Log($"{SkillName}: {targets[0].name}에게 스킬 시전.");
            return BaseNode.ENodeState.Success;
        }
            
        
        //if (unitAnimator.GetCurrentAnimatorStateInfo(0).IsName("UsingSkill"))
        if(caster.IsAnimationRunning("UsingSkill"))
        {
            Debug.Log($"{SkillName}: {targets[0].name}에게 스킬 사용 중.");
            return BaseNode.ENodeState.Running;
        }

        // Skill이 true고 현재 애니메이터의 진행 상황이 UsingSKill이 아닌 상황?
        
        //else if(unitAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        
        {
            Debug.Log($"{SkillName}: {targets[0].name}에게 스킬 완료.");
            caster.UnitAnimator.SetBool("Skill",false);
            return BaseNode.ENodeState.Success;
        }
        
        Debug.LogWarning("예외 상황");
        return BaseNode.ENodeState.Failure;
    }
}
