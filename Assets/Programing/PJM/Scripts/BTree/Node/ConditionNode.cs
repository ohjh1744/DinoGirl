using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionNode : INode
{
    private System.Func<bool> _condition;

    public ConditionNode(System.Func<bool> condition)
    {
        _condition = condition;
    }
    /// <summary>
    /// 조건을 검사해 조건식이 참이면 Success, 거짓이면 Failure를 반환
    /// </summary>
    /// <returns></returns>
    public INode.ENodeState Evaluate()
    {
        if (_condition != null) 
            return _condition.Invoke() ? INode.ENodeState.Success : INode.ENodeState.Failure;
        
        Debug.Log("조건이 정의되지 않음.");
        return INode.ENodeState.Failure;

    }
}
