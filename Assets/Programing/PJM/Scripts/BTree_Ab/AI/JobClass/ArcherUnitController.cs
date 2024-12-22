using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherUnitController : UnitController
{
    protected void Awake()
    {
        //DetectRange = 20.0f;
        AttackRange = 15.0f;
        MoveSpeed = 1.0f;
    }
    
    protected override BaseNode SetBTree()
    {
        return new SelectorNode
        (
            new List<BaseNode>
            {
                new SequenceNode
                (
                    new List<BaseNode>
                    {
                        new ConditionNode(CheckAttackRange),
                        new ActionNode(SetTargetToAttack),
                        new ActionNode(() => PerformAttack("Attacking"))
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
    
    /*private BaseNode.ENodeState PerformAttack(string animationName)
    {
        if (_currentTarget == null) return BaseNode.ENodeState.Failure;
        if (!_attackTriggered)
        {
            _attackTriggered = true;
            _unitAnimator.SetTrigger("Attack");
            Debug.Log($"{_currentTarget.gameObject.name}에 아처 공격!");
            StartCoroutine(ResetAttackTrigger(animationName));
            return BaseNode.ENodeState.Running;
        }
        if (IsAnimationRunning(animationName))
        {
            // 공격 애니메이션 진행중
            return BaseNode.ENodeState.Running;
        }
        // 공격 애니메이션 끝남 
        // 타겟이 여전히 유효한지 검사 currentTarget
        // 유효 하다면 데미지를 주었다는것을 전달 , 배틀 매니저를 거칠수도 있음
        // BattleManager.Instance.CharList. TakeDamage(공격력 or 스킬의 계수 * 공격력, currentTaget) // 전투 상태의 캐릭터의 공격력을 받아옴
        //  OnTakeDamaged; + 데미지 받았을때 소리, 피격이펙트 출력 
        //-------------------------------------------------------------
        /*      
             캐릭터 모델에 정의 :  public event Action OnDamageTaken;
             
             다른 클래스에서
             OnDamageTaken += 
                
                
         * 
         *  캐릭터의 모델 혹은 배틀매니저쪽에서 최종 데미지 계산 후 적용
         *  public void TakeDamageInvoke(float Damage)
         * {
         *      float finalDamage = CaculateFinalDamage(damage); // 방어력 계산?
         *      ApplyDamage(finalDamage); // 실제 데미지를 적용시켜주는 메서드
         * }
         *
         * 아래의 메서드는 위에 포함되어있어도 상관 없음 아마?
         * public float CaculateFinalDamage(float damage)
         * {
         *     float caculatingFinalDamage = damage - defensePoint
         *     방어력까지 계산한 최종 데미지 계산
         *      return caculatinFinalDamage;
         * 
         * }
         * 
         
         * 
         *
         * 
         #1#
        return BaseNode.ENodeState.Success;
    }*/
}
