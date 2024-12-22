using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class UnitController : MonoBehaviour
{
    
    protected BehaviourTreeRunner _BTRunner;
    protected Animator _unitAnimator;
    public Animator UnitAnimator { get => _unitAnimator; set => _unitAnimator = value; }
    protected Transform _detectedEnemy;
    public Transform DetectedEnemy { get => _detectedEnemy; protected set => _detectedEnemy = value; }
    
    protected Transform _currentTarget;
    public Transform CurrentTarget { get => _currentTarget; protected set => _currentTarget = value; }
    
    protected int unitID;
    public int UnitID { get { return unitID; } }

    // 카메라 범위
    protected Vector2 _bottomLeft;
    protected Vector2 _topRight;
    
    
    [SerializeField] protected float _detectRange;
    public float DetectRange { get => _detectRange; protected set => _detectRange = value; }
    
    [SerializeField] protected float _attackRange;
    public float AttackRange { get => _attackRange; protected set => _attackRange = value; }
    
    [SerializeField] protected float _moveSpeed;
    public float MoveSpeed { get => _moveSpeed; protected set => _moveSpeed = value; }
    
    [SerializeField] protected LayerMask _allianceLayer;
    public LayerMask AllianceLayer { get => _allianceLayer; protected set => _allianceLayer = value; }
    [SerializeField] protected LayerMask _enemyLayer;
    public LayerMask EnemyLayer { get => _enemyLayer; protected set => _enemyLayer = value; }
    
    protected bool _attackStarted = false;
    public bool AttackStarted { get => _attackStarted; protected set => _attackStarted = value;}
    
    [SerializeField] protected bool _isPriorityTargetFar;
    public bool IsPriorityTargetFar { get => _isPriorityTargetFar; set => _isPriorityTargetFar = value; }
        

    
    
    
    protected virtual void Start()
    {
        SetLayer();
        SetDetectingArea();
        _unitAnimator = GetComponent<Animator>();
        BaseNode rootNode = SetBTree();
        _BTRunner = new BehaviourTreeRunner(rootNode);
    }

    protected virtual void Update()
    {
        if (Time.timeScale == 0)
            return;

        _BTRunner.Operate();
    }


    protected abstract BaseNode SetBTree(); // 각 유닛이 구현할 행동 트리 메서드

    protected bool IsAnimationRunning(string stateName)
    {
        /*AnimatorStateInfo stateInfo = UnitAnimator.GetCurrentAnimatorStateInfo(0);
        
        return stateInfo.IsName(stateName) && stateInfo.normalizedTime < 1.0f;*/
        if (UnitAnimator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
        {
            var normalizedTime = UnitAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            return normalizedTime != 0 && normalizedTime < 1.0f;
        }
        return false;
            

        
    }

    protected bool IsAnimationFinished(string stateName)
    {
        if (_unitAnimator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
        {
            var normalizedTime = _unitAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            return normalizedTime > 1.0f;
        }
        return false;
    }

    protected virtual void SetLayer()
    {
        string myLayerName = LayerMask.LayerToName(gameObject.layer);
        EnemyLayer = myLayerName == "UserCharacter" ? LayerMask.GetMask("Enemy") : LayerMask.GetMask("UserCharacter");
        AllianceLayer = LayerMask.GetMask(myLayerName);
    }

    protected BaseNode.ENodeState SetTargetToAttack()
    {
        if (DetectedEnemy != null)
        {
            CurrentTarget = DetectedEnemy;
            return BaseNode.ENodeState.Success;
        }
        return BaseNode.ENodeState.Failure;
    }
    
    protected BaseNode.ENodeState PerformAttack(string animationName) // 추후 해싱
    {
        // 타겟이 유효하지 않을때 
        if (CurrentTarget == null)
        {
            AttackStarted = false;
            return BaseNode.ENodeState.Failure;
        }
            
        // 공격을 시작
        if (!AttackStarted)
        {
            AttackStarted = true;
            UnitAnimator.SetTrigger("Attack");
            Debug.Log($"{CurrentTarget.gameObject.name}에 {gameObject.name}이 공격 시작!");
            StartCoroutine(ResetAttackRoutine((animationName)));
            return BaseNode.ENodeState.Running;
        }
        // 공격이 진행중
        if (AttackStarted && IsAnimationRunning(animationName))
        {
            Debug.Log($"공격 진행중 어택트리거 상태 : {AttackStarted}");
            return BaseNode.ENodeState.Running;
        }
        if (!AttackStarted)
        {
            // 공격 모션이 끝남, 공격모션이 끝나고 한번만 실행되어야 함
            Debug.Log($"공격 종료됨 어택트리거 상태 : {AttackStarted}");
            //AttackStarted = false;
            return BaseNode.ENodeState.Success;
        }

        return BaseNode.ENodeState.Failure;
    }
    
    // coroutine
    protected IEnumerator ResetAttackRoutine(string animationName)
    {
        /*while (IsAnimationRunning(animationName))
        {
            yield return null;
        }*/
        
        //UnitAnimator.SetTrigger("Attack");
        
        // 애니메이션의 길이만큼 대기 후 리셋
        yield return new WaitForSeconds(UnitAnimator.GetCurrentAnimatorStateInfo(0).length);
        AttackStarted = false;
        Debug.Log($"{animationName} 애니메이션 완료: 공격 리셋됨.");
    }

    protected BaseNode.ENodeState ChaseTarget()
    {
        if (DetectedEnemy != null)
        {
            float sqrDistance = Vector2.SqrMagnitude(DetectedEnemy.position - transform.position);
            if (sqrDistance > _attackRange * _attackRange)
            {
                transform.position = Vector2.MoveTowards(transform.position, DetectedEnemy.position, _moveSpeed * Time.deltaTime);
                Debug.Log($"타겟 {DetectedEnemy.gameObject.name}를 추적 중");
                return BaseNode.ENodeState.Running;
            }
            return BaseNode.ENodeState.Success;
        }
        return BaseNode.ENodeState.Failure;
    }

    protected BaseNode.ENodeState StayIdle()
    {
        Debug.Log("Idle 상태");
        //UnitAnimator.SetTrigger("Idle");
        return BaseNode.ENodeState.Success;
    }

    protected bool CheckAttackRange()
    {
        if (DetectedEnemy == null)
            return false;
        float sqrDistance = Vector2.SqrMagnitude(DetectedEnemy.position - transform.position);
        return sqrDistance <= AttackRange * AttackRange;
    }

    protected BaseNode.ENodeState SetDetectedTarget()
    {
        // 이미 감지된 적이 있었을경우엔 수행할 필요 없음,  바로 chase로 전환
        if (DetectedEnemy != null)
            return BaseNode.ENodeState.Success;
        
        Collider2D[] detectedColliders = Physics2D.OverlapAreaAll(_bottomLeft,_topRight, _enemyLayer);

        if (detectedColliders.Length == 0)
        {
            DetectedEnemy = null;
            return BaseNode.ENodeState.Failure;
        }
        
        float minDistance = float.MaxValue;
        float maxDistance = float.MinValue;
        Transform closetEnemy = null;
        Transform farthestEnemy = null;

        foreach (Collider2D collider in detectedColliders)
        {
            float distance = Vector2.Distance(transform.position, collider.transform.position);

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
        if (IsPriorityTargetFar)
        {
            // 가장 먼 타겟을 DetectedEnemy 로 설정
            DetectedEnemy = farthestEnemy;
        }
        else
        {
            // 가장 가까운 타겟을 DetectedEnemy로 설정
            DetectedEnemy = closetEnemy;
        }

        return BaseNode.ENodeState.Success;
        
    }
    
    
    
    // others
    protected void SetDetectingArea()
    {
        if (Camera.main != null)
        {
            _bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
            _topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));
        }
    }
    protected void OnDrawGizmos()
    {
        string layerName = LayerMask.LayerToName(gameObject.layer);
        Gizmos.color = (layerName == "UserCharacter") ? Color.green : Color.red;
        
        //Gizmos.color = Color.yellow;
        if(_detectedEnemy != null)
            Gizmos.DrawLine(transform.position, _detectedEnemy.position);

        Gizmos.color = Color.cyan;
        Vector2 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector2 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));
        
        Gizmos.DrawLine(new Vector3(bottomLeft.x, bottomLeft.y, 0), new Vector3(topRight.x, bottomLeft.y, 0)); // 아래쪽
        Gizmos.DrawLine(new Vector3(bottomLeft.x, topRight.y, 0), new Vector3(topRight.x, topRight.y, 0));    // 위쪽
        Gizmos.DrawLine(new Vector3(bottomLeft.x, bottomLeft.y, 0), new Vector3(bottomLeft.x, topRight.y, 0)); // 왼쪽
        Gizmos.DrawLine(new Vector3(topRight.x, bottomLeft.y, 0), new Vector3(topRight.x, topRight.y, 0));    // 오른쪽

        /*Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _detectRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);*/
    }
}

