using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TargetingSkillToAlly", menuName = "Skills/TargetingSkillToAlly")]
public class TargetingSkillToAlly : Skill
{
    protected override BaseNode.ENodeState SetTargets(BaseUnitController caster, List<BaseUnitController> targets)
    {
        ResetTargets(targets);
        
        if (TargetAll)
        {
            string myLayerName = LayerMask.LayerToName(caster.gameObject.layer);
            if (myLayerName == "UserCharacter")
            {
                foreach (var target in BattleSceneManager.Instance.myUnits)
                {
                    if(target == null || !target.gameObject.activeSelf)
                        continue;
                    targets.Add(target);
                    Debug.Log(target.gameObject.name);
                }
                
            }
            else
            {
                foreach (var target in BattleSceneManager.Instance.enemyUnits)
                {
                    if(target == null || !target.gameObject.activeSelf)
                        continue;
                    targets.Add(target);
                    Debug.Log(target.gameObject.name);
                }
            }
            return targets.Count > 0 ? BaseNode.ENodeState.Success : BaseNode.ENodeState.Failure;
        }
        
        Collider2D[] detectedColliders = Physics2D.OverlapCircleAll(caster.transform.position, SkillRange, caster.AllianceLayer);
        if (detectedColliders.Length == 0)
        {
            return BaseNode.ENodeState.Failure;
        }

        foreach (var collider in detectedColliders)
        {
            if (collider.TryGetComponent(out BaseUnitController target))
            {
                targets.Add(target);
            }
            else
            {
                Debug.LogWarning($"{collider.gameObject.name}은 baseUnitCollider를 가지지 않음");
            }
        }

        if (targets.Count == 0)
        {
            Debug.LogWarning("적합한 타겟을 찾지 못했습니다.");
            return BaseNode.ENodeState.Failure;
        }

        // Todo : 
        // 현재 체력의 비율을 기준으로 정렬 (임시), 필요할 경우 타겟 선정 조건을 따로 설정할 수도 있어야함
        targets.Sort((a, b) =>
        {
            float hpA = a.UnitModel.Hp / a.UnitModel.MaxHp;
            //Debug.Log(hpA);
            float hpB = b.UnitModel.Hp / b.UnitModel.MaxHp;
            //Debug.Log(hpB);
            return hpA.CompareTo(hpB);
        });

        // 최대 타겟 수만큼만 타겟에 남기기
        //for (int i = 0; i < MaxTargetingNum; i++)
        if(targets.Count > MaxTargetingNum)
        {
            targets.RemoveRange(MaxTargetingNum, targets.Count - MaxTargetingNum);
        }
        
        return targets.Count > 0 ? BaseNode.ENodeState.Success : BaseNode.ENodeState.Failure;
    }
    
    
    protected override BaseNode.ENodeState Perform(BaseUnitController caster, List<BaseUnitController> targets)
    {
        //if (targets[0] == null || !targets[0].gameObject.activeSelf)
        if(targets.Count == 0)
        {
            Debug.Log($"{SkillName}: 타겟이 없습니다.");
            return BaseNode.ENodeState.Failure;
        }

        //if (!unitAnimator.GetBool("Skill"))
        if (caster.UnitViewer.UnitAnimator == null)
        {
            Debug.LogWarning("애니메이터 없음;");
            return BaseNode.ENodeState.Failure;
        }
        // 임시
        caster.UnitViewer.UnitAnimator.SetBool(caster.UnitViewer.ParameterHash[(int)Parameter.Run], false);
            
        // 스킬 시전 시작
        if(!caster.UnitViewer.UnitAnimator.GetBool(caster.UnitViewer.ParameterHash[(int)Parameter.Skill0]))
        {
            caster.UnitViewer.UnitAnimator.SetBool(caster.UnitViewer.ParameterHash[(int)Parameter.Skill0],true);
            //Debug.Log($"{SkillName}: {targets[0].name}에게 스킬 시전.");
            Debug.Log($" {caster.gameObject.name} 스킬 시전");
            SpawnEffect(caster.transform, VFXToMine);
            caster.CoolTimeCounter = Cooltime;
            caster.IsSkillRunning = true;
            return BaseNode.ENodeState.Running;
        }
            
        
        //if (unitAnimator.GetCurrentAnimatorStateInfo(0).IsName("UsingSkill"))
        var stateInfo = caster.UnitViewer.UnitAnimator.GetCurrentAnimatorStateInfo(0);
        
        
        //if(caster.UnitViewer.IsAnimationRunning("UsingSkill"))
        if(stateInfo.IsName("UsingSkill"))
        {
            if (stateInfo.normalizedTime < 1.0f)
            {
                Debug.Log($"{caster.gameObject.name} : '{SkillName}' 사용 중.");
                return BaseNode.ENodeState.Running;
            }
            else if (stateInfo.normalizedTime >= 1.0f)
            {
                Debug.Log($"{caster.gameObject.name} : '{SkillName}' 사용 완료.");
                //Debug.Log($"{SkillName}: {targets[0].name}에게 스킬 완료.");
                caster.UnitViewer.UnitAnimator.SetBool(caster.UnitViewer.ParameterHash[(int)Parameter.Skill0],false);
                caster.IsSkillRunning = false;

                
                foreach (var target in targets)
                {
                    if(target == null || !target.gameObject.activeSelf)
                        continue;
                    // 적용해줄 로직
                    // 임시로 반드시 힐하도록 설정
                    // 임시로 아군 체력의 50%
                    int healingAmount = (int)(target.UnitModel.MaxHp * SkillRatio);
                    target.UnitModel.TakeHeal(healingAmount);
                    SpawnEffect(target.CenterPosition, VFXToTarget);
                    Debug.Log(target.gameObject.name);
                }
                return BaseNode.ENodeState.Success;
            }
            
        }
        else
        {
            // 트랜지션에서 애니메이션이 블렌딩 될 때 출력됨
            Debug.Log("IsUsingSkill이 True지만 현재 애니메이션 상태가 UsingSkill이 아님");
            return BaseNode.ENodeState.Running;
        }

        // Skill이 true고 현재 애니메이터의 진행 상황이 UsingSKill이 아닌 상황?
        
        //else if(unitAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        
        /*{
            Debug.Log($"{SkillName}: {targets[0].name}에게 스킬 완료.");
            caster.UnitViewer.UnitAnimator.SetBool(caster.UnitViewer.ParameterHash[(int)AniState.Skill],false);
            return BaseNode.ENodeState.Success;
        }*/
        
        Debug.LogWarning("예외 상황");
        return BaseNode.ENodeState.Failure;
    }
    
    // 스킬 조건과 스킬의 실제 적용해줄 추가 로직 선언이 필요, 구분도 지어야함
}

// 기존 멀티 타겟팅 스킬 
/*public int maxTargets; // 최대 타겟 수
// 전체 타겟일 경우 배틀매니저에서 전체 리스트를 가져옴?

protected override BaseNode.ENodeState SetTargets(BaseUnitController caster, List<BaseUnitController> targets)
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

/*protected override BaseNode.ENodeState Perform(BaseUnitController caster, List<Transform> targets)
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
}#1#*/

