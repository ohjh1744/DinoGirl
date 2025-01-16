
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBossSkill", menuName = "Skills/BossSkill/AoE")]
public class BossAoESkill : AoESkill
{
public override BaseNode.ENodeState Perform(BaseUnitController caster, List<BaseUnitController> targets)
    {
        var raidBossCaster = caster as RaidBossUnitController;
        if (raidBossCaster == null)
        {
            Debug.LogWarning("다운캐스팅 실패");
            return BaseNode.ENodeState.Failure;
        }
        
        if (targets.Count == 0)
        {
            Debug.Log($"{SkillName}: 타겟이 없습니다.");
            return BaseNode.ENodeState.Failure;
        }

        if (raidBossCaster.UnitViewer.UnitAnimator == null)
        {
            Debug.LogWarning("애니메이터 없음;");
            return BaseNode.ENodeState.Failure;
        }
        
        raidBossCaster.UnitViewer.UnitAnimator.SetBool(raidBossCaster.UnitViewer.ParameterHash[(int)Parameter.Run], false);
        
        // 스킬 시전 시작
        if(!GetBoolSkillParameter(raidBossCaster))
        {
            SetBoolSkillParameter(raidBossCaster, true);
            Debug.Log($" {raidBossCaster.gameObject.name} 스킬 시전");
            SpawnVFX(raidBossCaster.transform, raidBossCaster.CenterPosition,VFXToMine);
            raidBossCaster.CoolTimeCounter = Cooltime;
            raidBossCaster.IsSkillRunning = true;
            return BaseNode.ENodeState.Running;
        }
        
        var stateInfo = raidBossCaster.UnitViewer.UnitAnimator.GetCurrentAnimatorStateInfo(0);
        {
            if (stateInfo.normalizedTime < 1.0f)
            {
                Debug.Log($"{raidBossCaster.gameObject.name} : '{SkillName}' 사용 중.");
                return BaseNode.ENodeState.Running;
            }
            else if (stateInfo.normalizedTime >= 1.0f)
            {
                {
                    Debug.Log($"{raidBossCaster.gameObject.name} : '{SkillName}' 사용 완료.");
                    
                    SetBoolSkillParameter(raidBossCaster, false);
                    raidBossCaster.IsSkillRunning = false;

                    OverlapAreaAllWithAngle(raidBossCaster, targets);
                    //SpawnEffect(raidBossCaster.CenterPosition,VFXToMine);
                    float skillDamage = raidBossCaster.UnitModel.AttackPoint * SkillRatio;
                    Debug.Log($"스킬 데미지 {skillDamage} ");
                    foreach (var target in targets)
                    {
                        // 데미지 주는 로직
                        // 데미지를 줄 인원 수 선택 필요
                        if (target.gameObject != null)
                        {
                            target.UnitModel.TakeDamage(Mathf.RoundToInt(skillDamage));
                            if (CrowdControl != CrowdControls.None)
                            {
                                target.UnitModel.TakeCrowdControl(CrowdControl, CcDuration, raidBossCaster);
                            }
                        }
                    }
                    return BaseNode.ENodeState.Success;
                }
            }
        }
        return BaseNode.ENodeState.Failure;
    }
}
