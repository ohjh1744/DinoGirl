using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class SelectorNode : BaseNode
{
    private List<BaseNode> _childs;
    //private int _runningNodeIndex = -1;
    public SelectorNode(List<BaseNode> childs)
    {
        _childs = childs;
    }

    public override ENodeState Evaluate()
    {
        if (_childs == null || _childs.Count == 0)
            return ENodeState.Failure;

        // 이전에 Running중인 노드가 있었을 경우
        /*if (_runningNodeIndex != -1)
        {
            Debug.Log($"Running 중인 노드 있음 : {_childs[_runningNodeIndex]}");
            ENodeState result = _childs[_runningNodeIndex].Evaluate();
            if (result == ENodeState.Running)
            {
                return ENodeState.Running;
            }
                
            _runningNodeIndex = -1;
            return result;
        }*/

        // 이전에 Running중인 노드가 없었을 경우
        for (int i = 0; i < _childs.Count; i++)
        {
            ENodeState result = _childs[i].Evaluate();

            switch (result)
            {
                case ENodeState.Running:
                    return ENodeState.Running;
                case ENodeState.Success:
                    return ENodeState.Success;
                case ENodeState.Failure:
                    continue;
                
                /*// Running 체크 하는 버전
                case ENodeState.Running:
                    Debug.Log($"새로운 Running 노드 발견: {_childs[i]}");
                    _runningNodeIndex = i;
                    return ENodeState.Running;
                case ENodeState.Success:
                    _runningNodeIndex = -1;
                    return ENodeState.Success;
                case ENodeState.Failure:
                    continue;*/
            }
        }
        return ENodeState.Failure;
    }

    /*public override ENodeState Evaluate()
    {
        if (_childs == null)
            return ENodeState.Failure;

        foreach (var child in _childs)
        {
            switch (child.Evaluate())
            {
                case ENodeState.Running:
                    return ENodeState.Running;
                case ENodeState.Success:
                    return ENodeState.Success;
            }
        }

        return ENodeState.Failure;
    }*/
    
    
    public override void ResetNode()
    {
        //_runningNodeIndex = -1;
        foreach (var child in _childs)
        {
            child.ResetNode();
        }
    }
    
}
