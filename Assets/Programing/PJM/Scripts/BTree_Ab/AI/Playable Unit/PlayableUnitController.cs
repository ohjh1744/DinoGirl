using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayableUnitController : UnitController
{
    private float _coolTime;
    public float CoolTime {get => _coolTime; private set => _coolTime = value; }
    
    private float _coolTimeCounter;
    public float CoolTimeCounter {get => _coolTimeCounter; private set => _coolTimeCounter = value; }
    
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

    public abstract BaseNode.ENodeState UseSkill();


    protected bool CheckSkillCooltime()
    {
        if (_coolTimeCounter <= 0)
        {
            _coolTimeCounter = CoolTime;
            return true;
        }
        else
        {
            _coolTimeCounter -= Time.deltaTime;
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
    
    
}
