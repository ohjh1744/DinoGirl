using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorNode : BaseNode
{
    private List<BaseNode> _childs;
    private int _runningNodeIndex = -1;
    public SelectorNode(List<BaseNode> childs)
    {
        _childs = childs;
    }

    public override ENodeState Evaluate()
    {
        if (_childs == null || _childs.Count == 0)
            return ENodeState.Failure;

        if (_runningNodeIndex != -1)
        {
            ENodeState result = _childs[_runningNodeIndex].Evaluate();
            if (result != ENodeState.Running)
            {
                _runningNodeIndex = -1;
            }
            return result;
        }

        for (int i = 0; i < _childs.Count; i++)
        {
            ENodeState result = _childs[i].Evaluate();

            switch (result)
            {
                case ENodeState.Running:
                    return ENodeState.Running;
                case ENodeState.Success:
                    return ENodeState.Success;
                // 트리를 다시 돌 때 앞서 Running이 있었을 경우
                // Success가 나오면 앞서 진행하던 Running중인 Action Node를 멈춰주는것이 필요할 수 있음
                
                // Running 체크 하는 버전
                /*case ENodeState.Running:
                    _runningNodeIndex = i;
                    return ENodeState.Running;
                case ENodeState.Success:
                    return ENodeState.Success;
                case ENodeState.Failure:
                    continue;*/
            }
        }
        return ENodeState.Failure;
    }

    public override void ResetNode()
    {
        _runningNodeIndex = -1;
        foreach (var child in _childs)
        {
            child.ResetNode();
        }
    }
    
}
