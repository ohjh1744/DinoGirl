using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoAI : MonoBehaviour
{
    private BehaviourTreeRunner _BTRunner;
    private Animator _animator;
    private int _tempMana = 50;
    private string _tempRole = "Dealer";
    private Transform _detectedEnemy;
    private float _detectRange = 10.0f;
    private float _attackRange = 5.0f;
    private float _moveSpeed = 3.0f;

    private void Start()
    {
        BaseNode rootNode = SetBTree();
        _BTRunner = new BehaviourTreeRunner(rootNode);
    }

    private void Update()
    {
        _BTRunner.Operate();
    }

    private bool IsMyTurn()
    {
        return true;
    }

    private BaseNode SetBTree()
    {
        return new SelectorNode
        (
            new List<BaseNode>
            {
                new SequenceNode
                (
                    new List<BaseNode>
                    {
                        new ActionNode(CheckAutoOn),
                    }
                ),
                new SequenceNode
                (
                    new List<BaseNode>
                    {
                        new ActionNode(DetectEnemys),
                        new ActionNode(MoveToEnemy),
                    }
                ),
                new SequenceNode
                (
                    new List<BaseNode>
                    {
                        new ActionNode(DoAttack),
                    }
                ),
                new ActionNode(TempMethod)
            }
        );
    }

    private BaseNode.ENodeState CheckAutoOn()
    {
        if (TurnManager.Instance.IsAutoBattle)
        {
            return BaseNode.ENodeState.Success;
        }
        else
        {
            return BaseNode.ENodeState.Failure;
        }
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

    private BaseNode.ENodeState CheckAttackRange()
    {
        Debug.Log("사거리 내에 적 있음");
        return BaseNode.ENodeState.Success;
    }

    private BaseNode.ENodeState MoveToEnemy()
    {
        if (_detectedEnemy != null)
        {
            if (Vector2.SqrMagnitude(_detectedEnemy.position - transform.position) < _attackRange * _attackRange)
            {
                return BaseNode.ENodeState.Success;
            }

            transform.position = Vector2.MoveTowards(transform.position, _detectedEnemy.position, _moveSpeed * Time.deltaTime);
            return BaseNode.ENodeState.Running;
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
    }

    bool IsAnimationRunning(string stateName)
    {
        if (_animator is not null)
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
            {
                var normalizedTime = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

                return normalizedTime != 0 && normalizedTime < 1f;
            }
        }

        return false;
    }
}
