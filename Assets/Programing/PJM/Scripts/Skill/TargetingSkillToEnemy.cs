using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TargetingSkillToEnemy", menuName = "Skills/TargetingSkillToEnemy")]
public class TargetingSkillToEnemy : Skill
{
    protected override BaseNode.ENodeState SetTargets(BaseUnitController caster, List<BaseUnitController> targets)
    {
        ResetTargets(targets);

        if (TargetAll)
        {
            string myLayerName = LayerMask.LayerToName(caster.gameObject.layer);
            if (myLayerName == "UserCharacter")
            {
                foreach (var target in BattleSceneManager.Instance.enemyUnits)
                {
                    if(target == null || !target.gameObject.activeSelf)
                        continue;
                    targets.Add(target);
                    Debug.Log(target.gameObject.name);
                }
                
            }
            else
            {
                foreach (var target in BattleSceneManager.Instance.myUnits)
                {
                    if(target == null || !target.gameObject.activeSelf)
                        continue;
                    targets.Add(target);
                    Debug.Log(target.gameObject.name);
                }
            }
            return targets.Count > 0 ? BaseNode.ENodeState.Success : BaseNode.ENodeState.Failure;
        }
        
        
        Collider2D[] detectedColliders = Physics2D.OverlapCircleAll(caster.transform.position, SkillRange, caster.EnemyLayer);
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

        // 거리를 기준으로 정렬
        targets.Sort((a, b) =>
        {
            float distanceA = Vector2.Distance(caster.transform.position, a.transform.position);
            float distanceB = Vector2.Distance(caster.transform.position, b.transform.position);
            return IsPriorityTargetFar ? distanceB.CompareTo(distanceA) : distanceA.CompareTo(distanceB);
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
        if (targets.Count == 0)
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
        //caster.UnitViewer.UnitAnimator.SetBool(caster.UnitViewer.ParameterHash[(int)Parameter.Attack], false);

        // 스킬 시전 시작
        //if (!caster.IsSkillRunning)
        if(!caster.UnitViewer.UnitAnimator.GetBool(caster.UnitViewer.ParameterHash[(int)Parameter.Skill]))
        {
            caster.UnitViewer.UnitAnimator.SetBool(caster.UnitViewer.ParameterHash[(int)Parameter.Skill], true);
            Debug.Log($" {caster.gameObject.name} 스킬 시전");
            caster.CoolTimeCounter = Cooltime;
            caster.IsSkillRunning = true;
            return BaseNode.ENodeState.Running;
        }
        
        
        var stateInfo = caster.UnitViewer.UnitAnimator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("UsingSkill"))
        {
            if (stateInfo.normalizedTime < 1.0f)
            {
                Debug.Log($"{caster.gameObject.name} : '{SkillName}' 사용 중.");
                return BaseNode.ENodeState.Running;
            }
            else if (stateInfo.normalizedTime >= 1.0f)
            {
                {
                    Debug.Log($"{caster.gameObject.name} : '{SkillName}' 사용 완료.");
                    caster.UnitViewer.UnitAnimator.SetBool(caster.UnitViewer.ParameterHash[(int)Parameter.Skill],
                        false);
                    caster.IsSkillRunning = false;

                    float attackDamage = caster.UnitModel.AttackPoint * SkillRatio;
                    foreach (var target in targets)
                    {
                        if(target == null || !target.gameObject.activeSelf)
                            continue;
                        // 데미지 주는 로직
                        // 데미지를 줄 인원 수 선택 필요
                        target.UnitModel.TakeDamage((int)attackDamage); // 소숫점 버림, 반올림할지 선택 필요
                        //Debug.Log($"{SkillName}으로 {(int)attackDamage} 만큼 데미지를 {target}에 가함");
                        if (CrowdControl != CrowdControls.None)
                        {
                            target.UnitModel.TakeCrowdControl(CrowdControl, CcDuration, caster);
                            if (CrowdControl == CrowdControls.Taunt)
                                target.TauntSource = caster;
                        }
                    }

                    return BaseNode.ENodeState.Success;
                }
            }
        }

        //else
            
        // 트랜지션에서 애니메이션이 블렌딩 될 때 출력됨 (해결)
        // 스킬이 진행중 군중제어에 끊겼지만 애니메이터의 파라미터는 여전히 스킬시전중일경우

        /*Debug.Log("IsUsingSkill이 True지만 현재 애니메이션 상태가 UsingSkill이 아님");
        caster.UnitViewer.UnitAnimator.SetBool(caster.UnitViewer.ParameterHash[(int)Parameter.Run], false);
        caster.IsSkillRunning = false;
        return BaseNode.ENodeState.Failure;*/
        // 아래쪽과 구분할 필요가 없어질 경우 병합



        //return BaseNode.ENodeState.Running;
            

        // Skill이 true고 현재 애니메이터의 진행 상황이 UsingSKill이 아닌 상황?

        //else if(unitAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)

        /*{
            Debug.Log($"{SkillName}: {targets[0].name}에게 스킬 완료.");
            caster.UnitViewer.UnitAnimator.SetBool(caster.UnitViewer.ParameterHash[(int)AniState.Skill],false);
            return BaseNode.ENodeState.Success;
        }*/

        Debug.LogWarning("예외 상황");
        //caster.UnitViewer.UnitAnimator.SetBool(caster.UnitViewer.ParameterHash[(int)Parameter.Run], false);
        //caster.IsSkillRunning = false;
        return BaseNode.ENodeState.Failure;
    }

    // 기존 적 단일대상 타겟 선택 메서드
    /*protected override BaseNode.ENodeState SetTargets(BaseUnitController caster, List<BaseUnitController> targets)
    {
        ResetTargets(targets);
        Collider2D[] detectedColliders = Physics2D.OverlapCircleAll(caster.transform.position, SkillRange, caster.EnemyLayer);
        if (detectedColliders.Length == 0)
        {
            return BaseNode.ENodeState.Failure;
        }

        float minDistance = float.MaxValue;
        float maxDistance = float.MinValue;
        BaseUnitController closetEnemy = null;
        BaseUnitController farthestEnemy = null;

        foreach (Collider2D collider in detectedColliders)
        {
            float distance = Vector2.Distance(caster.transform.position, collider.transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                if (collider.gameObject.TryGetComponent(out closetEnemy))
                {

                }
                else
                {
                    Debug.LogWarning("BaseUnitController 컴포넌트 찾지 못함.");
                    return BaseNode.ENodeState.Failure;
                }
                //closetEnemy = collider.gameObject.TryGetComponent<BaseUnitController>();
            }

            if (distance > maxDistance)
            {
                maxDistance = distance;
                farthestEnemy = collider.gameObject.TryGetComponent<BaseUnitController>();
            }
        }
        if (caster.UnitModel.IsPriorityTargetFar)
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
    }*/
    
    // 기존 적 단일대상 스킬 사용 메서드
    /*protected override BaseNode.ENodeState Perform(BaseUnitController caster, List<BaseUnitController> targets)
    {
        if (targets[0] == null || !targets[0].gameObject.activeSelf)
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
        if(!caster.UnitViewer.UnitAnimator.GetBool(caster.UnitViewer.ParameterHash[(int)Parameter.Skill]))
        {
            caster.UnitViewer.UnitAnimator.SetBool(caster.UnitViewer.ParameterHash[(int)Parameter.Skill],true);
            Debug.Log($"{SkillName}: {targets[0].name}에게 스킬 시전.");
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
                Debug.Log($"{SkillName}: {targets[0].name}에게 스킬 사용 중.");
                return BaseNode.ENodeState.Running;
            }
            else if (stateInfo.normalizedTime >= 1.0f)
            {
                Debug.Log($"{SkillName}: {targets[0].name}에게 스킬 완료.");
                caster.UnitViewer.UnitAnimator.SetBool(caster.UnitViewer.ParameterHash[(int)Parameter.Skill],false);
                caster.IsSkillRunning = false;
                foreach (var target in targets)
                {
                    // 데미지 주는 로직
                    // 데미지를 줄 인원 수 선택 필요
                    target.UnitModel.TakeDamage();
                }
                return BaseNode.ENodeState.Success;
            }
            
        }
        else
        {
            Debug.Log("IsUsingSkill이 True지만 현재 애니메이션 상태가 UsingSkill이 아님");
            return BaseNode.ENodeState.Running;
        }

        // Skill이 true고 현재 애니메이터의 진행 상황이 UsingSKill이 아닌 상황?
        
        //else if(unitAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        
        /*{
            Debug.Log($"{SkillName}: {targets[0].name}에게 스킬 완료.");
            caster.UnitViewer.UnitAnimator.SetBool(caster.UnitViewer.ParameterHash[(int)AniState.Skill],false);
            return BaseNode.ENodeState.Success;
        }#1#
        
        Debug.LogWarning("예외 상황");
        return BaseNode.ENodeState.Failure;
    }*/
}
