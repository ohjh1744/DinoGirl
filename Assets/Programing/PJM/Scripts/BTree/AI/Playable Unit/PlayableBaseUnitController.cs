using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayableBaseUnitController : BaseUnitController
{
    //[SerializeField] BattleSceneUIView battleSceneUIView; // 임시 주입
    [SerializeField] private Skill _uniqueSkill;
    public Skill UniqueSkill {get => _uniqueSkill; protected set => _uniqueSkill = value; }
    //private float skillRange;

    //public float SkillRange { get => skillRange; protected set => skillRange = value;}
    
    //private bool _isSkillRunning;
    //public bool IsSkillRunning { get => _isSkillRunning; protected set => _isSkillRunning = value; }
    
    private List<BaseUnitController> _skillTargets;
    public List<BaseUnitController> SkillTargets { get => _skillTargets; protected set => _skillTargets = value; }

    private bool _skillInputed;
    public bool SkillInputed { get => _skillInputed;  set => _skillInputed = value; }

    protected override void Awake()
    {
        base.Awake();
    }
    
    protected override void Start()
    {
        base.Start();
        
        
    }
    
    
    protected override BaseNode SetBTree()
    {
        // 여러 유닛들이 생길 수 있고 유닛바다 행동트리 노드가 변경 될 위험이 있어 하위 클래스로 구현
        // 여기에 기본 트리를 구현해 놓을수도 있음
        Debug.LogWarning("자식에서 구현하세요 Btree");
        throw new System.NotImplementedException();
    }

    /*protected abstract bool CheckSkillRange();
    protected abstract BaseNode.ENodeState SetTargetToSkill();

    protected abstract BaseNode.ENodeState PerformSkill(string animationName);*/
    
    
    //public abstract BaseNode.ENodeState UseSkill();

    protected override BaseNode.ENodeState SetDetectedTarget()
    {
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
        
        if (BattleSceneManager.Instance.enemyUnits.Count == 0)
            return BaseNode.ENodeState.Failure;
        
        float minDistance = float.MaxValue;
        float maxDistance = float.MinValue;
        BaseUnitController closetEnemy = null;
        BaseUnitController farthestEnemy = null;

        foreach (var unit in BattleSceneManager.Instance.enemyUnits)
        {
            if (unit == null || !unit.gameObject.activeSelf)
                continue;

            float distance = Vector2.Distance(transform.position, unit.transform.position);

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

        if (DetectedEnemy == null) 
            return BaseNode.ENodeState.Failure;
        
        UnitViewer.CheckNeedFlip(transform, DetectedEnemy.transform);
        return BaseNode.ENodeState.Success;
    }


    protected IEnumerator ResetSkillTrigger(string animationName)
    {
        /*while (UnitAnimator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
        {
            yield return null;
        }*/
        
        // 애니메이션의 길이만큼 대기 후 리셋
        yield return new WaitForSeconds(UnitViewer.UnitAnimator.GetCurrentAnimatorStateInfo(0).length);
        //UnitAnimator.ResetTrigger("Skill");
        IsSkillRunning = false;
        //CoolTimeCounter = CoolTime;
        Debug.Log($"{animationName} 애니메이션 완료: 스킬 리셋됨.");
    }


    /*protected bool CheckSkillCooltimeBack()
    {
        /*if (IsSkillRunning)
        {
            CoolTimeCounter -= Time.deltaTime;
            return false;
        }
        
        if (CoolTimeCounter > 0)
        {
            CoolTimeCounter -= Time.deltaTime;
            return false;
        }#1#

        if (CoolTimeCounter <= 0)
        {
            CoolTimeCounter = 0;
            return true;
        }
        
        return false;
        

        
        
        /*if (CoolTimeCounter <= 0)
        {
            CoolTimeCounter = 0;
            //CoolTimeCounter = UniqueSkill.Cooltime;
            return true;
        }
        else
        {
            CoolTimeCounter -= Time.deltaTime;
            return false;
        }#1#
    }*/

    protected BaseNode.ENodeState CheckWinBattle()
    {
        if (BattleSceneManager.Instance.curBattleState == BattleSceneManager.BattleState.Win)
        {
            // 승리했을 시
            UnitViewer.UnitAnimator.SetTrigger(UnitViewer.ParameterHash[(int)Parameter.Win]);
            return BaseNode.ENodeState.Success;
        }

        return BaseNode.ENodeState.Failure;
    }

    protected bool CheckAutoOn()
    {
        if (BattleSceneManager.Instance == null)
        {
            Debug.LogWarning("BattleSceneManager의 Instance가 없습니다.");
            return false;
        }
        /*// Todo : 배틀매니저에서 오토전투가 On 되었는지 확인함
        if(TempBattleContext.Instance == null)
            return false;*/
        
        return BattleSceneManager.Instance.isAutoOn;
    }

    protected bool CheckUserInput()
    {
        if (SkillInputed)
        {
            SkillInputed = false;
            return true;
        }

        return false;
    }

    


}
