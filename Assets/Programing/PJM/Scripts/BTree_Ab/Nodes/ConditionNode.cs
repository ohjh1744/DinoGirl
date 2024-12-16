using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionNode : BaseNode
{
    private Func<bool> _condition;

    public ConditionNode(Func<bool> condition)
    {
        _condition = condition;
    }

    public override ENodeState Evaluate()
    {
        if (_condition != null)
            return _condition.Invoke() ? ENodeState.Success : ENodeState.Failure;

        Debug.Log("조건이 정의되지 않음.");
        return ENodeState.Failure;
    }
}
