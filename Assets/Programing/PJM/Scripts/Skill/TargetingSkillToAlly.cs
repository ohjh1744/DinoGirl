using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TargetingSkillToAlly", menuName = "Skills/TargetingSkillToAlly")]
public class TargetingSkillToAlly : Skill
{
    public override BaseNode.ENodeState SetTargets(BaseUnitController caster, List<BaseUnitController> targets)
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
        
        Collider2D[] detectedColliders = Physics2D.OverlapCircleAll(caster.transform.position, SkillRangeRadius, caster.AllianceLayer);
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
        
        targets.Sort((a, b) =>
        {
            float hpA = a.UnitModel.Hp / a.UnitModel.MaxHp;
            //Debug.Log(hpA);
            float hpB = b.UnitModel.Hp / b.UnitModel.MaxHp;
            //Debug.Log(hpB);
            return hpA.CompareTo(hpB);
        });

        // 최대 타겟 수만큼만 타겟에 남기기
        if(targets.Count > MaxTargetingNum)
        {
            targets.RemoveRange(MaxTargetingNum, targets.Count - MaxTargetingNum);
        }
        
        return targets.Count > 0 ? BaseNode.ENodeState.Success : BaseNode.ENodeState.Failure;
    }


    public override BaseNode.ENodeState Perform(BaseUnitController caster, List<BaseUnitController> targets)
    {
        if(targets.Count == 0)
        {
            Debug.Log($"{SkillName}: 타겟이 없습니다.");
            return BaseNode.ENodeState.Failure;
        }

        if (caster.UnitViewer.UnitAnimator == null)
        {
            Debug.LogWarning("애니메이터 없음;");
            return BaseNode.ENodeState.Failure;
        }
        
            
        // 스킬 시전 시작
        if(!GetBoolSkillParameter(caster))
        {
            SetBoolSkillParameter(caster, true);
            Debug.Log($" {caster.gameObject.name} 스킬 시전");
            SpawnVFX(caster.transform, caster.transform, VFXToMine);
            PlaySkillSfx(SkillStartSound);
            caster.CoolTimeCounter = Cooltime;
            caster.IsSkillRunning = true;
            return BaseNode.ENodeState.Running;
        }
        
        var stateInfo = caster.UnitViewer.UnitAnimator.GetCurrentAnimatorStateInfo(0);
        
        if (stateInfo.normalizedTime < 1.0f)
        {
            //Debug.Log($"{caster.gameObject.name} : '{SkillName}' 사용 중.");
            return BaseNode.ENodeState.Running;
        }
        
        if (stateInfo.normalizedTime >= 1.0f)
        {
            Debug.Log($"{caster.gameObject.name} : '{SkillName}' 사용 완료.");
            //Debug.Log($"{SkillName}: {targets[0].name}에게 스킬 완료.");
            //caster.UnitViewer.UnitAnimator.SetBool(caster.UnitViewer.ParameterHash[(int)Parameter.Skill0],false);
            PlaySkillSfx(SkillEndSound);
            SetBoolSkillParameter(caster, false);
            caster.IsSkillRunning = false;

            foreach (var target in targets)
            {
                if(target == null || !target.gameObject.activeSelf)
                    continue;
                // 임시로 반드시 힐하도록 설정, 아군대상 스킬이 힐 외에 따로 없기때문에 일단 고정
                int healingAmount = (int)(target.UnitModel.MaxHp * SkillRatio);
                target.UnitModel.TakeHeal(healingAmount);
                SpawnAllVFXs(caster,target);
                Debug.Log(target.gameObject.name);
            }
            return BaseNode.ENodeState.Success;
        }
        Debug.LogWarning("예외 상황");
        return BaseNode.ENodeState.Failure;
    }
}

