using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    // 추상 클래스로 돌리고 WarriorAI 등으로 상속받는 AI를 자식을 만들어야함
    private BehaviourTreeRunner _BTRunner;
    private Animator _animator; 
    /*private int _tempMana = 50;
    private string _tempRole = "Dealer";*/
    private Transform _detectedEnemy;
    private Transform _currentTarget;


    [SerializeField] private float _detectRange = 5.0f;
    [SerializeField] private float _attackRange = 2.0f;
    [SerializeField] private float _moveSpeed = 2.0f;
    [SerializeField] private LayerMask _allianceLayer;
    [SerializeField] private LayerMask _enemyLayer;
    
    private bool _attackTriggered = false;

    public float DetectRange
    { get => _detectRange; private set => _detectRange = value;}
    public float AttackRange
    { get => _attackRange; private set => _attackRange = value;}

    public float MoveSpeed
    { get => _moveSpeed; private set => _moveSpeed = value;}

    public LayerMask AllianceLayer 
    { get => _allianceLayer; private set => _allianceLayer = value;}

    public LayerMask EnemyLayer
    { get => _enemyLayer; private set => _enemyLayer = value;}
    
    private void Start()
    {
        SetLayer();
        _animator = GetComponent<Animator>();
        BaseNode rootNode = SetBTree();
        _BTRunner = new BehaviourTreeRunner(rootNode);
    }

    private void Update()
    {
        _BTRunner.Operate();
    }

    private BaseNode SetBTree()
    {
        return new SelectorNode // Behaviour Selector
        (
            new List<BaseNode>
            {
                new SequenceNode // Attack Dicision
                (
                    new List<BaseNode>
                    {
                        new ActionNode(CheckAttacking),
                        new ConditionNode(CheckAttackRange),
                        new ActionNode(SetTargetToAttack),
                        new ActionNode(AttackTarget),
                    }
                ),
                new SequenceNode // Detect Target
                (
                    new List<BaseNode>
                    {
                        new ConditionNode(CheckDetectingRange),
                        new ActionNode(SelectDetectedTargetToChase),
                        new ActionNode(ChaseTarget),
                    }
                ),
                new ActionNode(StayIdle)
            }
        );
    }

    private bool IsAnimationRunning(string stateName)
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
        {
            var normalizedTime = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

            return normalizedTime != 0 && normalizedTime < 1f;
        }
        
        return false;
    }

    private bool IsAnimationFinished(string stateName)
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
        {
            var normalizedTime = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

            return normalizedTime > 1.0f;
        }
        return false;
    }

    /// <summary>
    /// 적과 자신의 구분을 위해 레이어를 설정하는 메서드
    /// </summary>
    private void SetLayer()
    {
        string myLayerName = LayerMask.LayerToName(gameObject.layer);
        if (myLayerName == "UserCharacter")
        {
            _enemyLayer = LayerMask.GetMask("Enemy");
        }
        else
        {
            _enemyLayer = LayerMask.GetMask("UserCharacter");
        }
            
        _allianceLayer = LayerMask.GetMask(myLayerName);
    }
    
    // Actions

    private BaseNode.ENodeState CheckAttacking()
    {
        return IsAnimationRunning("Attacking") ? BaseNode.ENodeState.Running : BaseNode.ENodeState.Success;
    }
    
    private BaseNode.ENodeState SetTargetToAttack()
    {
        if (_detectedEnemy != null)
        {
            // 앞선 컨디션 노드에서 확인했으므로 필요 없을 가능성 있음 
            _currentTarget = _detectedEnemy;
            return BaseNode.ENodeState.Success;
        }
        return BaseNode.ENodeState.Failure;
    }

    private BaseNode.ENodeState AttackTarget()
    {
        if (_currentTarget != null && !_attackTriggered)
        {
            _attackTriggered = true;
            _animator.SetTrigger("Attack");
            Debug.Log($"{_currentTarget.gameObject.name}를 공격 시작! ");
            StartCoroutine(ResetAttackTrigger());
            return BaseNode.ENodeState.Running;
        }

        if (IsAnimationRunning("Attacking"))
        {
            return BaseNode.ENodeState.Running;
        }
        
        return BaseNode.ENodeState.Failure;
    }
    
    private IEnumerator ResetAttackTrigger()
    {
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
        Debug.Log("공격 완료됨. 실제 데미지 들어감");
        _attackTriggered = false;
    }
    
    private BaseNode.ENodeState SelectDetectedTargetToChase()
    {
        if (_detectedEnemy != null)
        {
            _currentTarget = _detectedEnemy;
            return BaseNode.ENodeState.Success;
        }
        
        return BaseNode.ENodeState.Failure;
    }

    private BaseNode.ENodeState ChaseTarget()
    {
        if (_currentTarget != null)
        {
            float sqrDistance = Vector2.SqrMagnitude(_currentTarget.position - transform.position);
            if (sqrDistance > _attackRange * _attackRange) 
            {
                transform.position = Vector2.MoveTowards(transform.position, _currentTarget.position, _moveSpeed * Time.deltaTime);
                Debug.Log($"타겟 : {_currentTarget.gameObject.name}를 Chase 중");
                return BaseNode.ENodeState.Running;
            }
            return BaseNode.ENodeState.Success;
                
        }
        return BaseNode.ENodeState.Failure;
    }

    private BaseNode.ENodeState StayIdle()
    {
        Debug.Log("Idle 상태");
        return BaseNode.ENodeState.Success;
    }
    
    // Conditions

    private bool CheckAttackRange()
    {
        if (_detectedEnemy == null)
            return false;
        
        // 최적화를 위해 sqrDistance를 사용
        float sqrDistance = Vector2.SqrMagnitude(_detectedEnemy.position - transform.position);
        return sqrDistance <= _attackRange * _attackRange;
    }



    private bool CheckDetectingRange()
    {
        Collider2D[] detectedColliders = Physics2D.OverlapCircleAll(transform.position, _detectRange, _enemyLayer);
        if (detectedColliders.Length > 0)
        {
            // detected Enemy를 정할 조건이 필요할 경우 추가
            _detectedEnemy = detectedColliders[0].transform;
            return true;
        }

        _detectedEnemy = null;
        return false;
    }
    private void OnDrawGizmos()
    {
        // Detect Range
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _detectRange);

        // Attack Range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
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
