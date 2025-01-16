using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnitControllerWithProjectile : PlayableBaseUnitController
{
    [SerializeField] private GameObject _projectilePrefab;
    //[SerializeField] private Transform _muzzlePoint;
    private GameObject _projectileObject;
    public List<GameObject> SkillProjectile { get; set; }

    protected override void Awake()
    {
        base.Awake();
        SkillTargets = new List<BaseUnitController>();
        SkillProjectile = new List<GameObject>();
        CoolTimeCounter = 0.5f;
    }

    protected override void Start()
    {
        base.Start();
        // 추가로 해줄 동작 설정
    }


    protected override BaseNode SetBTree()
    {
        return new SelectorNode //Behaviour Selector
        (
            new List<BaseNode>
            {
                new SelectorNode
                (
                    new List<BaseNode>
                    {
                        new ActionNode(CheckBattleWin),
                        new ActionNode(CheckUnitDying),
                        new ActionNode(CheckCrowdControl),
                    }
                ),

                new SelectorNode // skillable Dicision Selector
                (
                    new List<BaseNode>
                    {
                        new DecoratorNode
                        (
                            new ConditionNode(IsSkillAlreadyRunning),
                            UniqueSkill.CreatePerformNode(this, SkillTargets)
                        ),
                        new SequenceNode // skillable Dicision Sequence
                        (
                            new List<BaseNode>()
                            {
                                new ConditionNode(CheckSkillCooltimeBack),
                                new SelectorNode // condition Selector
                                (
                                    new List<BaseNode>
                                    {
                                        new ConditionNode(CheckAutoOn),
                                        new ConditionNode(CheckUserInput)
                                    }
                                ),
                                UniqueSkill.CreateSkillBTree(this, SkillTargets)
                            }
                        ),
                    }
                ),

                new SequenceNode // Attack Dicision
                (
                    new List<BaseNode>
                    {
                        new ConditionNode(CheckAttackRange),
                        new ActionNode(SetTargetToAttack),
                        new ActionNode(PerformAttack)
                    }
                ),
                new SequenceNode
                (
                    new List<BaseNode>
                    {
                        new ActionNode(SetDetectedTarget),
                        // CheckMoveable ?
                        new ActionNode(ChaseTarget)
                    }
                ),
                new ActionNode(StayIdle)
            }
        );
    }

    protected override BaseNode.ENodeState PerformAttack()
    {
        if(CurrentTarget == null || !CurrentTarget.gameObject.activeSelf && CurrentTarget.isDying)
        {
            UnitViewer.UnitAnimator.SetBool(UnitViewer.ParameterHash[(int)Parameter.Attack], false);
            IsAttacking = false;
            Debug.Log(" 타겟이 유효하지 않아 공격 실패."); // 공격 애니메이션 진행중 대상이 사라졌을 경우?
            return BaseNode.ENodeState.Failure;
        }
        
        
        if ((UnitModel.CurCc & CrowdControls.Taunt) != 0) // 걸린 상태이상 중 도발이 있을경우
        {
            if (UnitModel.CcCaster != null && UnitModel.CcCaster.gameObject.activeSelf) // 도발을 건 대상이 유효한 대상일 때
            {
                CurrentTarget = UnitModel.CcCaster;
            }
        }
        
        UnitViewer.CheckNeedFlip(transform, CurrentTarget.transform);
        // 공격을 시작
        // 공격 파라미터가 False였을 경우에만 True로 바꿔주며 공격 시작
        
        if(!UnitViewer.UnitAnimator.GetBool(UnitViewer.ParameterHash[(int)Parameter.Attack]) && _projectileObject == null)
        {
            UnitViewer.UnitAnimator.SetBool(UnitViewer.ParameterHash[(int)Parameter.Run], false);
            UnitViewer.UnitAnimator.SetBool(UnitViewer.ParameterHash[(int)Parameter.Attack], true);
            Debug.Log($"{CurrentTarget.gameObject.name}에 {gameObject.name}이 공격을 시작!");
            IsAttacking = true; // true로 바꿔줬으니 다음 트리 순회때 해당 조건문 실행x
            //CreateProjectileObject();
            return BaseNode.ENodeState.Running;
        }
        
        
        
        // 공격 진행중, Attack 파라미터 True 상태
        //if(UnitViewer.IsAnimationRunning())
        
        //if(UnitViewer.UnitAnimator.GetBool(UnitViewer.parameterHash[(int)UnitView.AniState.Attack]))
        {
            var stateInfo = UnitViewer.UnitAnimator.GetCurrentAnimatorStateInfo(0);
            //if (stateInfo.IsName("Attacking"))
            {
                if (stateInfo.normalizedTime < 1.0f)
                {

                    if (stateInfo.normalizedTime > 0.4f && stateInfo.normalizedTime < 0.6f)
                    {
                        // 투사체가 생성되기 적당한 타이밍
                        CreateProjectileObject();
                    }
                    
                    Debug.Log($"{gameObject.name}가 {CurrentTarget.gameObject.name}를 공격 중");
                    return BaseNode.ENodeState.Running;
                }
                else if (stateInfo.normalizedTime >= 1.0f && _projectileObject == null)
                {
                    // 공격 애니메이션이 끝났을 경우 + 공격 투사체가 사라졌을 경우
                    Debug.Log($"{gameObject.name}가 {CurrentTarget.gameObject.name}에 대한 공격을 완료");
                    UnitViewer.UnitAnimator.SetBool(UnitViewer.ParameterHash[(int)Parameter.Attack], false);
                    IsAttacking = false;
                    // 공격수행 데미지 적용 시킴
                    CurrentTarget.UnitModel.TakeDamage(UnitModel.AttackPoint);
                    
                    return BaseNode.ENodeState.Success;
                }
            }
            //else // 
            {
                Debug.Log("projectile이 아직 사라지지 않음"); 
                return BaseNode.ENodeState.Running;
            }
        }
    }

    private void CreateProjectileObject()
    {
        if(_projectileObject == null)
        {
            _projectileObject = Instantiate(_projectilePrefab, MuzzlePoint.position, Quaternion.identity);
            Projectile projectile = _projectileObject.GetComponent<Projectile>();
            if (projectile != null)
            {
                projectile.TargetPos = CurrentTarget.CenterPosition;
                projectile.Target = CurrentTarget;
            }
        }
    }
}