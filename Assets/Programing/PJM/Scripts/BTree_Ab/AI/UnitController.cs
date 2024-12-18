using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class UnitController : MonoBehaviour
{
    protected BehaviourTreeRunner _BTRunner;
    protected Animator _animator;
    protected Transform _detectedEnemy;
    protected Transform _currentTarget;

    // 카메라 범위
    protected Vector2 _bottomLeft;
    protected Vector2 _topRight;
    
    
    [SerializeField] protected float _detectRange;
    [SerializeField] protected float _attackRange;
    [SerializeField] protected float _moveSpeed;
    [SerializeField] protected LayerMask _allianceLayer;
    [SerializeField] protected LayerMask _enemyLayer;
    
    protected bool _attackTriggered = false;
    [SerializeField] protected bool _isPriorityTargetFar;
        


    public Transform DetectedEnemy { get => _detectedEnemy; protected set => _detectedEnemy = value; }
    public Transform CurrentTarget { get => _currentTarget; protected set => _currentTarget = value; }
    
    public bool IsPriorityTargetFar { get => _isPriorityTargetFar; set => _isPriorityTargetFar = value; }

    public float DetectRange { get => _detectRange; protected set => _detectRange = value; }
    public float AttackRange { get => _attackRange; protected set => _attackRange = value; }
    public float MoveSpeed { get => _moveSpeed; protected set => _moveSpeed = value; }
    public LayerMask AllianceLayer { get => _allianceLayer; protected set => _allianceLayer = value; }
    public LayerMask EnemyLayer { get => _enemyLayer; protected set => _enemyLayer = value; }

    protected virtual void Awake()
    {
        
    }

    protected virtual void Start()
    {
        SetLayer();
        SetDetectingArea();
        _animator = GetComponent<Animator>();
        BaseNode rootNode = SetBTree();
        _BTRunner = new BehaviourTreeRunner(rootNode);
    }

    protected virtual void Update()
    {
        _BTRunner.Operate();
    }

    protected abstract BaseNode SetBTree(); // 각 유닛이 구현할 행동 트리 메서드

    protected bool IsAnimationRunning(string stateName)
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
        {
            var normalizedTime = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            return normalizedTime != 0 && normalizedTime < 1f;
        }
        return false;
    }

    protected bool IsAnimationFinished(string stateName)
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
        {
            var normalizedTime = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
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
        return BaseNode.ENodeState.Success;
    }

    protected bool CheckAttackRange()
    {
        if (DetectedEnemy == null) return false;
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
    }
    
    // coroutine
    protected IEnumerator ResetAttackTrigger(string animationName)
    {
        // 애니메이션의 길이만큼 대기 후 리셋
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
        _attackTriggered = false;
        Debug.Log($"{animationName} 애니메이션 완료: 공격 리셋됨.");
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
