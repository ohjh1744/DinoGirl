using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class ActionNode : BaseNode
{
    private Func<ENodeState> _action;

    public ActionNode(Func<ENodeState> action)
    {
        _action = action;
    }

    public override ENodeState Evaluate() => _action?.Invoke() ?? ENodeState.Failure;
}
