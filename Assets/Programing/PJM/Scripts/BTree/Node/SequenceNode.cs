using System.Collections.Generic;

public sealed class SequenceNode : INode
{
    private List<INode> _childs;

    public SequenceNode(List<INode> childs)
    {
        this._childs = childs;
    }

    /// <summary>
    /// Running 일때는 상태 유지, 다음 자식노드로 이동x, 다음 프레임에도 자식에 대한 평가 진행 필요
    /// </summary>
    /// <returns>ENodeState</returns>
    public INode.ENodeState Evaluate()
    {
        if (_childs == null || _childs.Count == 0)
            return INode.ENodeState.Failure;

        foreach (var child in _childs)
        {
            switch (child.Evaluate())
            {
                case INode.ENodeState.Running:
                    return INode.ENodeState.Running;
                case INode.ENodeState.Success:
                    continue;
                case INode.ENodeState.Failure:
                    return INode.ENodeState.Failure;
            }
        }
        
        return INode.ENodeState.Success;
    }
}
