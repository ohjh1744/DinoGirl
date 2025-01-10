using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecoratorNode : BaseNode
{
    // 조건노드에서 Success를 반환하면 자식노드를 수행하는 노드
    private ConditionNode _conditionNode;
    private BaseNode _childNode;

    public DecoratorNode(ConditionNode conditionNode, BaseNode childNode)
    {
        _conditionNode = conditionNode;
        _childNode = childNode;
    }

    public override ENodeState Evaluate()
    {
        if (_conditionNode.Evaluate() == ENodeState.Success)
        {
            return _childNode.Evaluate();
        }

        return ENodeState.Failure;
    }

    public override void ResetNode()
    {
        _conditionNode.ResetNode();
        _childNode.ResetNode();
    }
}
