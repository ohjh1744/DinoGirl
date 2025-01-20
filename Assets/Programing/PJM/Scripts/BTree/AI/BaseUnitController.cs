using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class BaseUnitController : MonoBehaviour
{
    [SerializeField] private Transform _centerPosition;
    public Transform CenterPosition { get => _centerPosition; private set => _centerPosition = value; }

    [SerializeField] private Transform _muzzlePoint;
    public Transform MuzzlePoint { get => _muzzlePoint; protected set => _muzzlePoint = value; }
    private bool _inAttackDelay;
    public bool isDying { get; set; } = false;

    private UnitView _unitViewer;
    public UnitView UnitViewer { get => _unitViewer; private set => _unitViewer = value; }
    private UnitModel _unitModel;
    public UnitModel UnitModel { get => _unitModel; private set => _unitModel = value; }

    private BehaviourTreeRunner _BTRunner;
    private BaseUnitController _detectedEnemy;
    public BaseUnitController DetectedEnemy { get => _detectedEnemy; set => _detectedEnemy = value; }

    private BaseUnitController _currentTarget;

    protected BaseUnitController CurrentTarget { get => _currentTarget; set => _currentTarget = value; }

    private BaseUnitController _tauntSource;
    public BaseUnitController TauntSource { get => _tauntSource; set => _tauntSource = value; }

    private Skill _curSkill;
    public Skill CurSkill { get => _curSkill; set => _curSkill = value; }
    
    protected int unitID;
    public int UnitID => unitID;

    private LayerMask _allianceLayer;
    public LayerMask AllianceLayer { get => _allianceLayer; private set => _allianceLayer = value; }
    private LayerMask _enemyLayer;
    public LayerMask EnemyLayer { get => _enemyLayer; private set => _enemyLayer = value; }

    private bool _isAttacking = false;

    protected bool IsAttacking { get => _isAttacking; set => _isAttacking = value;}
    
    public float CoolTimeCounter { get; set; }
    public bool IsSkillRunning { get; set; }

    private Skill.SkillState _curSkillState;
    public Skill.SkillState CurSkillState { get => _curSkillState; set => _curSkillState = value; }

    protected virtual void Awake()
    {
        if(UnitViewer == null)
            UnitViewer = GetComponent<UnitView>();
        if(UnitModel == null)
            UnitModel = GetComponent<UnitModel>();
        if(CenterPosition == null)
            CenterPosition = transform.Find("CenterPosition");
    }

    protected virtual void OnEnable()
    {
        UnitModel.OnDeath += HandleDeath;
    }

    protected virtual void Start()
    {
        SetLayer();
        BaseNode rootNode = SetBTree();
        _BTRunner = new BehaviourTreeRunner(rootNode);
    }

    protected virtual void Update()
    {
        if (Time.timeScale == 0)
            return;
        
        // 쿨타임 
        if (CoolTimeCounter > 0)
        {
            CoolTimeCounter -= UnitModel.CoolDownAcc * Time.deltaTime;
        }

        _BTRunner.Operate();
    }

    protected void OnDisable()
    {
        UnitModel.OnDeath -= HandleDeath;
    }


    protected abstract BaseNode SetBTree(); // 각 유닛이 구현할 행동 트리 메서드

    protected virtual void SetLayer()
    {
        string myLayerName = LayerMask.LayerToName(gameObject.layer);
        EnemyLayer = myLayerName == "UserCharacter" ? LayerMask.GetMask("Enemy") : LayerMask.GetMask("UserCharacter");
        AllianceLayer = LayerMask.GetMask(myLayerName);
    }
    
    protected BaseNode.ENodeState CheckUnitDying()
    {
        if (!isDying)
            return BaseNode.ENodeState.Failure;
        
        var stateInfo = UnitViewer.UnitAnimator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Dead"))
        {
            if (stateInfo.normalizedTime < 1.0f)
            {
                // 죽는중
                return BaseNode.ENodeState.Running;
            }
            else if (stateInfo.normalizedTime >= 1.0f)
            {
                isDying = false;
                gameObject.SetActive(false);
                return BaseNode.ENodeState.Success;
            }
        }

        return BaseNode.ENodeState.Failure;
    }

    protected BaseNode.ENodeState CheckBattleWin()
    {
        if (BattleSceneManager.Instance.curBattleState != BattleSceneManager.BattleState.Win)
            return BaseNode.ENodeState.Failure;
        UnitViewer.UnitAnimator.SetTrigger(UnitViewer.ParameterHash[(int)Parameter.Win]);
        return BaseNode.ENodeState.Success;
    }

    protected BaseNode.ENodeState CheckCrowdControl()
    {
        // 더 추가 예정?, 여러 상태이상들이 동시에 걸렸을때 관리 필요
        switch (UnitModel.CurCc)
        {
            case CrowdControls.Stun:
                return BaseNode.ENodeState.Running;
            default:
                return BaseNode.ENodeState.Failure;
        }
    }

    protected BaseNode.ENodeState SetTargetToAttack()
    {
        if(DetectedEnemy != null && DetectedEnemy.gameObject.activeSelf)
        {
            CurrentTarget = DetectedEnemy;
            //UnitViewer.CheckNeedFlip(transform, CurrentTarget.transform);
            return BaseNode.ENodeState.Success;
        }
        return BaseNode.ENodeState.Failure;
    }
    
    protected virtual BaseNode.ENodeState PerformAttack()
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
        UnitViewer.UnitAnimator.SetBool(UnitViewer.ParameterHash[(int)Parameter.Run], false);
        
        if(!UnitViewer.UnitAnimator.GetBool(UnitViewer.ParameterHash[(int)Parameter.Attack]))
        {
            UnitViewer.UnitAnimator.SetBool(UnitViewer.ParameterHash[(int)Parameter.Attack], true);
            IsAttacking = true; 
            return BaseNode.ENodeState.Running;
        }
        
        // 공격 진행중, Attack 파라미터 True 상태
        var stateInfo = UnitViewer.UnitAnimator.GetCurrentAnimatorStateInfo(0);
        switch (stateInfo.normalizedTime)
        {
            case < 1.0f:
                //Debug.Log($"{gameObject.name}가 {CurrentTarget.gameObject.name}를 공격 중");
                return BaseNode.ENodeState.Running;
            case >= 1.0f:
                // 공격 애니메이션이 끝났을 경우
                //Debug.Log($"{gameObject.name}가 {CurrentTarget.gameObject.name}에 대한 공격을 완료");
                UnitViewer.UnitAnimator.SetBool(UnitViewer.ParameterHash[(int)Parameter.Attack], false);
                IsAttacking = false;
                // 공격수행 데미지 적용 시킴
                CurrentTarget.UnitModel.TakeDamage(UnitModel.AttackPoint);
                return BaseNode.ENodeState.Success;
            default:
                Debug.LogWarning("예상치 못한 상태에서 공격 실패.");
                return BaseNode.ENodeState.Failure;
        }
    }

    protected virtual bool IsSkillAlreadyRunning()
    {
        return IsSkillRunning;
    }

    protected bool CheckMoveable()
    {
        return !(IsAttacking || IsSkillRunning);
    }
    
    protected IEnumerator AttackRoutine(string animationName)
    {
        /*while (UnitViewer.IsAnimationRunning(animationName))
        {
            yield return null;
        }*/
        
        //UnitAnimator.SetTrigger("Attack");
        
        // 애니메이션의 길이만큼 대기 후 리셋 + 후딜레이?
        // 현재 애니메이터의 info를 가져오기 때문에 실제 공격 애니메이션의 길이인지 확신 할 수 없음 다른방법 필요
        //Debug.Log($"{UnitViewer.UnitAnimator.GetCurrentAnimatorStateInfo(0).length + _tempDelay}초 후 리셋");
        yield return new WaitForSeconds(1.0f); // 애니메이션 길이
        //UnitViewer.UnitAnimator.SetBool(UnitViewer.parameterHash[(int)UnitView.AniState.Attack], false);
        IsAttacking = false;
        Debug.Log($"{animationName} 애니메이션 완료: 공격 리셋됨.");
    }
    protected BaseNode.ENodeState ChaseTarget()
    {
        if(DetectedEnemy != null && DetectedEnemy.gameObject.activeSelf)
        {
            UnitViewer.CheckNeedFlip(transform, DetectedEnemy.transform);
            float sqrDistance = Vector2.SqrMagnitude(DetectedEnemy.gameObject.transform.position - transform.position);
            if (sqrDistance > UnitModel.AttackRange * UnitModel.AttackRange) // 타겟이 공격 범위보다 멀때
            {
                UnitViewer.UnitAnimator.SetBool(UnitViewer.ParameterHash[(int)Parameter.Attack], false); // 임시
                UnitViewer.UnitAnimator.SetBool(UnitViewer.ParameterHash[(int)Parameter.Run], true);
                transform.position = Vector2.MoveTowards(transform.position, DetectedEnemy.gameObject.transform.position, UnitModel.Movespeed * Time.deltaTime);
                //Debug.Log($"타겟 {DetectedEnemy.gameObject.name}를 추적 중");
                return BaseNode.ENodeState.Running;
            }
            else // 타겟이 공격 범위 내에 있을때 , 행동트리 후반에 있어서 공격으로 바로 넘어가서 뜨지 않음
            {
                UnitViewer.UnitAnimator.SetBool(UnitViewer.ParameterHash[(int)Parameter.Run], false);
                //Debug.Log($"타겟 {DetectedEnemy.gameObject.name} 추적완료");
                return BaseNode.ENodeState.Success;
            }
        }
        // 타겟이 없을때
        Debug.Log("타겟 없음");
        UnitViewer.UnitAnimator.SetBool(UnitViewer.ParameterHash[(int)Parameter.Run], false);
        return BaseNode.ENodeState.Failure;
    }

    protected BaseNode.ENodeState StayIdle()
    {
        Debug.Log("Idle 상태");
        UnitViewer.UnitAnimator.SetBool(UnitViewer.ParameterHash[(int)Parameter.Attack], false);
        UnitViewer.UnitAnimator.SetBool(UnitViewer.ParameterHash[(int)Parameter.Run], false);
        return BaseNode.ENodeState.Success;
    }

    protected bool CheckAttackRange()
    {
        if(DetectedEnemy != null && DetectedEnemy.gameObject.activeSelf)
        {
            if (CurrentTarget != null && CurrentTarget.gameObject.activeSelf)
            {
                float sqrDistanceToCur = Vector2.SqrMagnitude(CurrentTarget.gameObject.transform.position - transform.position);
                return sqrDistanceToCur <= UnitModel.AttackRange * UnitModel.AttackRange;
            }
            float sqrDistance = Vector2.SqrMagnitude(DetectedEnemy.gameObject.transform.position - transform.position);
            return sqrDistance <= UnitModel.AttackRange * UnitModel.AttackRange;
        }
        
        return false;
    }
    
    protected void HandleDeath()
    {
        isDying = true;
        UnitViewer.UnitAnimator.SetTrigger(UnitViewer.ParameterHash[(int)Parameter.Die]);
    }
    
    protected bool CheckSkillCooltimeBack()
    {
        if (CoolTimeCounter <= 0)
        {
            CoolTimeCounter = 0;
            return true;
        }
        return false;
    }

    protected abstract BaseNode.ENodeState SetDetectedTarget();
    /*{
        //Debug.LogWarning("카메라 범위에서 적 체크는 과거사양입니다.");
        throw new System.NotImplementedException();
        /*Debug.LogWarning("기본 타겟 세팅 메서드 실행중");
        if ((UnitModel.CurCc & CrowdControls.Taunt) != 0) // 걸린 상태이상 중 도발이 있을경우
        {
            if (UnitModel.CcCaster != null && UnitModel.CcCaster.gameObject.activeSelf) // 도발을 건 대상이 유효한 대상일 때
            {
                DetectedEnemy = UnitModel.CcCaster;
            }
        }
        // 이미 감지된 적이 있었을경우엔 수행할 필요 없음,  바로 chase로 전환
        if(DetectedEnemy != null && DetectedEnemy.gameObject.activeSelf)
            return BaseNode.ENodeState.Success;
        
        Collider2D[] detectedColliders = Physics2D.OverlapAreaAll(_bottomLeft,_topRight, _enemyLayer);

        if (detectedColliders.Length == 0)
        {
            DetectedEnemy = null;
            return BaseNode.ENodeState.Failure;
        }
        
        float minDistance = float.MaxValue;
        float maxDistance = float.MinValue;
        BaseUnitController closetEnemy = null;
        BaseUnitController farthestEnemy = null;

        foreach (var col in detectedColliders)
        {
            BaseUnitController unit = col.gameObject.GetComponent<BaseUnitController>();
            if (unit == null)
            {
                Debug.LogWarning($"{col.gameObject.name}에 BaseUnitController가 없다.");
                continue;
            }
            
            float distance = Vector2.Distance(transform.position, col.transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                closetEnemy = unit;
            }

            if (distance > maxDistance)
            {
                maxDistance = distance;
                farthestEnemy = unit;
            }
        }
        if (UnitModel.IsPriorityTargetFar)
        {
            // 가장 먼 타겟을 DetectedEnemy 로 설정
            DetectedEnemy = farthestEnemy;
        }
        else
        {
            // 가장 가까운 타겟을 DetectedEnemy로 설정
            DetectedEnemy = closetEnemy;
        }
        
        UnitViewer.CheckNeedFlip(transform, DetectedEnemy.transform);

        return BaseNode.ENodeState.Success;#1#
        
    }*/

    /*protected void ExtractDetectedTargetFromList()
    {
        // 스폰이 완료된 이벤트에 맞춰서 실행될 메서드? 아니면 그냥 리스트에서 추출만?
        if(AllianceLayer == )
        
    }*/
    
    // others
    /*protected void SetDetectingArea()
    {
        if (Camera.main != null)
        {
            _bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
            _topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));
        }
    }*/

    /*protected void OnDrawGizmos()
    {
        string layerName = LayerMask.LayerToName(gameObject.layer);
        Gizmos.color = (layerName == "UserCharacter") ? Color.green : Color.red;
        
        //Gizmos.color = Color.yellow;
        if(DetectedEnemy != null && DetectedEnemy.gameObject.activeSelf)
            Gizmos.DrawLine(transform.position, _detectedEnemy.gameObject.transform.position);

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
        Gizmos.DrawWireSphere(transform.position, _attackRange);#1#
    }*/
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