using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayableBaseUnitController : BaseUnitController
{
    //[SerializeField] BattleSceneUIView battleSceneUIView; // 임시 주입
    [SerializeField] private Skill _uniqueSkill;
    public Skill UniqueSkill {get => _uniqueSkill; protected set => _uniqueSkill = value; }
    private float skillRange;

    public float SkillRange { get => skillRange; protected set => skillRange = value;}
    
    private bool _isSkillRunning;
    public bool IsSkillRunning { get => _isSkillRunning; protected set => _isSkillRunning = value; }
    
    private List<Transform> _skillTargets;
    public List<Transform> SkillTargets { get => _skillTargets; protected set => _skillTargets = value; }

    private bool _skillInputed;
    public bool SkillInputed { get => _skillInputed;  set => _skillInputed = value; }
    
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


    protected bool CheckSkillCooltimeBack()
    {
        if (IsSkillRunning)
        {
            CoolTimeCounter -= Time.deltaTime;
            return false;
        }
        
        if (CoolTimeCounter > 0)
        {
            CoolTimeCounter -= Time.deltaTime;
            return false;
        }

        if (CoolTimeCounter <= 0)
        {
            CoolTimeCounter = 0;
            return true;
        }
        
        Debug.LogWarning("예외상황");
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
        }*/
    }

    protected bool CheckAutoOn()
    {
        // Todo : 배틀매니저에서 오토전투가 On 되었는지 확인함
        return TempBattleContext.Instance.isAutoOn;
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
