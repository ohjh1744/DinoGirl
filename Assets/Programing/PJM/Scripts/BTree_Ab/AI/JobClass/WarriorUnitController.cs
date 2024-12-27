using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WarriorUnitController : BaseUnitController
{
    //[SerializeField] private bool _isAssassin;
    protected void Awake()
    {
        //DetectRange = 20.0f;
        //AttackRange = 2.0f;
        //MoveSpeed = 2.0f;
    }

    protected override void Start()
    {
        base.Start();
        // 추가로 해줄 동작 설정
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

    /*protected override bool CheckDetectingRange()
    {
        if (DetectedEnemy != null)
            return true;
        
        Collider2D[] detectedColliders = Physics2D.OverlapAreaAll(_bottomLeft,_topRight, _enemyLayer);

        if (detectedColliders.Length == 0)
        {
            DetectedEnemy = null;
            return false;
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

        return true;
    }*/
    
    // skill

    /*private BaseNode.ENodeState PerformAttack(string animationName)
    {
        if (_currentTarget == null) return BaseNode.ENodeState.Failure;
        if (!_attackTriggered)
        {
            _attackTriggered = true;
            _unitAnimator.SetTrigger("Attack");
            Debug.Log($"{_currentTarget.gameObject.name}에 워리어 공격!");
            StartCoroutine(ResetAttackTrigger(animationName));
            return BaseNode.ENodeState.Running;
        }
        if (IsAnimationRunning(animationName))
        {
            return BaseNode.ENodeState.Running;
        }
        return BaseNode.ENodeState.Success;
    }*/
}
