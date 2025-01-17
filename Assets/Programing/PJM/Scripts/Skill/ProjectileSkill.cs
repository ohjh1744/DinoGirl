using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Projectile Skill", menuName = "Skills/Projectile Skill")]
public class ProjectileSkill : Skill
{
    [SerializeField] private GameObject skillProjectile;
    public GameObject SkillProjectile { get => skillProjectile; set => skillProjectile = value; }

    public override BaseNode.ENodeState SetTargets(BaseUnitController caster, List<BaseUnitController> targets)
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
        
        
        Collider2D[] detectedColliders = Physics2D.OverlapCircleAll(caster.transform.position, SkillRangeRadius, caster.EnemyLayer);
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
        if(targets.Count > MaxTargetingNum)
        {
            targets.RemoveRange(MaxTargetingNum, targets.Count - MaxTargetingNum);
        }
        
        return targets.Count > 0 ? BaseNode.ENodeState.Success : BaseNode.ENodeState.Failure;
    }

    public override BaseNode.ENodeState Perform(BaseUnitController caster, List<BaseUnitController> targets)
    {
        var projectileCaster = caster as PlayerUnitControllerWithProjectile;
        if (projectileCaster == null)
        {
            Debug.LogWarning("다운캐스팅 실패");
            return BaseNode.ENodeState.Failure;
        }
        
        if (targets.Count == 0)
        {
            Debug.Log($"{SkillName}: 타겟이 없습니다.");
            return BaseNode.ENodeState.Failure;
        }

        if (projectileCaster.UnitViewer.UnitAnimator == null)
        {
            Debug.LogWarning("애니메이터 없음;");
            return BaseNode.ENodeState.Failure;
        }

        
        
        projectileCaster.UnitViewer.UnitAnimator.SetBool(projectileCaster.UnitViewer.ParameterHash[(int)Parameter.Run], false);

        // 스킬 시전 시작
        if(!GetBoolSkillParameter(projectileCaster))
        {
            SetBoolSkillParameter(projectileCaster, true);
            Debug.Log($" {projectileCaster.gameObject.name} 스킬 시전");
            
            SpawnVFX(projectileCaster.transform, projectileCaster.transform, VFXToMine);
            PlaySkillSfx(SkillStartSound);
            
            projectileCaster.CoolTimeCounter = Cooltime;
            projectileCaster.IsSkillRunning = true;
            return BaseNode.ENodeState.Running;
        }
        
        
        var stateInfo = projectileCaster.UnitViewer.UnitAnimator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.normalizedTime < 1.0f)
        {
            //Debug.Log($"{projectileCaster.gameObject.name} : '{SkillName}' 사용 중.");
            return BaseNode.ENodeState.Running;
        }
        else if (stateInfo.normalizedTime >= 1.0f)
        {
            {
                projectileCaster.SkillProjectile.Clear();
                float attackDamage = projectileCaster.UnitModel.AttackPoint * SkillRatio;
                    
                Debug.Log($"{projectileCaster.gameObject.name} : '{SkillName}' 사용 완료.");
                foreach (var target in targets)
                {
                    if(target == null || !target.gameObject.activeSelf)
                        continue;
                    CreateSkillProjectile(projectileCaster, target, attackDamage);
                }
                SetBoolSkillParameter(projectileCaster, false);
                projectileCaster.IsSkillRunning = false;
                
                PlaySkillSfx(SkillEndSound);
                return BaseNode.ENodeState.Success;
            }
        }
        Debug.LogWarning("예외 상황");
        
        return BaseNode.ENodeState.Failure;
    }

    private void CreateSkillProjectile(PlayerUnitControllerWithProjectile projectileCaster, BaseUnitController target, float damage)
    {
        GameObject projectileObject = Instantiate(SkillProjectile, projectileCaster.MuzzlePoint.position, Quaternion.identity);
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        if(projectile != null)
        {
            projectile.Target = target;
            projectile.TargetPos = target.CenterPosition;
            projectile.HitDamage = damage;
            projectileCaster.SkillProjectile.Add(projectileObject);   
        }
        else
        {
            Destroy(projectileObject);
        }
    }
}