/*private BaseNode.ENodeState CheckAutoOn()
{
    if (BattleManager.Instance.IsAutoBattle)
    {
        return BaseNode.ENodeState.Success;
    }

    return BaseNode.ENodeState.Failure;
}

private BaseNode.ENodeState DetectEnemys()
{
    var overlapColliders = Physics2D.OverlapCircleAll(transform.position, _detectRange, LayerMask.GetMask("Player"));
    if (overlapColliders != null && overlapColliders.Length > 0)
    {
        _detectedEnemy = overlapColliders[0].transform;
        return BaseNode.ENodeState.Success;
    }

    _detectedEnemy = null;
    return BaseNode.ENodeState.Failure;
}


private BaseNode.ENodeState MoveToEnemy()
{
    if (_detectedEnemy != null)
    {
        float sqrDistance = Vector2.SqrMagnitude(_detectedEnemy.position - transform.position);
        if (sqrDistance < _attackRange * _attackRange)
        {
            return BaseNode.ENodeState.Success;
        }

        if (sqrDistance > Mathf.Epsilon)
        {
            transform.position = Vector2.MoveTowards(transform.position, _detectedEnemy.position, _moveSpeed * Time.deltaTime);
            return BaseNode.ENodeState.Running;
        }

    }
    return BaseNode.ENodeState.Failure;
}

private BaseNode.ENodeState DoAttack()
{
    if (IsAnimationRunning("attackStateNameTemp"))
    {
        return BaseNode.ENodeState.Running;
    }

    return BaseNode.ENodeState.Success;
}

private BaseNode.ENodeState TempMethod()
{
    return BaseNode.ENodeState.Success;
}*/

/*if (_detectedEnemy != null)
            return true;

        // 현재 카메라에 보이는 전체 영역 탐지

        //Rect screenRect = new Rect(bottomLeft.x, bottomLeft.y, topRight.x - bottomLeft.x, topRight.y - bottomLeft.y);
        Collider2D[] detectedEnemys = Physics2D.OverlapAreaAll(_bottomLeft,_topRight, _enemyLayer);

        if (detectedEnemys.Length > 0)
        {
            _detectedEnemy = detectedEnemys[0].transform;
            return true;
        }

        return false;*/
        
/*// 가장 먼저 탐지한 적을 우선적으로 공격할 경우
Collider2D[] detectedColliders = Physics2D.OverlapCircleAll(transform.position, _detectRange, _enemyLayer);
if (detectedColliders.Length > 0)
{
    _detectedEnemy = detectedColliders[0].transform;
    return true;
}
_detectedEnemy = null;
return false;*/