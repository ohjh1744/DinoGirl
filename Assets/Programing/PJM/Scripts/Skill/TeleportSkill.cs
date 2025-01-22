using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TelePortToEnemy", menuName = "Skills/TargetingEnemy/TeleportSkill")]
public class TeleportSkill : TargetingSkillToEnemy
{
    public float distance;

    public override BaseNode.ENodeState Perform(BaseUnitController caster, List<BaseUnitController> targets)
    {
        if (targets.Count == 0)
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
        if (!caster.IsSkillRunning)
        {
            // 임시
            caster.UnitViewer.UnitAnimator.SetBool(caster.UnitViewer.ParameterHash[(int)Parameter.Run], false);
            // 적 뒤로 순간이동
            SpawnVFX(caster.transform,caster.CenterPosition, VFXToMine);
            PlaySkillSfx(SkillStartSound);
            float enemyDir = Mathf.Sign(targets[0].gameObject.transform.localScale.x);
            float behindX = targets[0].gameObject.transform.position.x + enemyDir * distance;
            Vector2 behindPos = new Vector2(behindX, targets[0].gameObject.transform.position.y);
            caster.gameObject.transform.position = behindPos;
            
            // 적을 바라보도록 설정
            float diffX = targets[0].transform.position.x - caster.gameObject.transform.position.x;
            float casterDir = diffX > 0 ? -1 : 1;
            Vector3 casterScale = caster.gameObject.transform.localScale;
            casterScale.x = Mathf.Abs(casterScale.x) * casterDir;
            caster.gameObject.transform.localScale = casterScale;
            
            SpawnVFX(caster.transform, caster.CenterPosition, VFXToMine);
            caster.DetectedEnemy = targets[0];
            caster.IsSkillRunning = true;
            
        }
        
        if(!GetBoolSkillParameter(caster))
        {
            SetBoolSkillParameter(caster,true);
            Debug.Log($" {caster.gameObject.name} 스킬 시전");
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
        else if (stateInfo.normalizedTime >= 1.0f)
        {
            {
                SetBoolSkillParameter(caster,false);
                caster.IsSkillRunning = false;

                float attackDamage = caster.UnitModel.AttackPoint * SkillRatio;
                foreach (var target in targets)
                {
                    // 데미지 주는 로직
                    // 데미지를 줄 인원 수 선택 필요
                    if (target.gameObject != null)
                    {
                        target.UnitModel.TakeDamage(Mathf.RoundToInt(attackDamage)); 
                        SpawnVFX(caster.transform, target.CenterPosition, VFXToTarget);
                        //Debug.Log($"{SkillName}으로 {(int)attackDamage} 만큼 데미지를 {target}에 가함");
                        if (CrowdControl != CrowdControls.None)
                        {
                            target.UnitModel.TakeCrowdControl(CrowdControl, CcDuration, caster);
                        }
                    }
                }
                PlaySkillSfx(SkillEndSound);
                return BaseNode.ENodeState.Success;
            }
        }

        Debug.LogWarning("예외 상황");
        return BaseNode.ENodeState.Failure;
    }
}
