using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class SequenceNode : BaseNode
{
    private List<BaseNode> _childs;

    public SequenceNode(List<BaseNode> childs)
    {
        _childs = childs;
    }

    public override ENodeState Evaluate()
    {
        if (_childs == null || _childs.Count == 0)
            return ENodeState.Failure;

        foreach (var child in _childs)
        {
            switch (child.Evaluate())
            {
                case ENodeState.Running:
                    return ENodeState.Running;
                case ENodeState.Success:
                    continue;
                case ENodeState.Failure:
                    return ENodeState.Failure;
            }
        }

        return ENodeState.Success;
    }

    public override void ResetNode()
    {
        foreach (var child in _childs)
        {
            child.ResetNode();
        }
    }
}
