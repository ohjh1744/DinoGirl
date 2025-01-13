using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnitControllerWithProjectile : EnemyBaseUnitController
{
    [SerializeField] private GameObject _projectilePrefab;
    //[SerializeField] private Transform _muzzlePoint;
    private GameObject _projectileObject;

    protected override void Awake()
    {
        base.Awake();
        if (_projectilePrefab == null)
        {
            Debug.LogError("투사체 프리팹 존재하지 않음");
            return;
        }
        
        if(MuzzlePoint == null)
            MuzzlePoint = transform.Find("MuzzlePoint");
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
                        new ActionNode(CheckUnitDying),
                        new ActionNode(CheckCrowdControl),
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
            Debug.Log(" 타겟이 유효하지 않아 공격 실패.");
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
        UnitViewer.UnitAnimator.SetBool(UnitViewer.ParameterHash[(int)Parameter.Run], false);
        if(!UnitViewer.UnitAnimator.GetBool(UnitViewer.ParameterHash[(int)Parameter.Attack]) && _projectileObject == null)
        {
            UnitViewer.UnitAnimator.SetBool(UnitViewer.ParameterHash[(int)Parameter.Attack], true);
            Debug.Log($"{CurrentTarget.gameObject.name}에 {gameObject.name}이 공격을 시작!");
            IsAttacking = true; // true로 바꿔줬으니 다음 트리 순회때 해당 조건문 실행x
            // 투사체 생성
            _projectileObject = Instantiate(_projectilePrefab, MuzzlePoint.position, Quaternion.identity);
            Projectile projectile = _projectileObject.GetComponent<Projectile>();
            if (projectile != null)
            {
                projectile.Target = CurrentTarget.CenterPosition;
            }
            
            
            // 공격 애니메이션의 길이 + 지정된 공격 후딜레이 후 공격을 종료시켜줄 코루틴 // 현재 미사용
            // 공격 판정은 들어간 뒤 후 딜레이가 적용되어야 하므로 바꿀 필요가 있음
            //StartCoroutine(AttackRoutine("Attacking"));
            return BaseNode.ENodeState.Running;
        }
        
        // 공격 진행중, Attack 파라미터 True 상태
        //if (IsAttacking) // 위의 조건이 있어 사실상 필요 없을지도
        
        //if(UnitViewer.IsAnimationRunning())
        
        //if(UnitViewer.UnitAnimator.GetBool(UnitViewer.parameterHash[(int)UnitView.AniState.Attack]))
        {
            var stateInfo = UnitViewer.UnitAnimator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Attacking"))
            {
                if (stateInfo.normalizedTime < 1.0f)
                {
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
            else // 
            {
                // 트랜지션 이동중 애니메이션이 블렌딩 되어서? 애니메이션 상태로 확인하면 아래 로그가 출력됨 (이건아님)
                // Todo : 원인 발견및 해결
                Debug.Log("IsAttacking이 True지만 현재 애니메이션 상태가 Attacking이 아님"); // IsAttacking이 True면 일단 공격중이니 Running 반환
                //Debug.Log("Attack Bool 파라미터가 True지만 현재 애니메이션 상태가 Attacking이 아님");
                return BaseNode.ENodeState.Running;
            }
        }
        
        // 공격을 끝내야 할 경우, AttackRoutine 코루틴에서 애니메이션 길이 이후 IsAttacking을 false로 바꿔줬을 때
        // Attack 파라미터는 아직 True 상태
        //if (!IsAttacking)
        
        //if(!UnitViewer.IsAnimationRunning("Attacking"))
        
        /*if(stateInfo.IsName("Attacking") && stateInfo.normalizedTime >= 1.0f)
        {
            Debug.Log($"공격 종료됨");
            UnitViewer.UnitAnimator.SetBool(UnitViewer.parameterHash[(int)UnitView.AniState.Attack], false);
            IsAttacking = false;
            //StartCoroutine(AttackDelayRoutine());
            return BaseNode.ENodeState.Success;
        }*/
        
        Debug.LogWarning("예상치 못한 상태에서 공격 실패.");
        return BaseNode.ENodeState.Failure;
    }
}
