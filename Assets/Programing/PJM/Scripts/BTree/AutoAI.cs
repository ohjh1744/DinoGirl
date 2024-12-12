using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEngine;

public class AutoAI : MonoBehaviour
{
    private BehaviourTreeRunner _BTRunner;
    private Animator _animator;
    private int _tempMana = 50;
    private string _tempRole = "Dealer";

    private void Start()
    {
        INode rootNode = SetBTree();
        _BTRunner = new BehaviourTreeRunner(rootNode);
    }

    private void Update()
    {
        _BTRunner.Operate();
    }

    private bool IsMyTurn()
    {
        // Todo : 턴 확인
        // 현재 임시로 항상 true
        return true;
    }

    private INode SetBTree()
    {
        return new SelectorNode
        (
            new List<INode>()
            {
                new SequenceNode
                (
                    new List<INode>()
                    {
                        new ActionNode(CheckAutoOn),
                        new ActionNode(CheckMyTurn),
                    }
                ),
                new SequenceNode
                (
                    new List<INode>()
                    {
                        new ActionNode(CheckEnemy),
                        new ActionNode(DoAttack),
                    }
                ),
                new SequenceNode
                (
                    new List<INode>()
                    {
                        new ActionNode(CheckEnemy),
                    }
                ),
                new ActionNode(PassTurn)
            }
        );
    }

    private INode.ENodeState CheckAutoOn()
    {
        if (TurnManager.Instance.IsAutoBattle)
        {
            return INode.ENodeState.Success;
        }
        else
        {
            return INode.ENodeState.Failure;
        }
    }
    private INode.ENodeState CheckMyTurn()
    {
        // Todo
        // 자기 턴인지 확인, 현재 반드시 자기턴
        if(IsMyTurn())
            return INode.ENodeState.Success;
        else
            return INode.ENodeState.Failure;
    }
    private INode.ENodeState CheckEnemy()
    {
        // Todo
        // 적이 남아있는지 확인, 임시로 반드시 남아있다고 해줌
        return INode.ENodeState.Success;
    }

    private INode.ENodeState DoAttack()
    {
        // Todo
        // 무슨 공격을 할지 선택하는 로직
        if (IsAnimationRunning("attackStateNameTemp"))
        {
            return INode.ENodeState.Running;
        }
        
        return INode.ENodeState.Success;
    }

    private INode.ENodeState PassTurn()
    {
        return INode.ENodeState.Success;
    }
    
    /*private INode.ENodeState CheckRole(string role)
    {
        switch (role)
        {
            case "Dealer":
                
        }
    }*/
    
    
    bool IsAnimationRunning(string stateName)
    {
        if(_animator is not null)
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



