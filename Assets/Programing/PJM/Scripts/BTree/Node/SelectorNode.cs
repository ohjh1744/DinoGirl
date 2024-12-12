using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorNode : INode
{
    // 자식 노드 중, 처음으로 Success나 Running 상태를 가진 노드가 발생시 해당 노드까지 진행 후 멈춤
    private List<INode> _childs;

    public SelectorNode(List<INode> childs)
    {
        this._childs = childs;
    }

    /// <summary>
    /// 자식 상태에 따른 결과를 반환
    /// 자식 상태가 Running : Running 반환
    /// 자식 상태가 Success : Success 반환
    /// 자식 상태가 Failure : 다음 자식으로 이동
    /// </summary>
    /// <returns>ENodeState</returns>
    public INode.ENodeState Evaluate()
    {
        if (_childs == null)
            return INode.ENodeState.Failure;

        foreach (var child in _childs)
        {
            switch (child.Evaluate())
            {
                case INode.ENodeState.Running :
                    return INode.ENodeState.Running;
                case INode.ENodeState.Success:
                    return INode.ENodeState.Success;
            }
        }
        
        return INode.ENodeState.Failure;
    }
}
