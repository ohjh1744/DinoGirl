using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayableUnitController : UnitController
{
    [SerializeField] private Skill _uniqueSkill;
    public Skill UniqueSkill {get => _uniqueSkill; protected set => _uniqueSkill = value; }
    private float skillRange;

    public float SkillRange { get => skillRange; protected set => skillRange = value;}
    
    private bool _skillTriggered;
    public bool SkillTriggered { get => _skillTriggered; protected set => _skillTriggered = value; }

    private float _coolTime;
    public float CoolTime {get => _coolTime; protected set => _coolTime = value; }
    
    private float _coolTimeCounter;
    public float CoolTimeCounter {get => _coolTimeCounter; protected set => _coolTimeCounter = value; }
    
    protected bool _isAutoOn; // 임시, 배틀매니저 값을 참조하는게 좋음
    
    protected override void Start()
    {
        base.Start();
        
    }
    
    
    protected override BaseNode SetBTree()
    {
        // 여러 유닛들이 생길 수 있고 유닛바다 행동트리 노드가 변경 될 위험이 있어 하위 클래스로 구현
        Debug.LogWarning("자식에서 구현하세요 Btree");
        throw new System.NotImplementedException();
    }

    protected abstract BaseNode.ENodeState SetTargetToSkill();

    protected abstract BaseNode.ENodeState PerformSkill(string animationName);
    
    
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
        SkillTriggered = false;
        CoolTimeCounter = CoolTime;
        Debug.Log($"{animationName} 애니메이션 완료: 스킬 리셋됨.");
    }


    protected bool CheckSkillCooltime()
    {
        if (CoolTimeCounter <= 0)
        {
            CoolTimeCounter = UniqueSkill.Cooltime;
            return true;
        }
        else
        {
            CoolTimeCounter -= Time.deltaTime;
            return false;
        }
    }

    protected bool CheckAutoOn()
    {
        // Todo : 배틀매니저에서 오토전투가 On 되었는지 확인함
        // 임시로 항시 True 를 반환
        return true;
    }

    protected bool CheckUserInput()
    {
        // Todo : UI와 연결해 버튼이 눌렸을 시 변경
        // 임시로 항시 True를 반환
        return true;
    }

    protected abstract bool CheckSkillRange();


}
